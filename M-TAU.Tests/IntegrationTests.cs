using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using M_TAU.Application.Dtos.Catalog;
using M_TAU.Application.Dtos.Chat;
using M_TAU.Application.Dtos.Common;
using M_TAU.Application.Dtos.Identity;
using M_TAU.Application.Dtos.Payment;
using M_TAU.Application.Dtos.Transaction;
using M_TAU.Domain.Catalog;
using M_TAU.Domain.Identity;
using M_TAU.Domain.Transaction;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace M_TAU.Tests;

public sealed class IntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public IntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    // DS01: Registrar + Login → JWT retornado
    [Fact]
    public async Task DS01_Register_And_Login_Returns_Jwt()
    {
        var client = _factory.CreateClient();
        var email = $"ds01-{Guid.NewGuid():N}@mtau.test";

        var register = await client.PostAsJsonAsync("/api/users",
            new UserCreateDto("DS01 User", email, "SecurePass!1", UserType.Buyer));
        Assert.True(register.IsSuccessStatusCode, $"Register failed: {register.StatusCode}");

        var login = await client.PostAsJsonAsync("/api/auth/login",
            new { Email = email, Password = "SecurePass!1" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var payload = await login.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.Token));
        Assert.Contains('.', payload.Token);
    }

    // DS02: Criar anúncio de produto
    [Fact]
    public async Task DS02_Create_Product_As_Seller()
    {
        var client = _factory.CreateClient();
        var (sellerId, token) = await RegisterAndLoginAsync(client, UserType.Seller);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new ProductCreateDto(
            "Cadeira de Rodas Motorizada",
            2500.00m,
            sellerId,
            "Seminova, em ótimo estado.",
            new TechnicalSpecCreateDto(DisabilityCategory.Physical, "90x60x110cm", 120.0, "Até 8h"));

        var response = await client.PostAsJsonAsync("/api/products", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        Assert.NotNull(created);
        Assert.Equal("Cadeira de Rodas Motorizada", created!.Title);
        Assert.Equal(sellerId, created.SellerId);
    }

    // DS03: Buscar com filtro por deficiência
    [Fact]
    public async Task DS03_Search_With_Disability_Category_Filter()
    {
        var client = _factory.CreateClient();
        var (sellerId, token) = await RegisterAndLoginAsync(client, UserType.Seller);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Seed two products with different categories
        await client.PostAsJsonAsync("/api/products", new ProductCreateDto(
            "Bengala Branca", 80.00m, sellerId, null,
            new TechnicalSpecCreateDto(DisabilityCategory.Visual)));
        await client.PostAsJsonAsync("/api/products", new ProductCreateDto(
            "Andador Sênior", 420.00m, sellerId, null,
            new TechnicalSpecCreateDto(DisabilityCategory.Physical)));

        client.DefaultRequestHeaders.Authorization = null;
        var paged = await client.GetFromJsonAsync<PaginatedResult<ProductResponseDto>>(
            $"/api/products?category={DisabilityCategory.Visual}");

        Assert.NotNull(paged);
        Assert.Contains(paged!.Items, p => p.Title == "Bengala Branca");
        Assert.DoesNotContain(paged.Items, p => p.Title == "Andador Sênior");
    }

    // DS04: Fluxo de checkout (cria pedido + payment checkout + webhook)
    [Fact]
    public async Task DS04_Checkout_Creates_Order_And_Completes_On_Webhook()
    {
        var client = _factory.CreateClient();
        var (sellerId, sellerToken) = await RegisterAndLoginAsync(client, UserType.Seller);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);

        var productResp = await client.PostAsJsonAsync("/api/products", new ProductCreateDto(
            "Prótese Auditiva", 1800.00m, sellerId, null,
            new TechnicalSpecCreateDto(DisabilityCategory.Auditory)));
        productResp.EnsureSuccessStatusCode();
        var product = await productResp.Content.ReadFromJsonAsync<ProductResponseDto>();
        Assert.NotNull(product);

        var (buyerId, buyerToken) = await RegisterAndLoginAsync(client, UserType.Buyer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);

        var orderResp = await client.PostAsJsonAsync("/api/orders",
            new OrderCreateDto(buyerId, product!.Id, product.Price));
        Assert.Equal(HttpStatusCode.Created, orderResp.StatusCode);
        var order = await orderResp.Content.ReadFromJsonAsync<OrderResponseDto>();
        Assert.NotNull(order);

        var checkoutResp = await client.PostAsJsonAsync("/api/payments/checkout",
            new PaymentCheckoutRequestDto(order!.Id));
        checkoutResp.EnsureSuccessStatusCode();
        var checkout = await checkoutResp.Content.ReadFromJsonAsync<PaymentCheckoutResponseDto>();
        Assert.NotNull(checkout);
        Assert.False(string.IsNullOrWhiteSpace(checkout!.CheckoutUrl));

        client.DefaultRequestHeaders.Authorization = null;
        var webhookResp = await client.PostAsJsonAsync("/api/payments/webhook",
            new PaymentWebhookDto(order.Id, "approved", order.Id.ToString()));
        webhookResp.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var refreshed = await client.GetFromJsonAsync<OrderResponseDto>($"/api/orders/{order.Id}");
        Assert.NotNull(refreshed);
        Assert.Equal(OrderStatus.Completed, refreshed!.Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);
        var soldProduct = await client.GetFromJsonAsync<ProductResponseDto>($"/api/products/{product.Id}");
        Assert.NotNull(soldProduct);
        Assert.Equal(ProductStatus.Sold, soldProduct!.Status);
    }

    // DS05: Envio/recebimento de mensagem no chat (via SignalR hub)
    [Fact]
    public async Task DS05_Send_And_Receive_Message_Via_SignalR()
    {
        var client = _factory.CreateClient();
        var (sellerId, sellerToken) = await RegisterAndLoginAsync(client, UserType.Seller);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);
        var productResp = await client.PostAsJsonAsync("/api/products", new ProductCreateDto(
            "Cadeira Pneumática", 1200.00m, sellerId, null,
            new TechnicalSpecCreateDto(DisabilityCategory.Physical)));
        productResp.EnsureSuccessStatusCode();
        var product = await productResp.Content.ReadFromJsonAsync<ProductResponseDto>();
        Assert.NotNull(product);

        var (buyerId, buyerToken) = await RegisterAndLoginAsync(client, UserType.Buyer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);

        var sessionResp = await client.PostAsJsonAsync("/api/chatsessions",
            new ChatSessionCreateDto(buyerId, sellerId, product!.Id));
        sessionResp.EnsureSuccessStatusCode();
        var session = await sessionResp.Content.ReadFromJsonAsync<ChatSessionResponseDto>();
        Assert.NotNull(session);

        var hubUrl = _factory.Server.BaseAddress + "hubs/chat";
        var hub = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                options.AccessTokenProvider = () => Task.FromResult<string?>(buyerToken);
            })
            .Build();

        var received = new TaskCompletionSource<MessageResponseDto>(TaskCreationOptions.RunContinuationsAsynchronously);
        hub.On<MessageResponseDto>("ReceiveMessage", msg => received.TrySetResult(msg));

        await hub.StartAsync();
        await hub.SendAsync("JoinSession", session!.Id);
        await hub.SendAsync("SendMessage",
            new MessageCreateDto(session.Id, buyerId, "Olá, tenho interesse no produto!"));

        var completed = await Task.WhenAny(received.Task, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.Same(received.Task, completed);
        var msg = await received.Task;
        Assert.Equal("Olá, tenho interesse no produto!", msg.Content);
        Assert.Equal(buyerId, msg.SenderId);

        await hub.DisposeAsync();
    }

    private async Task<(Guid Id, string Token)> RegisterAndLoginAsync(HttpClient client, UserType role)
    {
        var email = $"{role.ToString().ToLowerInvariant()}-{Guid.NewGuid():N}@mtau.test";
        var password = "SecurePass!1";

        var registerResp = await client.PostAsJsonAsync("/api/users",
            new UserCreateDto($"{role} User", email, password, role));
        registerResp.EnsureSuccessStatusCode();
        var user = await registerResp.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(user);

        var loginResp = await client.PostAsJsonAsync("/api/auth/login",
            new { Email = email, Password = password });
        loginResp.EnsureSuccessStatusCode();
        var payload = await loginResp.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(payload);

        return (user!.Id, payload!.Token);
    }

    private sealed record TokenResponse(string Token);
}

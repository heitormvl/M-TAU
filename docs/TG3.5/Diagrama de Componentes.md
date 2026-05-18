# Diagrama de Componentes – M-TAU

> **TG3 – Deadline 5 | Modelagem Parte II**  
> Representa os componentes de software da solução, suas responsabilidades e as dependências entre eles, mapeando a estrutura real da solução .NET (`M-TAU.sln`).

---

## Visão da Solução (.NET)

```mermaid
flowchart LR
    subgraph CLIENT["&lt;&lt;Component&gt;&gt; M-TAU.Client\n(Blazor WebAssembly)"]
        direction TB
        Pages["Pages\n(Home, Perfil, Detalhes, Chat, Checkout)"]
        Layout["Layout\n(MainLayout, NavBar)"]
        HttpClient["HttpClient\n(Serviço REST)"]
        SignalRClient["SignalR Client\n(Chat em tempo real)"]
    end

    subgraph API["&lt;&lt;Component&gt;&gt; M-TAU.API\n(ASP.NET Core Web API)"]
        direction TB
        AuthCtrl["AuthController\n(POST /auth/login, POST /auth/register)"]
        UsersCtrl["UsersController\n(GET/PUT /users/{id})"]
        ProductsCtrl["ProductsController\n(CRUD /products)"]
        OrdersCtrl["OrdersController\n(POST/GET /orders)"]
        FeedbacksCtrl["FeedbacksController\n(POST /feedbacks)"]
        ChatCtrl["ChatSessionsController\n(GET /chat-sessions)"]
        MessagesCtrl["MessagesController\n(POST/GET /messages)"]
        ChatHub["ChatHub\n(SignalR – /hubs/chat)"]
        JwtMiddleware["JWT Middleware\n(Autenticação)"]
        ProgramDI["Program.cs\n(DI + Swagger + CORS)"]
    end

    subgraph APP["&lt;&lt;Component&gt;&gt; M-TAU.Application\n(Camada de Aplicação)"]
        direction TB
        Services["Services\n(IProductService, IOrderService,\nIUserService, IChatSessionService,\nIMessageService, IFeedbackService)"]
        DTOs["DTOs\n(Create, Response, Filter)"]
        Validators["Validators\n(FluentValidation)"]
        Mappers["AutoMapper Profiles\n(CatalogMappingProfile,\nChatMappingProfile, etc.)"]
        AppDI["DependencyInjection.cs"]
    end

    subgraph DOMAIN["&lt;&lt;Component&gt;&gt; M-TAU.Domain\n(Núcleo de Domínio)"]
        direction TB
        Entities["Entities\n(User, Product, TechnicalSpec,\nPhoto, Order, Feedback,\nChatSession, Message)"]
        RepoInterfaces["Repository Interfaces\n(IRepository&lt;T,K&gt;,\nIProductRepository,\nIUserRepository, etc.)"]
        Common["Common\n(EntityBase&lt;TKey&gt;, ValueObject)"]
        Enums["Enumerações\n(UserType, ProductStatus,\nDisabilityCategory,\nOrderStatus, MessageStatus)"]
    end

    subgraph INFRA["&lt;&lt;Component&gt;&gt; M-TAU.Infrastructure\n(Infraestrutura)"]
        direction TB
        AppDbCtx["AppDbContext\n(EF Core – DbSets)"]
        Configurations["Entity Configurations\n(Fluent API)"]
        RepoImpl["Repository Implementations\n(ProductRepository,\nUserRepository, etc.)"]
        InfraDI["DependencyInjection.cs\n(AddDbContext, AddRepositories)"]
    end

    subgraph EXTERNAL["Sistemas Externos"]
        AzureSQL[("Azure SQL Database\n(SQL Server)")]
        PayGW["Gateway de Pagamento\n(Stripe / Mercado Pago)"]
        ViaCEP["API ViaCEP\n(Validação de CEP)"]
    end

    %% ── DEPENDÊNCIAS ────────────────────────────────────────────────────
    CLIENT -->|"HTTPS REST (JSON)"| API
    CLIENT <-->|"WebSocket (SignalR)"| ChatHub

    API --> APP
    API --> INFRA

    APP --> DOMAIN

    INFRA --> DOMAIN
    INFRA --> AzureSQL

    API -->|"POST /charges (REST)"| PayGW
    API -->|"GET /cep/{cep} (REST)"| ViaCEP
```

---

## Detalhamento das Dependências

| Componente | Depende de | Justificativa |
|---|---|---|
| `M-TAU.Client` | `M-TAU.API` (via HTTP/WS) | SPA Blazor consome endpoints REST e SignalR |
| `M-TAU.API` | `M-TAU.Application` | Controllers delegam lógica para Services |
| `M-TAU.API` | `M-TAU.Infrastructure` | Registra DbContext e repositórios via DI |
| `M-TAU.Application` | `M-TAU.Domain` | Services operam sobre entidades e interfaces de repositório |
| `M-TAU.Infrastructure` | `M-TAU.Domain` | Implementações de repositório dependem das interfaces do domínio |
| `M-TAU.Infrastructure` | Azure SQL | Persistência via EF Core |
| `M-TAU.API` | Gateway de Pagamento | Processamento de transações (RF06) |
| `M-TAU.API` | API ViaCEP | Preenchimento automático de endereço (RF08) |

---

## Diagrama de Pacotes (Package Diagram)

Representa as dependências em nível de namespace dentro de cada projeto.

```mermaid
flowchart LR
    subgraph M_TAU_Domain["M-TAU.Domain"]
        D_Common["M_TAU.Domain.Common"]
        D_Identity["M_TAU.Domain.Identity"]
        D_Catalog["M_TAU.Domain.Catalog"]
        D_Transaction["M_TAU.Domain.Transaction"]
        D_Chat["M_TAU.Domain.Chat"]
        D_Repositories["M_TAU.Domain.Repositories"]
    end

    subgraph M_TAU_Application["M-TAU.Application"]
        A_Services["M_TAU.Application.Services"]
        A_Dtos["M_TAU.Application.Dtos"]
        A_Validators["M_TAU.Application.Validators"]
        A_Mappers["M_TAU.Application.Mappers"]
    end

    subgraph M_TAU_Infrastructure["M-TAU.Infrastructure"]
        I_Persistence["M_TAU.Infrastructure.Persistence"]
    end

    subgraph M_TAU_API["M-TAU.API"]
        API_Controllers["M_TAU.API.Controllers"]
    end

    D_Identity --> D_Common
    D_Catalog --> D_Common
    D_Transaction --> D_Common
    D_Chat --> D_Common
    D_Repositories --> D_Identity
    D_Repositories --> D_Catalog
    D_Repositories --> D_Transaction
    D_Repositories --> D_Chat

    A_Services --> D_Repositories
    A_Services --> A_Dtos
    A_Validators --> A_Dtos
    A_Mappers --> D_Identity
    A_Mappers --> D_Catalog
    A_Mappers --> D_Transaction
    A_Mappers --> D_Chat

    I_Persistence --> D_Repositories
    I_Persistence --> D_Identity
    I_Persistence --> D_Catalog

    API_Controllers --> A_Services
    API_Controllers --> A_Dtos
```

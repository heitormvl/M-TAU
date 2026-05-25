# M-TAU — Marketplace de Tecnologias Assistivas

Plataforma para venda de equipamentos de tecnologia assistiva seminovos, conectando vendedores e compradores com filtros por categoria de deficiência, chat em tempo real, gateway de pagamento e avaliação pós-compra.

## Stack

- **API:** ASP.NET Core 10 (Web API + SignalR), EF Core 10 (SQLite em dev / Azure SQL em produção), JWT auth, FluentValidation, AutoMapper.
- **Front-end:** Blazor WebAssembly 10 + MudBlazor.
- **Testes:** xUnit + `Microsoft.AspNetCore.Mvc.Testing`.
- **Empacotamento:** Docker + docker-compose (nginx para o WASM).
- **CI/CD:** GitHub Actions (build/test no push para `main`, deploy para Azure no merge).

## Estrutura

```
M-TAU.Domain         # Entidades + interfaces de repositório
M-TAU.Application    # Casos de uso (serviços, DTOs, mappers, validadores)
M-TAU.Infrastructure # EF Core, repositórios, migrações
M-TAU.API            # Controllers + Hub SignalR + auth JWT
M-TAU.Client         # Blazor WASM (MudBlazor)
M-TAU.Tests          # Testes de integração ponta a ponta dos 5 DSs
```

## Como rodar

### Local (sem Docker)

```powershell
dotnet restore M-TAU.slnx
dotnet build M-TAU.slnx
dotnet run --project M-TAU.API
# em outro terminal
dotnet run --project M-TAU.Client
```

A API sobe em `http://localhost:5144`, o cliente em `http://localhost:5213`.

### Docker

```bash
docker compose up --build
```

API em `:5144`, cliente em `:5213`. SQLite e uploads são persistidos em volumes nomeados.

### Testes

```powershell
dotnet test M-TAU.Tests
```

Executa os 5 testes de integração que cobrem os diagramas de sequência DS01–DS05.

## Endpoints principais

| Recurso        | Método  | Rota                                       | Autenticação |
| -------------- | ------- | ------------------------------------------ | ------------ |
| Auth           | POST    | `/api/auth/login`                          | público      |
| Usuários       | POST    | `/api/users`                               | público      |
| Usuários       | DELETE  | `/api/users/{id}`                          | autenticado (anonimiza dados pessoais – LGPD) |
| Produtos       | GET     | `/api/products?category=&pageNumber=&pageSize=` | público (paginado) |
| Produtos       | POST    | `/api/products`                            | Seller       |
| Fotos          | POST    | `/api/products/{id}/photos/upload`         | Seller (multipart) |
| Pedidos        | POST    | `/api/orders`                              | autenticado  |
| Pagamentos     | POST    | `/api/payments/checkout`                   | autenticado  |
| Pagamentos     | POST    | `/api/payments/webhook`                    | público (gateway) |
| Feedback       | POST    | `/api/feedbacks`                           | autenticado (somente pedido `Completed`) |
| Chat (REST)    | POST    | `/api/chatsessions`                        | autenticado  |
| Chat (Realtime)| Hub     | `/hubs/chat` (`JoinSession`, `SendMessage`, `LeaveSession`) | JWT via `access_token` query |
| Admin          | GET     | `/api/admin/metrics`                       | Admin        |
| Admin          | PATCH   | `/api/admin/products/{id}/moderate`        | Admin        |
| Seller         | GET     | `/api/seller/metrics`                      | Seller/Admin |

Spec OpenAPI auto-gerada e exposta via Scalar em `/scalar` no ambiente de desenvolvimento.

## Pagamento

A configuração padrão usa o sandbox do Mercado Pago. Em produção, definir:

```json
"Payments": {
  "Provider": "MercadoPago",
  "CheckoutBaseUrl": "https://www.mercadopago.com.br/checkout/v1/redirect"
}
```

O webhook em `/api/payments/webhook` aceita `{ orderId, status }` e:
- `approved` / `paid` / `completed` → marca pedido como `Completed` e produto como `Sold`.
- `cancelled` / `rejected` → marca pedido como `Cancelled`.

## Roteiro de demo (1–2 jun 2026)

1. **DS01 — Cadastro & login.** Registrar Buyer e Seller na tela `/register`; logar; mostrar JWT no localStorage.
2. **DS02 — Anúncio.** Seller acessa `/seller/products/new`, preenche título/preço/categoria, faz upload de fotos, publica. Produto aparece em `/catalog`.
3. **DS03 — Busca filtrada.** No `/catalog`, filtrar por `Categoria: Física` (e por preço); produto criado aparece, outros não.
4. **DS04 — Chat ao vivo.** Buyer abre o produto, clica em "Contatar vendedor". Sessão é criada e abre `/buyer/chat/{id}`. Em outra aba como Seller, abrir a mesma sessão. Trocar mensagens em tempo real (SignalR).
5. **DS05 — Checkout & pagamento.** Buyer clica "Comprar"; preenche CEP (ViaCEP autopreenche cidade/bairro); clica "Ir para pagamento" → redirect para sandbox Mercado Pago. Simular webhook (POST `/api/payments/webhook` com `status=approved`); pedido vira `Completed`, produto vira `Sold`. Buyer envia feedback em `/buyer/feedback/{orderId}`.
6. **Admin.** Login com role `Admin`, dashboard exibe contagens reais; moderar um anúncio.
7. **Docker.** `docker compose up --build` e mostrar os serviços rodando em containers.
8. **CI/CD.** Abrir GitHub Actions, mostrar pipeline verde.
9. **Testes.** `dotnet test` — 5 verdes.

## Conformidade

- **WCAG (RNF01):** componentes MudBlazor com `aria-label`, alternativas textuais em imagens, navegação por teclado, contraste AA.
- **HTTPS (RNF07):** `UseHttpsRedirection` na API; TLS terminado pelo Azure App Service / Static Web App em produção.
- **LGPD (RNF06):** `DELETE /api/users/{id}` anonimiza nome, e-mail e hash de senha, preservando integridade referencial dos registros transacionais já anonimizados.

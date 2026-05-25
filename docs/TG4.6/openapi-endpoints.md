# Spec de Endpoints — M-TAU API

> Gerado a partir do `v1.json` (OpenAPI 3.1.1) exportado pelo Scalar em `http://localhost:5144/scalar`.  
> O arquivo `v1.json` original está nesta mesma pasta.  
> Base URL dev: `http://localhost:5144`

---

## Autenticação

Endpoints protegidos exigem:

```
Authorization: Bearer <JWT>
```

O hub SignalR aceita via query string:

```
ws://localhost:5144/hubs/chat?access_token=<JWT>
```

---

## Enumerações

| Enum | Valores (inteiro) |
|------|-------------------|
| `UserType` | 0 = Buyer · 1 = Seller · 2 = Admin |
| `ProductStatus` | 0 = Active · 1 = Paused · 2 = Sold · 3 = Removed |
| `OrderStatus` | 0 = Pending · 1 = Confirmed · 2 = Completed · 3 = Cancelled |
| `MessageStatus` | 0 = Sent · 1 = Read |
| `DisabilityCategory` | 0 = Física · 1 = Visual · 2 = Auditiva · 3 = Cognitiva |

---

## Admin

### `GET /api/Admin/metrics` — `[Authorize(Roles="Admin")]`

**Response 200:**
```json
{
  "users": 12,
  "products": 34,
  "orders": 8,
  "chatSessions": 5
}
```

### `PATCH /api/Admin/products/{id}/moderate` — `[Authorize(Roles="Admin")]`

| Parâmetro | Em | Tipo |
|-----------|----|------|
| `id` | path | `uuid` |

**Body:**
```json
{ "status": 1 }
```
*(ver enum `ProductStatus`)*

---

## Auth

### `POST /api/Auth/login` — público

**Body:**
```json
{ "email": "string", "password": "string" }
```

**Response 200:**
```json
{
  "token": "eyJ...",
  "expiresAt": "2026-06-01T00:00:00Z",
  "user": { "id": "uuid", "name": "string", "email": "string", "role": 0, "createdAt": "datetime" }
}
```

---

## Usuários

### `POST /api/Users` — público (registro)

**Body (`UserCreateDto`):**
```json
{ "name": "string", "email": "string", "password": "string", "role": 1 }
```

**Response 200 (`UserResponseDto`):**
```json
{ "id": "uuid", "name": "string", "email": "string", "role": 1, "createdAt": "datetime" }
```

### `GET /api/Users` — autenticado

| Query | Tipo | Descrição |
|-------|------|-----------|
| `Name` | string | filtro por nome |
| `Email` | string | filtro por e-mail |
| `Role` | int | filtro por tipo (UserType) |
| `PageNumber` | int | página (default 1) |
| `PageSize` | int | itens por página (default 10) |

**Response 200:** `UserResponseDto[]`

### `GET /api/Users/{id}` — autenticado

**Response 200:** `UserResponseDto`

### `PUT /api/Users/{id}/name` — autenticado

**Body:** `{ "name": "string" }`

### `PUT /api/Users/{id}/email` — autenticado

**Body:** `{ "email": "string" }`

### `DELETE /api/Users/{id}` — autenticado

**LGPD:** anonimiza `name`, `email` e `passwordHash` preservando integridade referencial.

---

## Produtos

### `GET /api/Products` — público (paginado)

| Query | Tipo | Descrição |
|-------|------|-----------|
| `Title` | string | busca por título |
| `MinPrice` | double | preço mínimo |
| `MaxPrice` | double | preço máximo |
| `Status` | int | `ProductStatus` |
| `SellerId` | uuid | filtro por vendedor |
| `Category` | int | `DisabilityCategory` |
| `PageNumber` | int | |
| `PageSize` | int | |

**Response 200 (`PaginatedResultOfProductResponseDto`):**
```json
{
  "items": [ { "id", "title", "price", "status", "sellerId", "description", "technicalSpec", "photos" } ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 12,
  "totalPages": 4
}
```

### `POST /api/Products` — Seller (`ProductCreateDto`)

```json
{
  "title": "Cadeira de rodas dobrável",
  "price": 1450.00,
  "sellerId": "uuid",
  "description": "Ótimo estado, pouco uso.",
  "technicalSpec": {
    "category": 0,
    "measures": "54x42x90 cm",
    "weightCapacity": 120.0,
    "usageTime": "1-3 anos"
  }
}
```

**Response 200:** `ProductResponseDto`

### `GET /api/Products/{id}` — público

**Response 200:** `ProductResponseDto`

### `PUT /api/Products/{id}` — Seller

**Body:** `ProductCreateDto` (mesmos campos do POST)

### `PATCH /api/Products/{id}/status` — Seller/Admin

**Body:** `0|1|2|3` (enum `ProductStatus` direto)

### `POST /api/Products/{id}/photos` — Seller (via URL)

**Body (`PhotoCreateDto`):**
```json
{ "url": "https://...", "isMain": false }
```

### `POST /api/Products/{id}/photos/upload` — Seller (multipart)

| Query | Tipo |
|-------|------|
| `isMain` | boolean |

**Body:** `multipart/form-data` com campo `file` (binary)

### `DELETE /api/Products/{id}/photos/{photoId}` — Seller

### `DELETE /api/Products/{id}` — Seller/Admin

---

## Pedidos

### `GET /api/Orders` — autenticado (paginado)

| Query | Tipo |
|-------|------|
| `BuyerId` | uuid |
| `ProductId` | uuid |
| `Status` | int (`OrderStatus`) |
| `PageNumber` | int |
| `PageSize` | int |

**Response 200:** `PaginatedResultOfOrderResponseDto`

### `POST /api/Orders` — autenticado (`OrderCreateDto`)

```json
{
  "buyerId": "uuid",
  "productId": "uuid",
  "totalAmount": 1450.00
}
```

**Response 200 (`OrderResponseDto`):**
```json
{ "id": "uuid", "buyerId": "uuid", "productId": "uuid", "totalAmount": 1450.00, "status": 0, "orderDate": "datetime" }
```

### `GET /api/Orders/{id}` — autenticado

**Response 200:** `OrderResponseDto`

### `PATCH /api/Orders/{id}/status` — autenticado

**Body:** inteiro do enum `OrderStatus`

---

## Pagamentos

### `POST /api/Payments/checkout` — autenticado

**Body (`PaymentCheckoutRequestDto`):**
```json
{ "orderId": "uuid" }
```

**Response 200 (`PaymentCheckoutResponseDto`):**
```json
{ "orderId": "uuid", "checkoutUrl": "https://sandbox.mercadopago.com.br/...", "provider": "MercadoPago" }
```

### `POST /api/Payments/webhook` — público (gateway)

**Body (`PaymentWebhookDto`):**
```json
{ "orderId": "uuid", "status": "approved", "externalReference": "string|null" }
```

Status reconhecidos: `approved` / `paid` / `completed` → `Order.Completed` + `Product.Sold`  
`cancelled` / `rejected` → `Order.Cancelled`

---

## Chat (REST)

### `GET /api/ChatSessions` — autenticado (paginado)

| Query | Tipo |
|-------|------|
| `BuyerId` | uuid |
| `SellerId` | uuid |
| `ProductId` | uuid |
| `PageNumber` | int |
| `PageSize` | int |

**Response 200:** `ChatSessionResponseDto[]`

### `POST /api/ChatSessions` — autenticado

**Body (`ChatSessionCreateDto`):**
```json
{ "buyerId": "uuid", "sellerId": "uuid", "productId": "uuid" }
```

**Response 200 (`ChatSessionResponseDto`):**
```json
{ "id": "uuid", "buyerId": "uuid", "sellerId": "uuid", "productId": "uuid", "createdAt": "datetime", "messages": [] }
```

### `GET /api/ChatSessions/{id}` — autenticado

**Response 200:** `ChatSessionResponseDto`

### `POST /api/ChatSessions/{id}/messages` — autenticado (fallback REST)

**Body (`MessageCreateDto`):**
```json
{ "chatSessionId": "uuid", "senderId": "uuid", "content": "string" }
```

**Response 200:** `MessageResponseDto`

### `GET /api/Messages` — autenticado

| Query | Tipo |
|-------|------|
| `sessionId` | uuid |

**Response 200:** `MessageResponseDto[]`

### `PATCH /api/Messages/{id}/status` — autenticado

**Body:** inteiro do enum `MessageStatus` (0 = Sent, 1 = Read)

---

## Chat (Tempo Real — SignalR Hub)

**Endpoint:** `ws://localhost:5144/hubs/chat?access_token=<JWT>`

| Método (Client → Server) | Payload | Descrição |
|--------------------------|---------|-----------|
| `JoinSession` | `{ sessionId: "uuid" }` | Entra no grupo da sessão |
| `LeaveSession` | `{ sessionId: "uuid" }` | Sai do grupo |
| `SendMessage` | `{ sessionId: "uuid", content: "string" }` | Envia; persiste no DB e faz broadcast |

| Evento (Server → Client) | Payload | Descrição |
|--------------------------|---------|-----------|
| `ReceiveMessage` | `{ sender: "string", content: "string", sentAt: "datetime" }` | Mensagem recebida em tempo real |

---

## Feedbacks

### `GET /api/Feedbacks` — autenticado

| Query | Tipo |
|-------|------|
| `FromUserId` | uuid |
| `OrderId` | uuid |
| `MinRating` | int |
| `PageNumber` | int |
| `PageSize` | int |

**Response 200:** `FeedbackResponseDto[]`

### `POST /api/Feedbacks` — autenticado

> ⚠️ Retorna **409 Conflict** se `Order.Status != Completed`.

**Body (`FeedbackCreateDto`):**
```json
{ "rating": 5, "fromUserId": "uuid", "comment": "Excelente produto!", "orderId": "uuid" }
```

**Response 200 (`FeedbackResponseDto`):**
```json
{ "id": "uuid", "rating": 5, "fromUserId": "uuid", "comment": "string", "orderId": "uuid" }
```

### `GET /api/Feedbacks/{id}` — autenticado

---

## Seller

### `GET /api/Seller/metrics` — `[Authorize(Roles="Seller,Admin")]`

**Response 200 (`SellerMetricsDto`):**
```json
{
  "activeListings": 3,
  "totalSales": 7,
  "pendingOrders": 2,
  "revenue": 5200.00
}
```

---

## Schemas completos

### `TechnicalSpecCreateDto`
```json
{
  "category": 0,
  "measures": "string|null",
  "weightCapacity": 120.0,
  "usageTime": "string|null"
}
```

### `TechnicalSpecResponseDto`
```json
{
  "id": "uuid",
  "category": 0,
  "weightCapacity": 120.0,
  "measures": "string|null",
  "usageTime": "string|null"
}
```

### `PhotoCreateDto`
```json
{ "url": "string", "isMain": false }
```

### `PhotoResponseDto`
```json
{ "id": "uuid", "url": "string", "isMain": true }
```

# Spec de Endpoints — M-TAU API

> Documentação complementar ao Scalar (auto-gerado em `/scalar` no ambiente dev).  
> Base URL dev: `http://localhost:5144`

---

## Autenticação

Todos os endpoints protegidos exigem o header:

```
Authorization: Bearer <JWT>
```

O hub SignalR aceita o token via query string: `?access_token=<JWT>`

---

## Auth

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/auth/login` | público | Autentica usuário; retorna JWT e dados do usuário |

**Body login:**
```json
{ "email": "string", "password": "string" }
```
**Response 200:**
```json
{ "token": "string", "expiresAt": "datetime", "user": { "id", "name", "email", "role" } }
```

---

## Usuários

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/users` | público | Cadastra novo usuário (register) |
| GET | `/api/users/{id}` | autenticado | Retorna dados do usuário |
| GET | `/api/users` | autenticado | Lista todos os usuários |
| PUT | `/api/users/{id}/name` | autenticado | Atualiza nome |
| PUT | `/api/users/{id}/email` | autenticado | Atualiza e-mail |
| DELETE | `/api/users/{id}` | autenticado | **LGPD:** anonimiza dados pessoais (soft delete) |

**Body POST /api/users:**
```json
{ "name": "string", "email": "string", "password": "string", "role": "Buyer|Seller|Admin" }
```

---

## Produtos

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/products` | público | Lista produtos paginados com filtros |
| GET | `/api/products/{id}` | público | Retorna produto por ID |
| POST | `/api/products` | Seller | Cria novo anúncio |
| PUT | `/api/products/{id}` | Seller | Atualiza anúncio |
| PATCH | `/api/products/{id}/status` | Seller/Admin | Altera status (Active, Paused, Sold, Removed) |
| POST | `/api/products/{id}/photos` | Seller | Adiciona foto via URL |
| POST | `/api/products/{id}/photos/upload` | Seller | Upload de foto (multipart/form-data) |
| DELETE | `/api/products/{id}/photos/{photoId}` | Seller | Remove foto |
| DELETE | `/api/products/{id}` | Seller/Admin | Remove anúncio (soft delete) |

**Query GET /api/products:**
```
?category=Fisica|Visual|Auditiva|Cognitiva
&search=string
&pageNumber=1
&pageSize=12
```

**Response paginado:**
```json
{
  "items": [...],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 12,
  "totalPages": 4
}
```

---

## Pedidos

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/orders` | autenticado | Lista pedidos do usuário (paginado) |
| GET | `/api/orders/{id}` | autenticado | Retorna pedido por ID |
| POST | `/api/orders` | autenticado | Cria pedido (Buyer) |
| PATCH | `/api/orders/{id}/status` | autenticado | Atualiza status do pedido |

---

## Pagamentos

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/payments/checkout` | autenticado | Gera preferência e retorna URL do sandbox Mercado Pago |
| POST | `/api/payments/webhook` | público (gateway) | Recebe confirmação; atualiza Order → Completed e Product → Sold |

**Body webhook:**
```json
{ "orderId": "guid", "status": "approved|paid|completed|cancelled|rejected" }
```

---

## Chat (REST)

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/chatsessions` | autenticado | Lista sessões do usuário |
| GET | `/api/chatsessions/{id}` | autenticado | Retorna sessão por ID |
| POST | `/api/chatsessions` | autenticado | Cria nova sessão entre Buyer e Seller |
| POST | `/api/chatsessions/{id}/messages` | autenticado | Envia mensagem via REST (fallback) |
| GET | `/api/messages?sessionId={id}` | autenticado | Lista mensagens de uma sessão |
| PATCH | `/api/messages/{id}/status` | autenticado | Atualiza status da mensagem (Read) |

---

## Chat (Tempo Real — SignalR)

**Hub:** `ws://localhost:5144/hubs/chat?access_token=<JWT>`

| Método do Hub | Direção | Payload | Descrição |
|---------------|---------|---------|-----------|
| `JoinSession` | Client → Server | `{ sessionId }` | Entra no grupo da sessão |
| `LeaveSession` | Client → Server | `{ sessionId }` | Sai do grupo |
| `SendMessage` | Client → Server | `{ sessionId, content }` | Envia mensagem; persiste no DB |
| `ReceiveMessage` | Server → Client | `{ sender, content, sentAt }` | Broadcast para todos no grupo |

---

## Feedbacks

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/feedbacks` | autenticado | Lista feedbacks |
| GET | `/api/feedbacks/{id}` | autenticado | Retorna feedback por ID |
| POST | `/api/feedbacks` | autenticado | Cria avaliação (exige Order.Status == Completed; 409 caso contrário) |

**Body POST:**
```json
{ "orderId": "guid", "rating": 1–5, "comment": "string" }
```

---

## Admin

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/admin/metrics` | Admin | Contagens: users, products, orders, chats |
| PATCH | `/api/admin/products/{id}/moderate` | Admin | Moderação: força status do produto |

---

## Seller

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/seller/metrics` | Seller/Admin | Métricas do seller autenticado: anúncios ativos, pedidos pendentes, total vendas |

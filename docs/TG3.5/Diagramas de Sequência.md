# Diagramas de Sequência – M-TAU

> **TG3 – Deadline 5 | Modelagem Parte II**  
> Diagramas de sequência UML descrevendo os fluxos principais do sistema mapeados aos casos de uso da TG1.2.

---

## DS01 – UC01: Cadastro e Autenticação de Usuário (RF01)

Fluxo de registro de novo usuário seguido de login com emissão de JWT.

```mermaid
sequenceDiagram
    actor Usuario as Usuário
    participant FE as M-TAU.Client<br/>(Blazor)
    participant API as M-TAU.API<br/>(AuthController)
    participant SVC as UserService
    participant REPO as IUserRepository
    participant DB as Azure SQL DB

    %% ── REGISTRO ──────────────────────────────────────────────────────
    Usuario->>FE: Preenche formulário de cadastro
    FE->>API: POST /api/auth/register<br/>{ name, email, password }
    API->>API: Valida RegisterDto (FluentValidation)
    API->>SVC: RegisterAsync(registerDto)
    SVC->>REPO: GetByEmailAsync(email)
    REPO->>DB: SELECT * FROM Users WHERE Email = ?
    DB-->>REPO: null (não encontrado)
    REPO-->>SVC: null
    SVC->>SVC: BCrypt.HashPassword(password)
    SVC->>SVC: new User(id, name, email, hash, BUYER)
    SVC->>REPO: AddAsync(user)
    REPO->>DB: INSERT INTO Users
    DB-->>REPO: OK
    SVC-->>API: UserResponseDto
    API-->>FE: 201 Created { id, name, email }
    FE-->>Usuario: Redireciona para tela de login

    %% ── LOGIN ─────────────────────────────────────────────────────────
    Usuario->>FE: Insere e-mail e senha
    FE->>API: POST /api/auth/login<br/>{ email, password }
    API->>SVC: AuthenticateAsync(loginDto)
    SVC->>REPO: GetByEmailAsync(email)
    REPO->>DB: SELECT * FROM Users WHERE Email = ?
    DB-->>REPO: User
    REPO-->>SVC: User
    SVC->>SVC: BCrypt.Verify(password, hash)
    Note over SVC: Senha válida
    SVC->>SVC: GenerateJWT(user.Id, user.Role)
    SVC-->>API: TokenResponseDto { token, expiresAt }
    API-->>FE: 200 OK { token, user }
    FE->>FE: Armazena JWT no localStorage
    FE-->>Usuario: Redireciona para Home
```

---

## DS02 – UC02: Anunciar Tecnologia Assistiva (RF02 + RF04)

Fluxo completo de criação de anúncio por um vendedor autenticado.

```mermaid
sequenceDiagram
    actor Vendedor
    participant FE as M-TAU.Client<br/>(Blazor)
    participant API as M-TAU.API<br/>(ProductsController)
    participant SVC as ProductService
    participant REPO as IProductRepository
    participant DB as Azure SQL DB

    Vendedor->>FE: Acessa "Novo Anúncio" e preenche formulário
    Note over FE: Título, Descrição, Preço,<br/>Categoria, Specs Técnicas, Fotos

    FE->>API: POST /api/products<br/>Authorization: Bearer {JWT}<br/>{ title, price, category, technicalSpec, photos }
    API->>API: Extrai SellerId do JWT
    API->>API: Valida ProductCreateDto (FluentValidation)

    API->>SVC: CreateAsync(productCreateDto)
    SVC->>SVC: new Product(id, title, price, sellerId)
    SVC->>SVC: product.SetDescription(description)
    SVC->>SVC: new TechnicalSpec(id, category)
    SVC->>SVC: product.SetTechnicalSpec(spec)
    loop Para cada foto
        SVC->>SVC: new Photo(id, url, isMain)
        SVC->>SVC: product.AddPhoto(photo)
    end
    SVC->>REPO: AddAsync(product)
    REPO->>DB: INSERT INTO Products<br/>INSERT INTO TechnicalSpecs<br/>INSERT INTO Photos
    DB-->>REPO: OK
    SVC-->>API: ProductResponseDto
    API-->>FE: 201 Created { id, title, price, status: "Active" }
    FE-->>Vendedor: Exibe confirmação e redireciona para inventário
```

---

## DS03 – UC03: Buscar Produtos com Filtros de Deficiência (RF03)

Fluxo de busca pública com filtros especializados.

```mermaid
sequenceDiagram
    actor Comprador
    participant FE as M-TAU.Client<br/>(Blazor)
    participant API as M-TAU.API<br/>(ProductsController)
    participant SVC as ProductService
    participant REPO as IProductRepository
    participant DB as Azure SQL DB

    Comprador->>FE: Seleciona categoria "Visual" e digita "bengala"
    FE->>API: GET /api/products?category=VISUAL&search=bengala
    API->>SVC: GetAllAsync(ProductFilterDto { category=VISUAL, search="bengala" })
    SVC->>REPO: ListByCategoryAsync(VISUAL)
    REPO->>DB: SELECT p.* FROM Products p<br/>INNER JOIN TechnicalSpecs t ON t.ProductId = p.Id<br/>WHERE t.Category = 'VISUAL'<br/>AND p.Status = 'Active'<br/>AND p.Title LIKE '%bengala%'
    DB-->>REPO: List~Product~
    REPO-->>SVC: IReadOnlyCollection~Product~
    SVC->>SVC: Mapeia para List~ProductResponseDto~ (AutoMapper)
    SVC-->>API: IReadOnlyCollection~ProductResponseDto~
    API-->>FE: 200 OK [ { id, title, price, category, photos }, ... ]
    FE-->>Comprador: Exibe grid de produtos filtrados
```

---

## DS04 – UC04: Realizar Checkout e Pagamento (RF06)

Fluxo de criação de pedido com integração ao gateway de pagamento externo.

```mermaid
sequenceDiagram
    actor Comprador
    participant FE as M-TAU.Client<br/>(Blazor)
    participant API as M-TAU.API<br/>(OrdersController)
    participant OSVC as OrderService
    participant PSVC as ProductService
    participant PREPO as IProductRepository
    participant OREPO as IOrderRepository
    participant PayGW as Gateway de Pagamento<br/>(Stripe / Mercado Pago)
    participant DB as Azure SQL DB

    Comprador->>FE: Clica em "Comprar" na página do produto
    FE->>API: POST /api/orders<br/>Authorization: Bearer {JWT}<br/>{ productId }
    API->>API: Extrai BuyerId do JWT
    API->>OSVC: CreateAsync(OrderCreateDto { buyerId, productId })

    OSVC->>PREPO: GetByIdAsync(productId)
    PREPO->>DB: SELECT * FROM Products WHERE Id = ?
    DB-->>PREPO: Product (Status=Active)
    PREPO-->>OSVC: Product

    OSVC->>OSVC: Valida product.Status == Active
    OSVC->>OSVC: new Order(id, buyerId, productId, product.Price)
    OSVC->>OREPO: AddAsync(order)
    OREPO->>DB: INSERT INTO Orders (Status=Pending)
    DB-->>OREPO: OK

    OSVC->>PayGW: POST /charges<br/>{ amount, currency, description, buyerToken }
    PayGW-->>OSVC: { chargeId, status: "approved" }

    alt Pagamento aprovado
        OSVC->>OREPO: UpdateAsync(order → Status=Confirmed)
        OREPO->>DB: UPDATE Orders SET Status='Confirmed'
        OSVC->>PREPO: UpdateAsync(product → Status=Sold)
        PREPO->>DB: UPDATE Products SET Status='Sold'
        OSVC-->>API: OrderResponseDto { id, status: "Confirmed" }
        API-->>FE: 201 Created { orderId, status: "Confirmed" }
        FE-->>Comprador: Exibe tela de confirmação de pedido
    else Pagamento recusado
        OSVC->>OREPO: UpdateAsync(order → Status=Cancelled)
        OREPO->>DB: UPDATE Orders SET Status='Cancelled'
        OSVC-->>API: Lança PaymentFailedException
        API-->>FE: 422 Unprocessable Entity { error: "Pagamento recusado" }
        FE-->>Comprador: Exibe mensagem de erro de pagamento
    end
```

---

## DS05 – UC05: Interagir via Chat em Tempo Real (RF05)

Fluxo de criação de sessão de chat e troca de mensagens via SignalR.

```mermaid
sequenceDiagram
    actor Comprador
    actor Vendedor
    participant FE_B as M-TAU.Client<br/>(Comprador)
    participant FE_S as M-TAU.Client<br/>(Vendedor)
    participant API as M-TAU.API<br/>(ChatSessionsController)
    participant Hub as ChatHub<br/>(SignalR)
    participant CSVC as ChatSessionService
    participant MSVC as MessageService
    participant DB as Azure SQL DB

    Comprador->>FE_B: Clica em "Conversar com Vendedor"
    FE_B->>API: POST /api/chat-sessions<br/>Authorization: Bearer {JWT}<br/>{ sellerId, productId }
    API->>CSVC: CreateAsync(ChatSessionCreateDto)
    CSVC->>DB: INSERT INTO ChatSessions (BuyerId, SellerId, ProductId)
    DB-->>CSVC: ChatSession
    CSVC-->>API: ChatSessionResponseDto { sessionId }
    API-->>FE_B: 201 Created { sessionId }

    FE_B->>Hub: Connect (WebSocket)<br/>Joins group: session_{sessionId}
    FE_S->>Hub: Connect (WebSocket)<br/>Joins group: session_{sessionId}

    Comprador->>FE_B: Digita e envia mensagem "Olá, ainda disponível?"
    FE_B->>Hub: SendMessage({ sessionId, content: "Olá..." })
    Hub->>MSVC: SendAsync(MessageCreateDto { sessionId, senderId, content })
    MSVC->>DB: INSERT INTO Messages (Status=Sent)
    DB-->>MSVC: Message
    MSVC-->>Hub: MessageResponseDto
    Hub-->>FE_B: ReceiveMessage({ sender: "Comprador", content, sentAt })
    Hub-->>FE_S: ReceiveMessage({ sender: "Comprador", content, sentAt })
    FE_S-->>Vendedor: Exibe mensagem recebida em tempo real

    Vendedor->>FE_S: Responde "Sim, está disponível!"
    FE_S->>Hub: SendMessage({ sessionId, content: "Sim..." })
    Hub->>MSVC: SendAsync(MessageCreateDto)
    MSVC->>DB: INSERT INTO Messages
    Hub-->>FE_B: ReceiveMessage({ sender: "Vendedor", content, sentAt })
    Hub-->>FE_S: ReceiveMessage({ sender: "Vendedor", content, sentAt })
    FE_B-->>Comprador: Exibe resposta do vendedor
```

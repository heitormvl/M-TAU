# Diagrama de Classes Refinado – M-TAU

> **TG3 – Deadline 5 | Modelagem Parte II**  
> Refina o diagrama inicial entregue na TG1.2, incorporando métodos de domínio, enumerações, interfaces de repositório e de serviço derivadas da implementação real do código.

---

```mermaid
classDiagram
    direction LR

    %% ── IDENTITY MODULE ─────────────────────────────────────────────────
    class User {
        +Guid Id
        +string Name
        +string Email
        +string PasswordHash
        +string Role
        +DateTime CreatedAt
        +SetName(string name) void
        +SetEmail(string email) void
    }

    %% ── CATALOG MODULE ─────────────────────────────────────────────────
    class Product {
        +Guid Id
        +string Title
        +string? Description
        +decimal Price
        +string Status
        +Guid SellerId
        +TechnicalSpec? TechnicalSpec
        +IReadOnlyCollection~Photo~ Photos
        +SetTitle(string title) void
        +SetDescription(string? desc) void
        +SetPrice(decimal price) void
        +UpdateStatus(string status) void
        +AddPhoto(Photo photo) void
        +SetTechnicalSpec(TechnicalSpec spec) void
    }

    class TechnicalSpec {
        +Guid Id
        +string Category
        +string? Measures
        +double WeightCapacity
        +string? UsageTime
        +SetMeasures(string? m) void
        +SetWeightCapacity(double w) void
        +SetUsageTime(string? t) void
    }

    class Photo {
        +Guid Id
        +string Url
        +bool IsMain
    }



    %% ── TRANSACTION MODULE ──────────────────────────────────────────────
    class Order {
        +Guid Id
        +Guid BuyerId
        +Guid ProductId
        +DateTime OrderDate
        +string Status
        +decimal TotalAmount
        +SetTotalAmount(decimal amount) void
        +UpdateStatus(string status) void
    }

    class Feedback {
        +Guid Id
        +int Rating
        +string? Comment
        +Guid FromUserId
        +Guid? OrderId
        +SetRating(int rating) void
        +SetComment(string? comment) void
    }



    %% ── CHAT MODULE ─────────────────────────────────────────────────────
    class ChatSession {
        +Guid Id
        +Guid BuyerId
        +Guid SellerId
        +Guid ProductId
        +DateTime CreatedAt
        +IReadOnlyCollection~Message~ Messages
        +AddMessage(Message message) void
    }

    class Message {
        +Guid Id
        +Guid ChatSessionId
        +Guid SenderId
        +string Content
        +DateTime SentAt
        +string Status
        +SetContent(string content) void
        +UpdateStatus(string status) void
    }



    %% ── ASSOCIAÇÕES DE DOMÍNIO ─────────────────────────────────────────
    User "1" --> "0..*" Product : vende (SellerId)
    User "1" --> "0..*" Order : realiza (BuyerId)
    User "1" --> "0..*" Feedback : emite (FromUserId)
    Product "1" *-- "0..1" TechnicalSpec : contém
    Product "1" *-- "1..*" Photo : possui
    Order "1" --> "0..1" Feedback : gera
    Order "1" --> "1" Product : refere-se a
    ChatSession "1" *-- "0..*" Message : contém
    ChatSession "1" --> "1" Product : sobre
```

---

## Notas de Refinamento

| Aspecto | TG1.2 (inicial) | TG3.5 (refinado) |
|---|---|---|
| **Escopo** | Completo com abstracts e enums | Apenas entidades concretas de domínio |
| **Métodos de domínio** | Ausentes | Incluídos em todas as entidades (`SetTitle`, `UpdateStatus`, etc.) |
| **Tipos de dados** | Enums como classes | Simplificados como strings (Category, Status, Role) |
| **Herança** | Todas herdam de EntityBase | Removida; `Id` adicionado explicitamente em cada classe |
| **Relacionamentos** | Associações simples | Composição (`*--`) onde aplicável (Photo/TechnicalSpec pertencem ao Product) |
| **Multiplicidade** | Parcial | Explícita em todas as associações |

# Diagramas de Atividade – M-TAU

> **TG3 – Deadline 5 | Modelagem Parte II**  
> Diagramas de atividade UML descrevendo o fluxo de controle e decisões dos processos de negócio críticos do sistema.

---

## DA01 – Fluxo de Checkout e Pagamento (UC04 – RF06)

Descreve o processo completo desde a intenção de compra até a confirmação ou falha do pagamento, incluindo as guards de validação de produto e retorno do gateway externo.

```mermaid
flowchart TD
    Start(["●Início"])

    A["Comprador clica em 'Comprar' na página do produto"]
    B{{"Usuário autenticado?"}}
    C["Redireciona para Login / Registro"]
    D["API extrai BuyerId do JWT"]
    E["Busca produto pelo ProductId"]
    F{{"Produto com Status = Active?"}}
    G["Retorna erro 409: 'Produto indisponível'"]
    H["Cria Order (Status = Pending)"]
    I["Persiste Order no banco"]
    J["Envia requisição ao Gateway de Pagamento (Stripe / Mercado Pago)"]
    K{{"Pagamento aprovado?"}}
    L["Atualiza Order → Status = Confirmed"]
    M["Atualiza Product → Status = Sold"]
    N["Retorna 201 Created OrderResponseDto"]
    O["Exibe tela de Confirmação ao Comprador"]
    P["Atualiza Order → Status = Cancelled"]
    Q["Retorna 422 Unprocessable 'Pagamento recusado'"]
    R["Exibe mensagem de erro ao Comprador"]
    End1(["◉Fim"])
    End2(["◉Fim"])
    End3(["◉Fim"])

    Start --> A
    A --> B
    B -- Não --> C
    C --> End3
    B -- Sim --> D
    D --> E
    E --> F
    F -- Não --> G
    G --> End3
    F -- Sim --> H
    H --> I
    I --> J
    J --> K
    K -- Sim --> L
    L --> M
    M --> N
    N --> O
    O --> End1
    K -- Não --> P
    P --> Q
    Q --> R
    R --> End2
```

---

## DA02 – Fluxo de Publicação de Anúncio (UC02 – RF02 + RF09)

Descreve as atividades de um vendedor ao criar e gerenciar um anúncio de tecnologia assistiva.

```mermaid
flowchart TD
    Start(["●Início"])

    A["Vendedor acessa 'Novo Anúncio'"]
    B{{"Token JWT válido com Role=SELLER?"}}
    C["Retorna 401 Unauthorized"]
    D["Exibe formulário de criação de anúncio"]
    E["Preenche: Título, Preço, Descrição, Categoria de Deficiência, Especificações Técnicas"]
    F["Faz upload das fotos"]
    G{{"Formulário válido?"}}
    H["Exibe erros de validação ao vendedor"]
    I["Cria entidade Product com Status = Active"]
    J["Cria entidade TechnicalSpec vínculada ao produto"]
    K["Cria entidades Photo vínculadas ao produto"]
    L["Persiste Product + TechnicalSpec + Photos no banco"]
    M["Retorna 201 Created ProductResponseDto"]
    N["Exibe confirmação 'Anúncio publicado!'"]

    O["Vendedor acessa Gerenciar Inventário (UC07)"]
    P{{"Ação desejada?"}}
    Q["Atualiza campos (título, preço, descrição)"]
    R["Atualiza Product.Status → Paused"]
    S["Produto some da busca pública"]
    T["Soft Delete: Product.Status = Deleted"]
    U["Produto removido da listagem pública"]
    V["Persiste alterações"]

    End1(["◉Fim"])
    End2(["◉Fim"])

    Start --> A
    A --> B
    B -- Não --> C
    C --> End2
    B -- Sim --> D
    D --> E
    E --> F
    F --> G
    G -- Inválido --> H
    H --> E
    G -- Válido --> I
    I --> J
    J --> K
    K --> L
    L --> M
    M --> N
    N --> O
    O --> P
    P -- Editar --> Q
    P -- Pausar --> R
    R --> S
    P -- Excluir --> T
    T --> U
    Q --> V
    S --> V
    U --> V
    V --> End1
```

---

## DA03 – Fluxo de Avaliação Pós-Negociação (UC06 – RF07)

Descreve o processo de submissão de avaliação, que só é liberado após conclusão do pedido.

```mermaid
flowchart TD
    Start(["●Início"])

    A["Comprador acessa seus Pedidos"]
    B["Busca Order por BuyerId"]
    C{{"Order.Status = Completed?"}}
    D["Exibe botão 'Avaliar negociação'"]
    E["Botão desabilitado (pedido em andamento)"]
    F["Comprador clica em 'Avaliar negociação'"]
    G["Exibe formulário: Nota (1-5 ★) + Comentário"]
    H["Submete avaliação"]
    I{{"Nota entre 1 e 5?"}}
    J["Exibe erro: 'Nota deve ser entre 1 e 5'"]
    K["Cria entidade Feedback (Rating, Comment, FromUserId, OrderId)"]
    L["Persiste Feedback no banco"]
    M["Atualiza média de avaliação do vendedor no perfil"]
    N["Retorna 201 Created FeedbackResponseDto"]
    O["Exibe 'Avaliação enviada!'"]

    End1(["◉Fim"])
    End2(["◉Fim"])

    Start --> A
    A --> B
    B --> C
    C -- Não --> E
    E --> End2
    C -- Sim --> D
    D --> F
    F --> G
    G --> H
    H --> I
    I -- Inválida --> J
    J --> G
    I -- Válida --> K
    K --> L
    L --> M
    M --> N
    N --> O
    O --> End1
```

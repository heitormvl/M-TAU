# Roteiro de Vídeo — M-TAU (YouTube)

> Duração estimada: **8–12 minutos**  
> Formato: screencast com narração. Mostrar o app funcionando de ponta a ponta.

---

## Abertura (0:00 – 0:45)

**[Tela: slide inicial ou logo do M-TAU]**

> "Olá! Nesse vídeo eu vou apresentar o M-TAU — Marketplace de Tecnologias Assistivas Usadas.
>
> A ideia é simples: conectar pessoas que têm equipamentos de tecnologia assistiva parados em casa — cadeira de rodas, bengala, prótese auditiva — com quem realmente precisa desses produtos.
>
> O app foi desenvolvido como MVP final da disciplina de Laboratório de Engenharia de Software da Universidade Presbiteriana Mackenzie, em 2026.
>
> A stack é ASP.NET Core 10 no backend, Blazor WebAssembly no frontend, chat em tempo real com SignalR e integração com o Mercado Pago. Tudo containerizado com Docker e com pipeline de CI/CD no GitHub Actions.
>
> Vamos ver tudo funcionando."

---

## 1. Arquitetura em 30 segundos (0:45 – 1:15)

**[Tela: diagrama de componentes ou README no GitHub]**

> "A aplicação tem 5 projetos .NET:
> - **Domain** — entidades e interfaces
> - **Application** — serviços e casos de uso
> - **Infrastructure** — EF Core e repositórios
> - **API** — controllers REST e hub SignalR
> - **Client** — Blazor WASM com MudBlazor
>
> Em dev roda com SQLite. Em produção, Azure SQL.
> Dois containers Docker: um para a API, um para o Blazor servido pelo nginx."

---

## 2. Registro e Login — DS01 (1:15 – 2:30)

**[Tela: browser na rota /register]**

> "Primeiro, vou cadastrar um Vendedor."

- Preencher nome, e-mail `seller@m-tau.test`, senha, role **Seller** → clicar Cadastrar
- Mostrar snackbar de sucesso e redirect para `/login`
- Fazer login → abrir DevTools → Application → localStorage
- Destacar a chave `authToken` com o JWT

> "O JWT fica armazenado no localStorage. Ele carrega o ID do usuário e a role — usamos isso nos controllers para proteger rotas e filtrar dados por vendedor."

- Abrir aba anônima → repetir para `buyer@m-tau.test`, role **Buyer**

---

## 3. Criação de Anúncio — DS02 (2:30 – 4:30)

**[Tela: logado como Seller, /seller/dashboard]**

> "No painel do Seller, vejo minhas métricas — anúncios ativos, pedidos pendentes e total de vendas. Tudo zerado por enquanto."

- Clicar **Novo Anúncio** → `/seller/products/new`
- Preencher:
  - Título: `Cadeira de rodas dobrável`
  - Preço: `1450`
  - Categoria: `Física`
  - Especificações: medidas, capacidade, `1–3 anos de uso`
  - Upload de 2 fotos `.jpg`
- Clicar **Publicar**

> "Snackbar de sucesso. Redireciona para o inventário."

**[Tela: /seller/products]**

> "O produto aparece com status **Ativo**."

**[Voltar ao dashboard]**

> "E agora o dashboard já mostra 1 anúncio ativo."

---

## 4. Busca Filtrada — DS03 (4:30 – 5:30)

**[Tela: logado como Buyer, /catalog]**

> "Agora como Comprador, acesso o catálogo."

- Aplicar filtro **Categoria: Física**
- Aplicar faixa de preço **1000–2000**

> "O produto que o Seller acabou de criar aparece aqui. Outros produtos de outras categorias não aparecem — os filtros funcionam de forma combinada, como especificado no RF03."

---

## 5. Chat em Tempo Real — DS04 (5:30 – 7:00)

**[Tela: ProductDetail.razor]**

> "Clico no produto e vejo os detalhes. Tem o botão **Contatar vendedor**."

- Clicar → sistema cria a `ChatSession` e redireciona para `/buyer/chat/{id}`

> "A sessão de chat foi criada. O Comprador já está conectado via WebSocket, usando SignalR."

**[Abrir segunda aba como Seller → /seller/chat]**

> "Em outra aba, o Vendedor entra na mesma sessão."

- Buyer digita: `"Olá! A cadeira ainda está disponível?"`
- Seller responde: `"Sim, perfeita condição!"`

> "As mensagens aparecem em tempo real, sem refresh. O SignalR mantém a conexão WebSocket aberta e faz broadcast para todos no grupo da sessão. As mensagens são persistidas no banco."

---

## 6. Checkout e Pagamento — DS05 (7:00 – 9:00)

**[Tela: ProductDetail.razor como Buyer]**

> "Agora o Comprador decide comprar."

- Clicar **Comprar** → `/checkout/{productId}`
- Digitar CEP `01310-100`

> "O campo de Cidade e Bairro preenche automaticamente via ViaCEP. Não calculamos frete — isso está fora do escopo — mas a origem fica registrada."

- Preencher logradouro e número
- Clicar **Ir para pagamento** → redirect para sandbox Mercado Pago

> "Em produção o usuário pagaria aqui. Para a demo, simulo o webhook de aprovação."

**[Tela: terminal]**

```bash
curl -X POST http://localhost:5144/api/payments/webhook \
  -H "Content-Type: application/json" \
  -d '{"orderId":"<ID>","status":"approved"}'
```

> "O webhook chega na API, que atualiza o status do pedido para `Completed` e do produto para `Sold`."

**[Tela: /buyer/orders]**

> "O pedido aparece como **Concluído**."

**[Tela: /seller/products]**

> "E o produto do Seller está marcado como **Vendido**."

**[Tela: /buyer/feedback/{orderId}]**

> "Agora o formulário de avaliação está liberado — só aparece quando o pedido está concluído, como exige o RF07. Envio 5 estrelas e um comentário."

---

## 7. Painel Admin (9:00 – 9:45)

**[Tela: /admin — logado como Admin]**

> "O Admin tem acesso a contagens reais carregadas da API: total de usuários, produtos, pedidos e sessões de chat."

**[Tela: /admin/products]**

> "E pode moderar qualquer anúncio — pausar, aprovar ou remover."

---

## 8. Infraestrutura (9:45 – 11:00)

**[Tela: terminal]**

```bash
docker compose up --build
docker compose ps
```

> "Com um único comando, os dois containers sobem: API na porta 5144, cliente na 5213. Dados e uploads persistidos em volumes Docker."

**[Tela: GitHub → Actions]**

> "Cada push para `main` dispara o pipeline de CI: build + 5 testes de integração. Aqui o último run verde."

```bash
dotnet test M-TAU.Tests
```

> "E aqui localmente: 5 testes, todos passando em ~21 segundos. Eles cobrem ponta a ponta os 5 diagramas de sequência — do cadastro ao feedback."

---

## Encerramento (11:00 – 12:00)

**[Tela: README ou slide de encerramento]**

> "O M-TAU entrega um marketplace funcional com:
> - Autenticação JWT com múltiplas roles
> - Catálogo paginado com filtros de acessibilidade
> - Chat em tempo real via SignalR
> - Integração com gateway de pagamento e webhook
> - Conformidade LGPD e WCAG
> - Tudo containerizado e com CI/CD
>
> O código está aberto no GitHub. Se quiser rodar localmente, é só `docker compose up --build`.
>
> Valeu!"

---

## Checklist de gravação

- [ ] Limpar `localStorage` antes de começar (`localStorage.clear()` no DevTools)
- [ ] Ter 3 abas abertas: Seller (anônima 1), Buyer (anônima 2), Admin (aba normal)
- [ ] API e Client rodando (`dotnet run` ou `docker compose up`)
- [ ] Preparar o `curl` do webhook com o `orderId` real (copiar após criar o pedido)
- [ ] Microfone testado; resolução mínima 1080p
- [ ] Gravação sem notificações do sistema operacional

# Roteiro de Demo — M-TAU (1–2 jun 2026)

> Versão narrada do passo-a-passo da apresentação. Cada item mapeia para um diagrama de sequência.

## Setup (5 min antes)

- `docker compose up --build -d` (ou `dotnet run` em dois terminais para API + Client).
- Pré-criar 2 abas anônimas para Buyer e Seller; 1 aba para Admin.
- Limpar localStorage entre execuções: `localStorage.clear()`.

## DS01 — Registro & Autenticação (2 min)

1. `/register` → cadastrar Seller (`seller@m-tau.test`, role Seller).
2. `/login` → fazer login; mostrar o JWT em `localStorage.authToken` no DevTools.
3. Repetir cadastro/login para Buyer (`buyer@m-tau.test`, role Buyer).

## DS02 — Criação de anúncio (3 min)

1. Como Seller, ir em `/seller/dashboard` → métricas zeradas.
2. Clicar "Novo anúncio" e preencher:
   - Título: "Cadeira de rodas dobrável"
   - Preço: 1450
   - Categoria: Física
   - Especificação: medidas, capacidade, tempo de uso
   - Upload de 2 fotos (.jpg)
3. Publicar → snackbar de sucesso; redireciona para `/seller/products`.
4. Dashboard agora mostra "1 Anúncio ativo".

## DS03 — Busca filtrada (2 min)

1. Como Buyer, ir em `/catalog`.
2. Aplicar filtro de categoria "Física" + faixa de preço 1000–2000.
3. Produto criado aparece no grid; paginação mostra 1 página.

## DS04 — Chat em tempo real (3 min)

1. Como Buyer, abrir o produto → clicar "Contatar vendedor".
2. Confirma criação da `ChatSession` e redireciona para `/buyer/chat/{id}`.
3. Em outra aba como Seller, abrir `/seller/chat` e entrar na mesma sessão.
4. Trocar mensagens; observar que aparecem em tempo real sem F5 (SignalR).

## DS05 — Checkout & Pagamento (4 min)

1. Como Buyer, na tela do produto, clicar "Comprar".
2. `/checkout/{productId}`:
   - Digitar um CEP válido (ex.: `01310-100`) → cidade e bairro preenchem automaticamente.
   - Logradouro/Número.
3. Clicar "Ir para pagamento" → redirect para sandbox do Mercado Pago.
4. Simular webhook do gateway:

```bash
curl -X POST http://localhost:5144/api/payments/webhook \
  -H "Content-Type: application/json" \
  -d '{"orderId":"<ID-DO-PEDIDO>","status":"approved"}'
```

5. Em `/buyer/orders` o pedido vira **Completed**; em `/seller/products` o produto vira **Sold**.
6. Como Buyer, em `/buyer/feedback/{orderId}`, enviar avaliação (estrelas + comentário). Tentativa com pedido não-Completed retorna 409.

## Admin (1 min)

1. Login com Admin → `/admin` mostra contagens reais (users/products/orders/chats) carregadas de `GET /api/admin/metrics`.
2. `/admin/products` → moderar produto (pausar/aprovar/remover).

## Entrega (1 min)

- Mostrar `docker compose ps` rodando.
- Abrir GitHub → Actions → último pipeline verde.
- `dotnet test` no terminal → 5/5 passando.

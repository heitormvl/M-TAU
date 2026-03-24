# Declaração de Escopo: Projeto M-TAU

## 1. Descrição do Escopo do Produto

O **M-TAU (Marketplace de Tecnologias Assistivas Usadas)** será uma plataforma web robusta desenvolvida em **.NET 10 Core**, projetada para conectar vendedores de tecnologias assistivas (cadeiras de rodas, próteses, periféricos adaptados, etc.) a compradores que buscam equipamentos com custo reduzido. O sistema focará em uma experiência de usuário (UX) acessível e filtros técnicos especializados que não são encontrados em marketplaces genéricos.

## 2. Entregáveis do Projeto (Deliverables)

Para cumprir as metas acadêmicas da disciplina, o projeto entregará:

* **Documentação Técnica:** TAP, Cronograma, EAP, Requisitos (RF/RNF), Diagramas UML (Classe, Sequência, Caso de Uso) e Modelo Entidade-Relacionamento (MER).
* **Protótipo de Alta Fidelidade:** Wireframes detalhando a interface acessível.
* **Infraestrutura DevOps:** Repositório Git, Pipeline de CI/CD configurado e ambiente de staging na Azure.
* **Software Funcional (MVP):** Aplicação web com frontend e backend integrados, banco de dados SQL Server e consumo de APIs externas.

## 3. Requisitos e Funções Principais

O sistema deve, obrigatoriamente, realizar:

* **Gestão de Usuários:** Cadastro e autenticação de compradores e vendedores.
* **Catálogo Assistivo:** Cadastro de produtos com especificações técnicas detalhadas (ex: medidas, peso suportado, tempo de uso).
* **Busca Inteligente:** Filtros por categorias de deficiência (Física, Visual, Auditiva, Cognitiva).
* **Módulo de Negociação:** Chat em tempo real para comunicação direta entre as partes.
* **Processamento de Pagamento:** Integração com Gateway de Pagamento (ex: Stripe ou Mercado Pago) para garantir a transação financeira.
* **Feedback do Ecossistema:** Sistema de avaliações e notas para ambos os perfis (comprador/vendedor).

## 4. Critérios de Aceitação

O projeto será considerado bem-sucedido e aceito para avaliação final se:

1. **Funcionalidade:** Todos os requisitos listados no item 3 estiverem operacionais.
2. **Qualidade Técnica:** O código seguir princípios de Clean Code e SOLID, utilizando Entity Framework para persistência.
3. **Disponibilidade:** A aplicação estiver publicada na nuvem (Azure) e acessível via URL.
4. **Acessibilidade:** A interface atender a, no mínimo, critérios básicos de acessibilidade digital (contraste, navegação via teclado, suporte a leitores de tela).
5. **Documentação:** Todos os artefatos solicitados nas diretrizes estiverem atualizados no Moodle/OneDrive.

## 5. Restrições e Limites (Fora do Escopo)

Para garantir a viabilidade da entrega dentro do semestre letivo, os seguintes itens **não** fazem parte do escopo:

* **Logística Própria:** O sistema não gerenciará a entrega física, frete ou retirada (responsabilidade dos usuários).
* **Suporte Pós-Venda:** Reclamações, devoluções ou disputas judiciais não serão geridas pela plataforma.
* **Validação de Identidade:** Não haverá integração com bureaus de crédito ou verificação de documentos oficiais (RG/CPF) no cadastro.
* **Gestão Financeira Complexa:** Não haverá carteira digital (wallet) interna ou retenção de valores (custody) pelo sistema, apenas o checkout via gateway.

## 6. Riscos de Escopo

* **Integração de Terceiros:** Atrasos na homologação da conta de teste do Gateway de Pagamento.
* **Escalabilidade:** Necessidade de refatoração caso a modelagem de dados dos filtros técnicos se torne excessivamente complexa.
# Termo de Encerramento de Projeto — M-TAU

**Projeto:** M-TAU — Marketplace de Tecnologias Assistivas Usadas  
**Gerente:** Heitor Maciel de Vasconcellos Leite  
**Disciplina:** Laboratório de Engenharia de Software — 2026.1  
**Instituição:** Universidade Presbiteriana Mackenzie  
**Data de encerramento:** 26/05/2026  

---

## 1. Declaração de Encerramento

O projeto M-TAU é formalmente encerrado na data acima. O MVP foi desenvolvido, testado e entregue dentro do escopo definido no Termo de Abertura do Projeto (TAP), cumprindo todos os marcos do cronograma estabelecidos para a disciplina.

---

## 2. Entregas Realizadas

### TG1 — Planejamento e Requisitos
| Artefato | Arquivo | Status |
|----------|---------|--------|
| Termo de Abertura do Projeto (TAP) | `docs/TG1.1/tap.md` | ✅ Entregue |
| Declaração de Escopo | `docs/TG1.1/Declaração de Escopo.md` | ✅ Entregue |
| EAP / WBS | `docs/TG1.1/EAP.md` | ✅ Entregue |
| Requisitos Funcionais (RF01–RF09) | `docs/TG1.2/Requisitos Funcionais.md` | ✅ Entregue |
| Requisitos Não Funcionais | `docs/TG1.2/Requisitos Nao Funcionais.md` | ✅ Entregue |
| Arquitetura do Sistema | `docs/TG1.2/Arquitetura.md` | ✅ Entregue |

### TG2 — Modelagem e Prototipagem
| Artefato | Arquivo | Status |
|----------|---------|--------|
| Personas | `docs/TG2.3/Personas.md` | ✅ Entregue |
| Pipeline CI/CD | `.github/workflows/ci.yml` | ✅ Configurado |

### TG3 — Implementação Parcial
| Artefato | Arquivo | Status |
|----------|---------|--------|
| Diagrama de Classes Refinado | `docs/TG3.5/Diagrama de Classes Refinado.md` | ✅ Entregue |
| Diagrama de Componentes | `docs/TG3.5/Diagrama de Componentes.md` | ✅ Entregue |
| Diagramas de Atividade | `docs/TG3.5/Diagramas de Atividade.md` | ✅ Entregue |
| Diagramas de Sequência (DS01–DS05) | `docs/TG3.5/Diagramas de Sequência.md` | ✅ Entregue |
| Dockerfiles + docker-compose | `M-TAU.API/Dockerfile`, `M-TAU.Client/Dockerfile`, `docker-compose.yml` | ✅ Entregue |

### TG4 — Aplicação Finalizada e Documentação
| Artefato | Detalhe | Status |
|----------|---------|--------|
| Backend (100%) | 10 controllers, 9 serviços, SignalR Hub | ✅ Concluído |
| Frontend (100%) | Blazor WASM, MudBlazor, 20+ páginas | ✅ Concluído |
| Testes de Integração | 5 testes cobrindo DS01–DS05 — todos passando | ✅ Concluído |
| Spec de Endpoints | `docs/TG4.6/openapi-endpoints.md` + Scalar em `/scalar` | ✅ Concluído |
| Roteiro de Demo | `docs/TG4.6/demo-script.md` | ✅ Concluído |
| Script de Vídeo | `docs/TG4.6/roteiro-video.md` | ✅ Concluído |
| Termo de Encerramento | `docs/TG4.6/termo-de-encerramento.md` | ✅ Este documento |

---

## 3. Verificação de Requisitos Funcionais

| ID | Requisito | Implementado | Testado |
|----|-----------|:---:|:---:|
| RF01 | Gestão de Usuários (cadastro, login, JWT, LGPD) | ✅ | ✅ DS01 |
| RF02 | Catálogo com upload de fotos | ✅ | ✅ DS02 |
| RF03 | Filtros especializados por categoria de deficiência | ✅ | ✅ DS03 |
| RF04 | Especificações técnicas (medidas, capacidade, tempo de uso) | ✅ | ✅ DS02 |
| RF05 | Chat em tempo real via SignalR | ✅ | ✅ DS05 |
| RF06 | Gateway de pagamento (Mercado Pago sandbox + webhook) | ✅ | ✅ DS04 |
| RF07 | Avaliação pós-compra (bloqueada até Order.Completed) | ✅ | ✅ DS04 |
| RF08 | Preenchimento automático de CEP (ViaCEP) | ✅ | ✅ manual |
| RF09 | Gestão de inventário pelo Admin (pausar/aprovar/remover) | ✅ | ✅ manual |

---

## 4. Verificação de Requisitos Não Funcionais

| ID | Requisito | Status |
|----|-----------|--------|
| RNF01 | WCAG — acessibilidade (aria-label, alt, navegação por teclado, contraste AA) | ✅ MudBlazor + atributos aplicados |
| RNF04 | Cloud — deploy configurado para Azure App Service | ✅ `.github/workflows/cd.yml` + `appsettings.Production.json` |
| RNF06 | LGPD — `DELETE /api/users/{id}` anonimiza PII | ✅ `User.Anonymize()` preserva integridade referencial |
| RNF07 | HTTPS — `UseHttpsRedirection` na API; TLS no Azure | ✅ Configurado |

---

## 5. Métricas do Projeto

| Métrica | Valor |
|---------|-------|
| Duração total | 24/02/2026 – 26/05/2026 (91 dias) |
| Commits no `main` | `git rev-list --count main` |
| Cobertura de testes | 5 testes de integração ponta a ponta (DS01–DS05) |
| Tempo de execução dos testes | ~21 segundos |
| Endpoints de API | 30+ rotas REST + 3 métodos SignalR |
| Páginas no frontend | 20+ pages/components Blazor |

---

## 6. Pendências e Trabalhos Futuros

Os itens a seguir estão fora do escopo do MVP aprovado, mas representam evoluções naturais da plataforma:

- **Logística:** cálculo de frete real e integração com transportadoras.
- **Verificação de identidade:** KYC via documentos oficiais.
- **Notificações push:** alertas de mensagem e status de pedido.
- **Avaliação pública:** exibição da nota média no perfil do vendedor.
- **Busca full-text:** ElasticSearch para pesquisa avançada de especificações técnicas.

---

## 7. Lições Aprendidas

1. **SignalR e JWT via query string:** o middleware padrão do ASP.NET Core não lê o token do query string por padrão — exigiu o evento `OnMessageReceived` no `JwtBearerOptions`.
2. **Concorrência no EF Core:** múltiplas requisições simultâneas no hub compartilhando o mesmo `DbContext` causaram erros de concorrência; resolvido isolando o contexto por operação.
3. **Paginação antecipada:** implementar `PaginatedResult<T>` desde o início do projeto teria poupado refatoração nas 6 páginas Razor que consumiam listas planas.

---

## 8. Aprovação

Este termo registra a conclusão formal do projeto M-TAU conforme os requisitos da disciplina de Laboratório de Engenharia de Software — 2026.1.

| Papel | Nome | Data |
|-------|------|------|
| Gerente de Projeto | Heitor Maciel de Vasconcellos Leite | 26/05/2026 |
| Professor Orientador | Prof. Traue | ___________ |

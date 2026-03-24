# Especificação de Requisitos Não Funcionais - M-TAU

### 1. Acessibilidade

| ID        | Nome                             | Descrição                                                                                                           |
| --------- | -------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| **RNF01** | **Compatibilidade com Leitores de Tela** | O sistema deve ser totalmente navegável e compreensível utilizando leitores de tela como **NVDA** e **JAWS**. |

### 2. Desempenho e Escalabilidade

| ID        | Nome                      | Descrição                                                                                                                                  |
| --------- | ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| **RNF02** | **Tempo de Resposta**     | Consultas ao catálogo de produtos devem retornar resultados em menos de **2 segundos** sob condições normais de rede.                      |
| **RNF03** | **Persistência de Dados** | O uso de **Entity Framework Core** deve ser otimizado com consultas asno-rastreáveis (*AsNoTracking*) para listagens, visando performance. |
| **RNF04** | **Disponibilidade**       | A aplicação deve estar hospedada na **Azure** com uma meta de disponibilidade (SLA) de 99.5% durante o período de testes acadêmicos.       |

### 3. Segurança e Privacidade

| ID        | Nome                      | Descrição                                                                                                                    |
| --------- | ------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| **RNF05** | **Criptografia de Dados** | Senhas de usuários devem ser armazenadas utilizando algoritmos de hashing seguros (BCrypt ou Argon2) via ASP.NET Identity.   |
| **RNF06** | **Conformidade com LGPD** | O sistema deve permitir que o usuário solicite a exclusão de seus dados pessoais, conforme a Lei Geral de Proteção de Dados. |
| **RNF07** | **Comunicação Segura**    | Todo o tráfego de dados entre o cliente e o servidor .NET deve ser realizado via protocolo **HTTPS/TLS 1.3**.                |

### 4. Manutenibilidade (Arquitetura)

| ID        | Nome                             | Descrição                                                                                                                                          |
| --------- | -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| **RNF08** | **Arquitetura de Microserviços** | O sistema deve ser desacoplado em serviços independentes (ex: IdentityService, CatalogService, PaymentService) para facilitar o deploy via Docker. |
| **RNF09** | **Padronização de Código**       | O código fonte deve seguir os princípios de **Clean Code** e as convenções de nomenclatura oficiais da Microsoft para C#.                          |
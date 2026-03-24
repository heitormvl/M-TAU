# Documento de Arquitetura e Justificativa Técnica - M-TAU

## 1. Visão Geral da Arquitetura

O projeto M-TAU adota uma **Arquitetura de Microserviços** baseada em **.NET 10 Core**. A escolha visa o desacoplamento de domínios de negócio e a escalabilidade independente de módulos críticos, como o catálogo de tecnologias assistivas e o processamento de pagamentos.

### 1.1. Estilo Arquitetural

* **Padrão:** Microserviços baseados em domínios (Domain-Driven Design - DDD).
* **Comunicação:** Síncrona via REST/HTTP para operações de consulta e Assíncrona via Mensageria (futuro) para integração entre serviços.
* **Persistência:** Banco de dados poliglota, com cada serviço possuindo seu próprio esquema no **Azure SQL Database**.

## 2. Stack Tecnológica

* **Backend:** .NET 10 (ASP.NET Core Web API).
* **ORM:** Entity Framework Core (EF Core).
* **Infraestrutura:** Azure App Services e Docker.
* **Segurança:** JWT (JSON Web Tokens) e ASP.NET Identity.

## 3. Justificativa da Arquitetura Adotada

### 3.1. Escalabilidade Independente

Diferente de um monólito, o módulo de **Catálogo (CatalogService)**, que possui alta carga de leitura/busca, pode ser escalado horizontalmente sem a necessidade de replicar o módulo de **Identidade (IdentityService)** ou o **Chat**. Isso otimiza o uso de recursos na nuvem (Azure).

### 3.2. Manutenibilidade e Evolução (RNF08)

A separação por serviços permite que equipes diferentes trabalhem em módulos distintos sem interferência direta. O uso de .NET 10 garante acesso às últimas melhorias de performance e suporte a *Native AOT*, reduzindo o tempo de inicialização e o consumo de memória dos contêineres.

### 3.3. Resiliência e Isolamento de Falhas

Caso o serviço de **Chat** apresente instabilidade, as funções principais de busca e visualização de produtos (Catálogo) permanecem operacionais. Isso atende diretamente ao requisito de disponibilidade (**RNF04**).

### 3.4. Atendimento aos Requisitos Não Funcionais

* **Performance (RNF02):** A arquitetura permite o uso de cache distribuído por serviço, garantindo respostas em menos de 2 segundos.
* **Segurança (RNF05/RNF07):** O tráfego é isolado e protegido via TLS 1.3, com autenticação centralizada no IdentityService.

## 4. Trade-offs (Análise de Perdas e Ganhos)

| Critério | Impacto | Justificativa |
| --- | --- | --- |
| **Complexidade de Rede** | Negativo | Microserviços introduzem latência de rede. Será mitigado com o uso de HTTP/2 e gRPC em comunicações internas. |
| **Consistência de Dados** | Neutro | Adotamos consistência em fluxos críticos, exigindo uma modelagem mais cuidadosa das transações. |
| **Velocidade de Deploy** | Positivo | Pipelines de CI/CD independentes permitem entregas contínuas de funcionalidades menores sem risco para o sistema todo. |

---
name: csharp-domain-models
description: 'Use when generating C# 10/11+ domain models, entities, value objects, and repository interfaces for M-TAU.Domain with file-scoped namespaces, DataAnnotations, and Entity Framework Core-friendly mapping.'
argument-hint: 'Descreva o agregado, entidades, value objects e repositórios desejados'
user-invocable: true
disable-model-invocation: false
---

# C# Domain Models

## Quando Usar
- Gerar modelos de domínio dentro de `M-TAU.Domain`
- Criar entidades com identidade e ciclo de vida
- Criar value objects para conceitos sem identidade própria
- Criar interfaces de repositório base e específicas
- Manter compatibilidade com C# 10/11+, `Nullable`, e Entity Framework Core

## Resultado Esperado
- Código C# com `file-scoped namespace`
- Convenções compatíveis com `M_TAU.Domain`
- Validação básica com `DataAnnotations`
- Propriedades e construtores que suportem materialização do EF Core
- Estrutura mínima de domínio pronta para crescer por agregado

## Entradas Esperadas
- Nome do agregado ou módulo
- Entidades e seus identificadores
- Value objects necessários
- Regras básicas de validação
- Repositórios necessários

## Procedimento
1. Inspecione `M-TAU.Domain.csproj` e derive o namespace raiz a partir de `RootNamespace`.
2. Organize os arquivos por agregado ou pasta funcional, por exemplo `Entities`, `ValueObjects`, `Repositories` e `Common`.
3. Classifique cada tipo antes de gerar código.

### Decisão de Modelagem
- Use entidade quando o objeto possuir identidade estável, estado mutável controlado ou relações de navegação.
- Use value object quando a igualdade for definida pelos valores e o tipo puder ser imutável.
- Use `IRepository<TEntity, TKey>` quando a abstração for genérica o suficiente para reuso.
- Crie interface específica, como `IProdutoRepository`, quando houver consultas de domínio ou operações especializadas.
- Use primary constructor apenas quando a ergonomia compensar e não prejudicar o mapeamento; se houver dúvida, prefira construtor explícito com construtor protegido sem parâmetros para o EF Core.

### Geração dos Arquivos
1. Crie uma base comum de entidade usando [entity-base.cs](./assets/entity-base.cs).
2. Crie a base de value object usando [value-object.cs](./assets/value-object.cs) quando houver igualdade por componentes.
3. Crie a interface base de repositório usando [irepository.cs](./assets/irepository.cs).
4. Para cada entidade concreta, parta de [entity-template.cs](./assets/entity-template.cs).
5. Adapte nomes, tipos, validações e navegações ao agregado solicitado.

### Regras de Implementação
- Use `namespace` file-scoped.
- Preserve `Nullable` habilitado.
- Prefira `private set` ou `protected set` para invariantes, mantendo o EF Core capaz de materializar a entidade.
- Inicialize coleções de navegação com lista vazia.
- Aplique `DataAnnotations` apenas para validação básica e metadados simples, como `Required`, `StringLength`, `Range`, `EmailAddress` e `Phone`.
- Evite lógica de persistência no domínio.
- Não use `record` para entidades. Para value objects, `record` pode ser usado se a modelagem continuar clara e compatível com o restante do domínio.

## Checklist de Qualidade
- O arquivo usa `file-scoped namespace`.
- A entidade possui identificador e invariantes básicos.
- Existe construtor compatível com EF Core quando necessário.
- Propriedades anuláveis e obrigatórias estão coerentes.
- Navegações e coleções estão prontas para mapeamento.
- O repositório expõe apenas operações de domínio e persistência esperadas.
- O código evita dependências de infraestrutura.

## Conclusão
Considere a tarefa concluída quando houver:
- base compartilhada para entidades
- value objects somente onde agregam regra e semântica
- interfaces de repositório necessárias
- modelos compatíveis com o namespace `M_TAU.Domain`

## Exemplos de Prompt
- `/csharp-domain-models Gere o agregado Produto com entidade Produto, value object Dinheiro e interface IProdutoRepository.`
- `/csharp-domain-models Modele Cliente e Endereco em M-TAU.Domain com validações básicas e compatibilidade com EF Core.`
- `/csharp-domain-models Crie a base de entidades e repositórios do domínio de marketplace.`
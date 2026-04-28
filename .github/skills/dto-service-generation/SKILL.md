---
name: dto-service-generation
description: 'Generate DTOs (Create, Response, Filter/Query) as C# Records and async Service Interfaces for M-TAU.Application entities. Use when creating application layer for domain models with Task-based async signatures. Complements csharp-domain-models skill.'
argument-hint: 'Nome da entidade de domínio, propriedades desejadas e operações de serviço'
user-invocable: true
disable-model-invocation: false
---

# DTO and Service Interface Generation

## Quando Usar
- Gerar camada Application com DTOs baseados em entidades M-TAU.Domain
- Criar Service Interfaces com operações assíncronas (Task)
- Implementar padrão DTO com records C# 10+ imutáveis e performáticos
- Estruturar contrato de serviço entre Application e Presentation
- Evitar exposição direta de entidades de domínio na API/UI

## Resultado Esperado
- **DTOs como Records C#**: Imutáveis, com init, estruturais, menores em memória
- **3 Tipos de DTO por entidade**:
  - `{Entity}CreateDto` – entrada para criar (propriedades obrigatórias, sem Id)
  - `{Entity}ResponseDto` – saída de leitura (todas propriedades, incluindo Id)
  - `{Entity}FilterDto` – entrada para busca/filtros (propriedades opcionais)
- **Service Interfaces**:
  - Métodos assíncronos (Task/Task<T>)
  - CancellationToken suportado
  - Naming: `I{Entity}Service`
  - Organização: create, read, update, delete + operações de domínio
- **Namespace**: `M_TAU.Application.Services` e `M_TAU.Application.Dtos`
- **Compatibilidade**: .NET 10, Nullable habilitado, type-safe

## Entradas Esperadas
- **Entidade de Domínio**: nome completo com namespace `M_TAU.Domain.{Aggregate}`
- **Propriedades**: quais expor em cada tipo DTO
- **Operações Customizadas**: métodos específicos do domínio além de CRUD
- **Validações DTO**: DataAnnotations (Required, StringLength, Range, etc.)

## Procedimento

### 1. Inspecione a Entidade de Domínio
- Abra arquivo de entidade em `M-TAU.Domain/{Aggregate}/{Entity}.cs`
- Identifique propriedades públicas de leitura (get)
- Classifique: obrigatória, opcional, sensível (senhas, tokens)?
- Busque métodos de negócio que se tornam operações de serviço

### 2. Estruture os 3 DTOs
Seguindo [dto-record-template.cs](./assets/dto-record-template.cs):

#### CreateDto – Input para Criar
```csharp
public record {Entity}CreateDto(
    string PropertyRequired,
    string? PropertyOptional = null
);
```
- Inclua **apenas** propriedades que o cliente envia na criação
- **Nunca** inclua `Id`, `CreatedAt`, `UpdatedAt`
- Propriedades obrigatórias sem default, opcionais com `= null`
- Adicione DataAnnotations: `[Required]`, `[StringLength(120)]`, etc.

#### ResponseDto – Output de Leitura
```csharp
public record {Entity}ResponseDto(
    Guid Id,
    string Property,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null
);
```
- Inclua **todas** propriedades que o cliente lê
- Sempre comece com `Id`
- Sempre inclua timestamps (`CreatedAt`, `UpdatedAt`)
- Use tipos anuláveis só onde a entidade os permite

#### FilterDto – Input para Busca
```csharp
public record {Entity}FilterDto(
    string? Property = null,
    int? PageNumber = null,
    int? PageSize = null
);
```
- Todas propriedades com default `= null` (opcionais)
- Inclua `PageNumber`, `PageSize` para paginação
- Apenas propriedades usadas em filtros/busca

### 3. Crie a Service Interface
Seguindo [service-interface-template.cs](./assets/service-interface-template.cs):

```csharp
namespace M_TAU.Application.Services;

public interface I{Entity}Service
{
    Task<{Entity}ResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<{Entity}ResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<{Entity}ResponseDto>> FilterAsync({Entity}FilterDto filter, CancellationToken cancellationToken = default);
    
    Task<{Entity}ResponseDto> CreateAsync({Entity}CreateDto createDto, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Guid id, {Entity}CreateDto updateDto, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Operações customizadas de domínio
    // Exemplo: Task<IReadOnlyCollection<{Entity}ResponseDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
```

- **Prefixo**: `I{Entity}Service`
- **Namespace**: `M_TAU.Application.Services`
- **Métodos CRUD**: GetById, GetAll, Create, Update, Delete
- **Métodos Especializados**: Adicionar só operações extraídas de regras de domínio
- **Async por padrão**: Todos retornam `Task<T>` ou `Task`
- **CancellationToken**: Sempre opcional com `= default`
- **Collection Types**: `IReadOnlyCollection<T>` para evitar mutação

### 4. Organize Arquivos em M-TAU.Application
```
M-TAU.Application/
├── Dtos/
│   ├── Product/
│   │   ├── ProductCreateDto.cs
│   │   ├── ProductResponseDto.cs
│   │   └── ProductFilterDto.cs
│   ├── Order/
│   │   ├── OrderCreateDto.cs
│   │   ├── OrderResponseDto.cs
│   │   └── OrderFilterDto.cs
│   └── ...
├── Services/
│   ├── IProductService.cs
│   ├── IOrderService.cs
│   └── ...
└── Mappers/ (próxima phase)
```

### 5. Adicione DataAnnotations aos DTOs
Importe `using System.ComponentModel.DataAnnotations;` e aplique:
- `[Required]` – propriedade obrigatória
- `[StringLength(max, MinimumLength = min)]` – validar comprimento
- `[Range(min, max)]` – validar intervalo numérico
- `[EmailAddress]` – validar email
- `[Phone]` – validar telefone
- `[Url]` – validar URL
- Custom `[ValidationAttribute]` para regras complexas

Exemplo:
```csharp
public record ProductCreateDto(
    [Required]
    [StringLength(120, MinimumLength = 3)]
    string Title,
    
    [Range(0.01, double.MaxValue)]
    decimal Price,
    
    [StringLength(1000)]
    string? Description = null
);
```

## Opções de Automação

### Opção A: Geração Manual com Templates
1. Copie [dto-record-template.cs](./assets/dto-record-template.cs)
2. Personalize para cada DTO
3. Salve em `M-TAU.Application/Dtos/{Aggregate}/`

### Opção B: Geração Semi-Automática (PowerShell)
Veja [code-generator.ps1](./scripts/code-generator.ps1):
```powershell
.\code-generator.ps1 -Entity "Product" -Namespace "Catalog" -Properties "Title,Price,Description"
```
Gera automaticamente os 3 DTOs + Service Interface + estrutura de pastas.

## Checklist de Qualidade
- [ ] Todas DTOs usam `public record`
- [ ] CreateDto não possui `Id`, `CreatedAt`, `UpdatedAt`
- [ ] ResponseDto possui `Id` e timestamps
- [ ] FilterDto tem todas propriedades opcionais (com `= null`)
- [ ] Service Interface em namespace `M_TAU.Application.Services`
- [ ] Todos métodos retornam `Task<T>` ou `Task`
- [ ] `CancellationToken` é opcional em todas assinaturas
- [ ] DTOs estão em `M_TAU.Application.Dtos`
- [ ] Namespace segue padrão `M_TAU.Application` com file-scoped
- [ ] DataAnnotations aplicadas para validação básica
- [ ] Sem lógica de negócio (apenas passagem de dados)
- [ ] `IReadOnlyCollection<T>` para coleções de retorno

## Conclusão
Tarefa completa quando:
- 3 DTOs (Create, Response, Filter) gerados como records
- Service Interface criada em `M-TAU.Application/Services/`
- Todos arquivos respeitam namespace, async patterns, e validações
- Código está pronto para implementação de mappers e serviços

## Exemplos de Prompt

### Exemplo 1: Geração Simples
```
/dto-service-generation Gere DTOs e Service Interface para Product:
- CreateDto: Title (required, 3-120 chars), Price (required, > 0), Description (optional)
- ResponseDto: incluir Id, Title, Price, Description, Status, SellerId, CreatedAt
- Service: GetById, GetAll, Create, Update, Delete, ListBySeller
```

### Exemplo 2: Com Filtros Complexos
```
/dto-service-generation Para Order, gere:
- CreateDto: ProductId, BuyerId, TotalAmount
- ResponseDto: Id, ProductId, BuyerId, OrderDate, Status, TotalAmount
- FilterDto: ProductId, BuyerId, Status, OrderDateFrom, OrderDateTo, PageNumber, PageSize
- Service: operação customizada ListByBuyerAsync e UpdateStatusAsync
```

### Exemplo 3: Usar o Script
```
.\scripts\code-generator.ps1 -Entity "Message" -Namespace "Chat" -Properties "ChatSessionId,SenderId,Content,MessageStatus"
```

## Próximas Etapas
Após completar DTOs e Interfaces:
1. Crie Mappers: `M_TAU.Domain.{Entity}` ↔ `{Entity}Dto` usando AutoMapper/Mapster
2. Implemente Services: `M-TAU.Application/Services/Implementation/`
3. Registre em DI Container
4. Integre Controllers/Endpoints em M-TAU.API

## Referências
- C# Records: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records
- DataAnnotations: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannnotations
- Task-based Async: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/
- IReadOnlyCollection<T>: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1

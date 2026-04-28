using M_TAU.Application.Dtos.{Aggregate};

namespace M_TAU.Application.Services;

/// <summary>
/// I{Entity}Service provides application-layer operations for {Entity} management.
/// All methods are asynchronous (Task-based) and support CancellationToken for graceful shutdown.
/// </summary>
public interface I{Entity}Service
{
    /// <summary>
    /// Retrieves a single {Entity} by its unique Id.
    /// </summary>
    /// <param name="id">The unique identifier of the {Entity}</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The {Entity}ResponseDto if found; throws NotFoundException if not found</returns>
    Task<{Entity}ResponseDto> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all {Entity} records.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all {Entity}ResponseDto records</returns>
    Task<IReadOnlyCollection<{Entity}ResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters {Entity} records based on criteria in FilterDto.
    /// Supports pagination and multiple filter conditions.
    /// </summary>
    /// <param name="filter">Filter criteria including PropertyName, dates, and pagination</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Filtered collection of {Entity}ResponseDto records</returns>
    Task<IReadOnlyCollection<{Entity}ResponseDto>> FilterAsync(
        {Entity}FilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new {Entity} instance.
    /// </summary>
    /// <param name="createDto">DTO containing data for the new {Entity}</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The newly created {Entity}ResponseDto with assigned Id</returns>
    /// <exception cref="ValidationException">If CreateDto fails validation</exception>
    Task<{Entity}ResponseDto> CreateAsync(
        {Entity}CreateDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing {Entity}.
    /// </summary>
    /// <param name="id">The unique identifier of the {Entity} to update</param>
    /// <param name="updateDto">DTO containing updated data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <exception cref="NotFoundException">If {Entity} with given Id is not found</exception>
    /// <exception cref="ValidationException">If UpdateDto fails validation</exception>
    Task UpdateAsync(
        Guid id,
        {Entity}CreateDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a {Entity} by its Id.
    /// </summary>
    /// <param name="id">The unique identifier of the {Entity} to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <exception cref="NotFoundException">If {Entity} with given Id is not found</exception>
    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // ============================================================================
    // Domain-specific operations beyond CRUD
    // Uncomment and customize based on domain rules from M_TAU.Domain.{Entity}
    // ============================================================================

    /// <summary>
    /// Custom domain operation – List {Entity} records by a specific domain criteria.
    /// Example: List all active products, orders by buyer, etc.
    /// </summary>
    /// <param name="criteriaId">Domain-specific filter (e.g., SellerId, BuyerId, Status)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Filtered collection matching domain criteria</returns>
    // Task<IReadOnlyCollection<{Entity}ResponseDto>> ListByCriteriaAsync(
    //     Guid criteriaId,
    //     CancellationToken cancellationToken = default);

    /// <summary>
    /// Custom domain operation – Update domain state (e.g., change status, mark as deleted).
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="newStatus">New status or state value</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    // Task UpdateStatusAsync(
    //     Guid id,
    //     string newStatus,
    //     CancellationToken cancellationToken = default);
}

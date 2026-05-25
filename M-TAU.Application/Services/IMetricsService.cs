using M_TAU.Application.Dtos.Admin;

namespace M_TAU.Application.Services;

public interface IMetricsService
{
    Task<AdminMetricsDto> GetAdminMetricsAsync(CancellationToken cancellationToken = default);

    Task<SellerMetricsDto> GetSellerMetricsAsync(Guid sellerId, CancellationToken cancellationToken = default);
}

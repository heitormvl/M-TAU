namespace M_TAU.Application.Dtos.Admin;

public sealed record AdminMetricsDto(int Users, int Products, int Orders, int ChatSessions);

public sealed record SellerMetricsDto(int ActiveListings, int TotalSales, int PendingOrders, decimal Revenue);

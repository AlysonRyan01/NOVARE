namespace SharedService.Shared.Requests.SubscriptionService;

public record UpdateSubscriptionPlanSharedRequest(
    Guid Id,
    string Name,
    int MaxUsers,
    int MaxServiceOrders,
    decimal Price);
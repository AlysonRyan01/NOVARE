namespace SharedService.Shared.Requests.SubscriptionService;

public record CreateSubscriptionPlanSharedRequest(
    string Name,
    int MaxUsers,
    int MaxServiceOrders,
    decimal Price);
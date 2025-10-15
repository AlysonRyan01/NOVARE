namespace SharedService.Shared.Requests.SubscriptionService;

public record CheckoutRequest(decimal Amount, string PlanName, Guid EnterpriseId);

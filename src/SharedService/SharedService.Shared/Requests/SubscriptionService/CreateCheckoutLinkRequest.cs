namespace SharedService.Shared.Requests.SubscriptionService;

public record CreateCheckoutLinkRequest(string PlanName, Guid EnterpriseId, string EnterpriseEmail);
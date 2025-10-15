namespace SharedService.Shared.Requests.SubscriptionService;

public record CreateCheckoutSharedRequest(
    decimal Amount, 
    string PlanName,
    Guid EnterpriseId,
    string EnterpriseEmail);
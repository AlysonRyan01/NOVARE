namespace SharedService.Shared.Settings;

public class BillingSettings
{
    public string StripeApiKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
    public string CheckoutSuccessUrl { get; set; } = null!;
    public string CheckoutCancelUrl { get; set; } = null!;

    // Configurações de cobrança
    public string Currency { get; set; } = "brl";
    public string DefaultBillingInterval { get; set; } = "month";

    // IDs de produtos/preços já criados no Stripe (para produção)
    public string? PremiumPlanProductId { get; set; }
    public string? PremiumPlanPriceId { get; set; }
}
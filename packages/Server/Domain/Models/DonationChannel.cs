namespace Solidarity.Domain.Models;

public class DonationChannel : Model
{
	public string PaymentMethodIdentifier { get; set; } = null!;
	public Campaign Campaign { get; set; } = null!;
}
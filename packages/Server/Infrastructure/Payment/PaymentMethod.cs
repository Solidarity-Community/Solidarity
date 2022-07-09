namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentMethod : IHealthCheck
{
	protected readonly IDatabase _database;
	public PaymentMethod(IDatabase database) => _database = database;

	public string Identifier =>
		(GetType().GetCustomAttributes(typeof(PaymentMethodAttribute), true).First() as PaymentMethodAttribute)?.Identifier!;

	public virtual string Name => Identifier;

	protected string? Key
	{
		get => _database.PaymentMethodKeys.FirstOrDefault(key => key.PaymentMethodIdentifier == Identifier)?.Key;
		set
		{
			if (value is null)
			{
				_database.PaymentMethodKeys.Remove(
					_database.PaymentMethodKeys.Find(value) ?? throw new InvalidOperationException()
				);
			}
			else
			{
				_database.PaymentMethodKeys.Add(new()
				{
					Key = value,
					PaymentMethodIdentifier = Identifier
				});
			}

			_database.CommitChanges();
		}
	}

	public abstract Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
	public abstract Task<string> GetDonationData(Campaign campaign, Account? account);
	public abstract Task<decimal> GetBalance(Campaign campaign, Account? account);
	public abstract Task Refund(Campaign campaign, Account? account);
	public abstract Task<string> Fund(Campaign campaign, string destination);
}
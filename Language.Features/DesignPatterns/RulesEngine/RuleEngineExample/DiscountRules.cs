namespace RuleEngineExample;

public interface IDiscountRule
{
    int Order { get; }
    string Context { get; }
    decimal CalculateDiscount(Customer customer, decimal currentDiscount);
}


public class FirstTimeCustomerRule : IDiscountRule
{
    public int Order => 0;

    public string Context => "All";

    public decimal CalculateDiscount(Customer customer, decimal currentDiscount)
    {
        if (!customer.DateOfFirstPurchase.HasValue)
        {
            return .15m;
        }
        return 0;
    }
}

public class LoyalCustomerRule : IDiscountRule
{
    public int Order => 1;

    public string Context => "All";

    public decimal CalculateDiscount(Customer customer, decimal currentDiscount)
    {
        if (customer.DateOfFirstPurchase.HasValue)
        {
            if (customer.DateOfFirstPurchase.Value < DateTime.Now.AddYears(-15))
            {
                return .15m;
            }
            if (customer.DateOfFirstPurchase.Value < DateTime.Now.AddYears(-10))
            {
                return .12m;
            }
            if (customer.DateOfFirstPurchase.Value < DateTime.Now.AddYears(-5))
            {
                return .10m;
            }
            if (customer.DateOfFirstPurchase.Value < DateTime.Now.AddYears(-2))
            {
                return .08m;
            }
            if (customer.DateOfFirstPurchase.Value < DateTime.Now.AddYears(-1))
            {
                return .05m;
            }
        }
        return 0;
    }
}

public class VeteranRule : IDiscountRule
{
    public int Order => 2;

    public string Context => "All";

    public decimal CalculateDiscount(Customer customer, decimal currentDiscount)
    {
        if (customer.IsVeteran)
        {
            return .10m;
        }
        return 0;
    }
}

public class SeniorRule : IDiscountRule
{
    public int Order => 3;

    public string Context => "All";

    public decimal CalculateDiscount(Customer customer, decimal currentDiscount)
    {
        if (customer.DateOfBirth < DateTime.Now.AddYears(-65))
        {
            return .05m;
        }
        return 0;
    }
}

public class BirthdayRule : IDiscountRule
{
    public int Order => 4;

    public string Context => "All";

    public decimal CalculateDiscount(Customer customer, decimal currentDiscount)
    {
        bool isBirthday = customer.DateOfBirth.HasValue && customer.DateOfBirth.Value.Month == DateTime.Today.Month && customer.DateOfBirth.Value.Day == DateTime.Today.Day;

        if (isBirthday) return currentDiscount + 0.10m;
        return currentDiscount;
    }
}
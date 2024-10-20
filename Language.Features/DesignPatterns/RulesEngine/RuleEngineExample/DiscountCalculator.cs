using System.Reflection;

namespace RuleEngineExample;

public class DiscountCalculator
{
    public decimal CalculateDiscountPercentage(Customer customer)
    {
        var scenario = "All";
        var ruleType = typeof(IDiscountRule);
        var ruleCollection = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => ruleType.IsAssignableFrom(t) && !t.IsInterface)
            .Select(t => Activator.CreateInstance(t) as IDiscountRule)
            .Where(r => r!=null && r.Context == scenario)
            .OrderBy(r => r.Order) .ToList();

        var discountEngine = new DiscountRuleEngine(ruleCollection);
        return discountEngine.CalculateDiscountPercentage(customer);
    }
}

using DesignPatternsInCSharp.TemplateMethod;

namespace TemplatePattern;

public class PieBackingService : PanFoodServiceBase<Pie>
{
   

    public PieBackingService(LoggerAdapter loggerAdapter)
        :base(loggerAdapter)
    {
        
    }
    protected override void AddToppings()
    {
        _logger.Log("Adding pie filling");
    }

    protected override void PrepareCrust()
    {
        _logger.Log("Rolling out crust and pressing into pie pan");
    }

    protected override void Slice()
    {
        _logger.Log("Cutting into 6 slices.");
    }

    protected override void Bake()
    {
        _logger.Log("Baking for 45 minutes");
    }
    protected override void Cover()
    {
        _logger.Log("Adding lattice top");
    }
}

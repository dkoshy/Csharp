namespace ContactWriter;

internal class DemoAdd
{

    public DemoAdd( int n1 , int n2)
    {
        Num1 = n1;
        Num2 = n2;
    }
    public int Num1 { get; set; }
    public int Num2{ get; set; }
    public int Sum()
    {
        return Num1 + Num2;
    }
}

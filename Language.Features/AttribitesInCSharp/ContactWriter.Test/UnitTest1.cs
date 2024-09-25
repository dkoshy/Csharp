using ContactWriter;

namespace ContactWriter.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var expected = 70;
            var numadd = new DemoAdd(50, 20);

            var result = numadd.Sum();

            Assert.Equal(expected, result);
        }
    }
}
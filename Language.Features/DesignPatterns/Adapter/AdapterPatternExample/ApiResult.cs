using System.Collections.Generic;

namespace AdapterPatternExample
{
    public class ApiResult<T>
    {
        public int Count { get; set; }
        public List<T> Results { get; set; }
    }
}

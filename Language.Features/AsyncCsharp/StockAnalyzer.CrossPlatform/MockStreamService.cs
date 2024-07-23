using StockAnalyzer.Core.Domain;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalyzer.CrossPlatform
{

    public interface IStockStreamService
    {
        IAsyncEnumerable<StockPrice> GetAllStock(CancellationToken token = default);
    }
    public class MockStreamService : IStockStreamService
    {
        public async  IAsyncEnumerable<StockPrice> GetAllStock(
            [EnumeratorCancellation] CancellationToken token = default)
        {
            await Task.Delay(500 , token);
            yield return new StockPrice { Identifier = "A2", Change = 0.1m };
            await Task.Delay(500, token);
            yield return new StockPrice { Identifier = "A3", Change = 0.1m };
            await Task.Delay(500, token);
            yield return new StockPrice { Identifier = "A4", Change = 0.1m };
            await Task.Delay(500, token);
            yield return new StockPrice { Identifier = "A5", Change = 0.1m };
            await Task.Delay(500, token);
            yield return new StockPrice { Identifier = "A6", Change = 0.1m };
        }
    }

    public class StockPriceStreamService : IStockStreamService
    {
        public async  IAsyncEnumerable<StockPrice> 
            GetAllStock([EnumeratorCancellation]CancellationToken token = default)
        {
            using var lineStream = new StreamReader(File.OpenRead("StockPrices_Small.csv"));

            await lineStream.ReadLineAsync(); //skip the heading

            while(await lineStream.ReadLineAsync() is string line)
            {
                if(token.IsCancellationRequested)
                {
                    break;
                }

                yield return StockPrice.FromCSV(line);
            }
        }
    }
}

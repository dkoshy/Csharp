using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using HarfBuzzSharp;
using Newtonsoft.Json;
using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalyzer.CrossPlatform;

public partial class MainWindow : Window
{
    public DataGrid Stocks => this.FindControl<DataGrid>(nameof(Stocks));
    public ProgressBar StockProgress => this.FindControl<ProgressBar>(nameof(StockProgress));
    public TextBox StockIdentifier => this.FindControl<TextBox>(nameof(StockIdentifier));
    public Button Search => this.FindControl<Button>(nameof(Search));
    public TextBox Notes => this.FindControl<TextBox>(nameof(Notes));
    public TextBlock StocksStatus => this.FindControl<TextBlock>(nameof(StocksStatus));
    public TextBlock DataProvidedBy => this.FindControl<TextBlock>(nameof(DataProvidedBy));
    public TextBlock IEX => this.FindControl<TextBlock>(nameof(IEX));
    public TextBlock IEX_Terms => this.FindControl<TextBlock>(nameof(IEX_Terms));

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        IEX.PointerPressed += (e, a) => Open("https://iextrading.com/developer/");
        IEX_Terms.PointerPressed += (e, a) => Open("https://iextrading.com/api-exhibit-a/");

        /// Data provided for free by <a href="https://iextrading.com/developer/" RequestNavigate="Hyperlink_OnRequestNavigate">IEX</Hyperlink>. View <Hyperlink NavigateUri="https://iextrading.com/api-exhibit-a/" RequestNavigate="Hyperlink_OnRequestNavigate">IEX’s Terms of Use.</Hyperlink>
    }




    private static string API_URL = "https://ps-async.fekberg.com/api/stocks";

    private Stopwatch stopwatch = new Stopwatch();

    private CancellationTokenSource cancellationTokenSource;


    #region FromEvets
    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        BeforeLoadingStockData();

        //GetStockDataFromWebClient();
        //await GetStockFromFile();
        //await GetStocksUsingTPLVersion1();
        //GetStocksWithCancelationTokenusingTPL();
        //await GetStocksUsingHttpClinetWithCancelationToken();
        //await GetMultipleStockDetails();
        //await GetStocksAndProcessItImmediatly();
        //await GetStreamOfAsyncDataWithMockService();

        await GetStocksByParallelOeration();


        AfterLoadingStockData();
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Shutdown();
        }
    }

    #endregion

    public static void Open(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }

    #region PrivateMethodes


    private async Task GetStocksByParallelOeration()
    {
        
        var stocks = new Dictionary<string, Task<IEnumerable<StockPrice>>>
            {
                { "MSFT",  GetStockByIdentifier("MSFT") },
                { "GOOGL", GetStockByIdentifier("GOOGL") },
                { "AAPL",  GetStockByIdentifier("AAPL") },
                { "CAT",   GetStockByIdentifier("CAT") },
                { "ABC",   GetStockByIdentifier("ABC") },
                { "DEF",   GetStockByIdentifier("DEF") }
            };
        var bag = new ObservableCollection<StockCalculation>();
      

        try
        {
            /*
             await Task.Run(() =>
            {
                var bag = new ObservableCollection<StockCalculation>();

                Parallel.ForEach(stocks,  async (s, e) =>
                {
                    var result = await s.Value;
                    var calculated = Calculate(result);
                });
            });*/

            await Parallel.ForEachAsync(stocks, async (stocks, state) =>
            {
                var result = await stocks.Value;
                var calculated = Calculate(result);
                if (calculated is not null)
                    bag.Add(calculated);

            });
        }
        catch(Exception  ex)
        {
            Notes.Text = ex.Message;
        }


        //Stocks.Items = bag;
        Stocks.Items = bag;
    }


    private async Task<IEnumerable<StockPrice>> GetStockByIdentifier(string identifier)
    {
        using var client = new HttpClient();
        var resposne = await client.GetAsync($"{API_URL}/{identifier}");
        var content = await resposne.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);
        return data;
    }

    private StockCalculation Calculate(IEnumerable<StockPrice> prices)
    {
        if(prices == null)
        {
            return default;
        }
        var calculation = new StockCalculation();
        calculation.Identifier = prices.First().Identifier;
        calculation.Result = prices.Average(s => s.Change);

        return calculation;
    }

    private async Task GetStreamOfAsyncDataWithMockService()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            Search.Content = "Search";
            return;
        }

        try
        {

            var streamofData = new StockPriceStreamService();    //new MockStreamService();
            var stockenumerator = streamofData.GetAllStock();
            var data = new ObservableCollection<StockPrice>();
            Stocks.Items = data;

            cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text = "Cancelation is Requested";
            });

            await foreach (var stock in stockenumerator
                .WithCancellation(cancellationTokenSource.Token))
            {
                data.Add(stock);
            }
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
        finally
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Search.Content = "Search";
        }
    }

    private async Task GetStocksAndProcessItImmediatly()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            Search.Content = "Search";
            return;
        }

        try
        {
            cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text = "Cancelation is Requested";
            });

            var identifiers = StockIdentifier.Text.Split(',', ' ');
            var tasks = new List<Task<IEnumerable<StockPrice>>>();
            var processedStocks = new ConcurrentBag<StockPrice>();
            Search.Content = "Cancel";
            var service = new StockService();

            foreach (var identifier in identifiers)
            {
                //http client will hadle request canacelation process.
                var loadTask = service.GetStockPricesFor(identifier, cancellationTokenSource.Token);

                loadTask = loadTask.ContinueWith(t =>
                {
                    var result = t.Result.Take(5);

                    result.ToList().ForEach((r =>
                    {
                        r.Identifier = $"{r.Identifier} - PR ";
                        processedStocks.Add(r);
                    }));

                    return result;
                });

                tasks.Add(loadTask);
            }

            await Task.WhenAll(tasks);
            Stocks.Items = processedStocks;
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
        finally
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Search.Content = "Search";
        }
    }
    private async Task GetMultipleStockDetails()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            Search.Content = "Search";
            return;
        }

        try
        {
            cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text = "Cancelation is Requested";
            });

            var identifiers = StockIdentifier.Text.Split(',', ' ');
            var tasks = new List<Task<IEnumerable<StockPrice>>>();
            Search.Content = "Cancel";
            var service = new StockService();

            foreach (var identifier in identifiers)
            {
                //http client will hadle request canacelation process.
                tasks.Add(service.GetStockPricesFor(identifier,
                        cancellationTokenSource.Token));
            }

            var data = await Task.WhenAll(tasks);
            Stocks.Items = data.SelectMany(s => s);
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
        finally
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Search.Content = "Search";
        }
    }

    private async Task GetStocksUsingHttpClinetWithCancelationToken()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;

            Search.Content = "Search";
            return;
        }

        try
        {
            cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text = "Cancelation is Requested";
            });

            Search.Content = "Cancel";
            var service = new StockService();
            //http client will hadle request canacelation process.
            var data = await service.GetStockPricesFor(StockIdentifier.Text,
                    cancellationTokenSource.Token);

            Stocks.Items = data;

        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
        finally
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Search.Content = "Search";
        }

    }

    private void GetStocksWithCancelationTokenusingTPL()
    {
        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;

            Search.Content = "Search";
            return;
        }

        try
        {
            cancellationTokenSource = new();

            Search.Content = "Cancel";

            var token = cancellationTokenSource.Token;

            token.Register(() =>
            {
                Notes.Text = "Cancellation is requested.";
            });

            var getStockLines = Task.Run<List<string>>(async () =>
            {
                using var lineStream = new StreamReader(File.OpenRead("StockPrices_Small.csv"));

                var lines = new List<string>();

                while (await lineStream.ReadLineAsync() is string line)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    lines.Add(line);
                }

                return lines;

            }, token);

            getStockLines.ContinueWith(async t =>
            {

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Notes.Text = t.Exception.InnerException.Message;
                });

            }, TaskContinuationOptions.OnlyOnFaulted);

            var taskToExtractStockPrice = getStockLines.ContinueWith(async (taskCompleted) =>
            {
                var lines = taskCompleted.Result;

                var data = new List<StockPrice>();

                foreach (var line in lines.Skip(1))
                {
                    data.Add(StockPrice.FromCSV(line));
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Stocks.Items = data.Where(s => s.Identifier == StockIdentifier.Text).ToList();
                });

            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            taskToExtractStockPrice.ContinueWith(async _ =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    AfterLoadingStockData();

                    cancellationTokenSource = null;
                    cancellationTokenSource?.Dispose();
                    Search.Content = "Search";

                });
            });

        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
    }

    private void GetStocksWithTPLwithContinueVersion()
    {
        try
        {
            var taskGetstockList = Task.Run<List<string>>(async () =>
            {
                using var lineStream = new StreamReader(File.OpenRead("StockPrices_Small.csv"));

                var lines = new List<string>();

                while (await lineStream.ReadLineAsync() is string line)
                {
                    lines.Add(line);
                }

                return lines;
            });

            taskGetstockList.ContinueWith(async (taskFaulted) =>
            {
                var message = taskFaulted.Exception.InnerException.Message;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {

                    Notes.Text = message;
                });

            }, TaskContinuationOptions.OnlyOnFaulted);

            var procesedTask = taskGetstockList.ContinueWith(async (taskCompleted) =>
            {

                var lines = taskCompleted.Result;

                var data = new List<StockPrice>();

                foreach (var line in lines.Skip(1))
                {
                    data.Add(StockPrice.FromCSV(line));
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Stocks.Items = data.Where(s => s.Identifier == StockIdentifier.Text).ToList();
                });
            },
           TaskContinuationOptions.OnlyOnRanToCompletion);

            procesedTask.ContinueWith(async _ =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    AfterLoadingStockData();
                });
            });
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
    }

    private async Task GetStocksUsingTPLVersion1()
    {
        var data = new List<StockPrice>();
        try
        {
            data = await Task.Run(() =>
            {
                var lines = File.ReadAllLines("StockPrices_Small.csv");
                var data = new List<StockPrice>();
                foreach (var line in lines.Skip(1))
                {
                    var stockPrice = StockPrice.FromCSV(line);
                    data.Add(stockPrice);
                }

                return data;
            });


            var result = data.Where(s => s.Identifier == StockIdentifier.Text).ToList();

            if (result == null || result.Count == 0)
                throw new Exception($"Cound not load stock details {StockIdentifier.Text}");

            Stocks.Items = result;
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }
    }
    private void GetStockDataFromWebClient()
    {
        var client = new WebClient();

        var content = client.DownloadString($"{API_URL}/{StockIdentifier.Text}");

        // Simulate that the web call takes a very long time
        Thread.Sleep(10000);

        var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

        // This is the same as ItemsSource in WPF used in the course videos
        Stocks.Items = data;
    }

    private async Task GetStockFromHttpClient()
    {
        using (var client = new HttpClient())
        {
            var resposne = await client.GetAsync($"{API_URL}/{StockIdentifier.Text}");

            var content = await resposne.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

            Stocks.Items = data;
        }
    }

    private async Task GetStockFromFile()
    {
        try
        {
            var source = new DataStore();
            var stocks = await source.GetStockPrices(StockIdentifier.Text);

            Stocks.Items = stocks;
        }
        catch (Exception ex)
        {
            Notes.Text = ex.Message;
        }

    }

    private void BeforeLoadingStockData()
    {
        stopwatch.Restart();
        StockProgress.IsVisible = true;
        StockProgress.IsIndeterminate = true;
    }

    private void AfterLoadingStockData()
    {
        StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
        StockProgress.IsVisible = false;
    }
    #endregion

}

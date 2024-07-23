// See https://aka.ms/new-console-template for more information
using ConsoleApp.Async;


Console.WriteLine("Sarting Async Example.");

var examples = new AsyncAdvanceExamples();

//await examples.AnExmaple();

foreach(var data in examples.ParalalleExample())
{
    Console.WriteLine(data);
}



Console.WriteLine("Ending Async Example.");
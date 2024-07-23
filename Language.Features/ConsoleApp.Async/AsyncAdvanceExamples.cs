using System.Collections.Concurrent;

namespace ConsoleApp.Async
{
    public class AsyncAdvanceExamples
    {

        public  Task AnExmaple()
        {

            return Task.Factory.StartNew(() =>
            {
                string message = "From Main Thread";

                Task.Factory.StartNew(() => { Console.WriteLine("Sarting 1 "); },TaskCreationOptions.AttachedToParent);
                Task.Factory.StartNew(() => { Console.WriteLine("Sarting 2 "); }, TaskCreationOptions.AttachedToParent);
                Task.Factory.StartNew(() => { Console.WriteLine("Sarting 3 "); }, TaskCreationOptions.AttachedToParent);
                Task.Factory.StartNew(m => { Console.WriteLine(m); }, message, TaskCreationOptions.AttachedToParent);
             });
        }

        public ConcurrentBag<string> ParalalleExample()
        {
            var bag = new ConcurrentBag<string>();

            Parallel.Invoke(
            () =>
            { bag.Add("data1"); },
            () =>
            { bag.Add("data"); },
            () =>
            { bag.Add("data"); },
            () =>
            { bag.Add("data"); }

            );

            return bag;
       }
    }
}

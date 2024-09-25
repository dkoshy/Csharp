using static System.Console;

using ContactWriter;

var consoleWriter = new ContactConsoleWriter(new Contact
{
    FirstName = "Deepak",
    AgeInYears = 35
});



consoleWriter.Write();

ReadLine();
using ContactWriter.CustomAttributes;
using System;
using System.Diagnostics;

namespace ContactWriter;

//[DebuggerDisplay("Name is {FirstName} Age {AgeInYears}")]
[DebuggerTypeProxy(typeof(ContactDebuggerDisplay))]
[DeafaultForegroundColor]
public class Contact
{
    [Display("First Name :" , ConsoleColor.Cyan)]
    //[Intent]
    //[Intent]
    public string FirstName { get; set; }


    //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public int AgeInYears { get; set; }
}

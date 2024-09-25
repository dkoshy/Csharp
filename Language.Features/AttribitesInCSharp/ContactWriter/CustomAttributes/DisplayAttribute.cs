using System;

namespace ContactWriter.CustomAttributes;

[AttributeUsage(AttributeTargets.Property)]
internal class DisplayAttribute :Attribute
{

    public DisplayAttribute(string label , ConsoleColor color =  ConsoleColor.White)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
        Colour = color;
    }

    public string Label { get;  }
    public ConsoleColor Colour { get;  }

}

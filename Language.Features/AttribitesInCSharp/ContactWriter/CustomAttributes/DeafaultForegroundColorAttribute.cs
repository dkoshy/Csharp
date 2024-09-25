using System;

namespace ContactWriter.CustomAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class DeafaultForegroundColorAttribute :Attribute
{
    public ConsoleColor DefaultColor => ConsoleColor.Blue;
}

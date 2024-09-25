using System;

namespace ContactWriter.CustomAttributes;

[AttributeUsage(AttributeTargets.Property , AllowMultiple = true)]
public class IntentAttribute :Attribute
{

}

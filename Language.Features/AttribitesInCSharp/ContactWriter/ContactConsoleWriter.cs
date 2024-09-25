#define Skip_Step1
#define Skip_Step2

using ContactWriter.CustomAttributes;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using static System.Console;



namespace ContactWriter;


public  class ContactConsoleWriter
{
    private readonly Contact _contact;
    private  ConsoleColor _color;

    public ContactConsoleWriter(Contact contact)
    {
        _contact = contact;
        PreserveColor();
    }

    // [Obsolete("This will be removed in upcomming sprints.", true)]
    //[SkipExe]
    public void Write()
    {
        var isSkipped = Attribute.GetCustomAttribute(MethodBase.GetCurrentMethod(), typeof(SkipExeAttribute))
                        != null ? true : false;
        if(isSkipped) { return; }

        WriteName();
        WriteAge();
       
    }

    [Conditional("Skip_Step1")]
    private void WriteName()
    {
        //WriteLine(_contact.FirstName);
        var contactpopInfo = _contact.GetType().GetProperty(nameof(_contact.FirstName));
        var disattributeInfo = (DisplayAttribute)Attribute.GetCustomAttribute(contactpopInfo, typeof(DisplayAttribute));
        var IntentattributeInfo = (IntentAttribute[])Attribute.GetCustomAttributes(contactpopInfo, typeof(IntentAttribute));

        var displayText = new StringBuilder();
       
        displayText.Append(' ', (IntentattributeInfo?.Length ?? 0) * 4);
        

        if (disattributeInfo != null)
        {
            ForegroundColor = disattributeInfo.Colour;
            displayText.Append(disattributeInfo.Label);
        }

        displayText.Append(_contact.FirstName);
        WriteLine(displayText);

        SetColor();
    }

    [Conditional("Skip_Step2")]
    private void WriteAge()
    {
        WriteLine(_contact.AgeInYears);
    }

    private void PreserveColor()
    {
        var defaultcolorAttribute = (DeafaultForegroundColorAttribute) Attribute.GetCustomAttribute(_contact.GetType(), typeof(DeafaultForegroundColorAttribute));

        _color = defaultcolorAttribute == null ?  ForegroundColor : defaultcolorAttribute.DefaultColor; 

        SetColor();
    }

    private void SetColor()
    {
        ForegroundColor = _color;
    }
}

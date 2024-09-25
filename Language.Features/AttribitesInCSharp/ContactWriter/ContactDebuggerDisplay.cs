namespace ContactWriter;

public class ContactDebuggerDisplay
{
    private readonly Contact _contact;

    public ContactDebuggerDisplay(Contact contact)
    {
        _contact = contact;
    }


    public string ToUpperName => _contact.FirstName.ToUpperInvariant();
    public string AgeInHex => _contact.AgeInYears.ToString("X");


}
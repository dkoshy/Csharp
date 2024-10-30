namespace AdapterPatternExample;

public interface ICharecterSourceAdapter
{
    Task<IEnumerable<Person>> GetCharacters();
}

public class FileSourceAdapter : ICharecterSourceAdapter
{
    private readonly string _filePath;
    private readonly CharacterFileSource _fileSource;

    public FileSourceAdapter(string filePath 
        , CharacterFileSource fileSource)
    {
        _filePath = filePath;
        _fileSource = fileSource;
    }
    public async Task<IEnumerable<Person>> GetCharacters()
    {
        /*
        string filePath = @"Adapter/People.json";
        var characterSource = new CharacterFileSource();
        var people = await _fileSource.GetCharactersFromFile(filePath);

        return people;*/

        var people = (await _fileSource.GetCharactersAnotherFromFile(_filePath))
             .Select(c => new CharactersToPeopleAdapter(c));
        return people;
            
    }

    
}

public class APISourceAdapter : ICharecterSourceAdapter
{
    private readonly StarWarsApi _apiSource;

    public APISourceAdapter(StarWarsApi apiSource)
    {
        _apiSource = apiSource;
    }
    public async Task<IEnumerable<Person>> GetCharacters()
    {
       var people = await _apiSource.GetCharacters();

      return people;
    }
}

public class CharactersToPeopleAdapter : Person 
{
    private readonly Character _character;

    public CharactersToPeopleAdapter(Character character)
    {
        _character = character;
    }


    public override string Name
    {
        get => _character.FullName;
        set => _character.FullName = value;
    }

    public override string HairColor 
    {
        get => _character.Hair;
        set => _character.Hair = value;
    }

    public override string Gender
    {
        get => _character.Gender;
        set => _character.Gender = value;
    }
}



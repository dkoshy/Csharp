using Xunit.Abstractions;

namespace AdapterPatternExample
{
    public class TestRunner
    {
        private readonly ITestOutputHelper _output;

        public TestRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        /*
        
        [Fact]
        public async Task DisplayCharactersFromFile()
        {
            var service = new StarWarsCharacterDisplayService();

            var result = await service.ListCharacters(StarWarsCharacterDisplayService.CharacterSource.File);

            _output.WriteLine(result);
        }

        [Fact]
        public async Task DisplayCharactersFromApi()
        {
            var service = new StarWarsCharacterDisplayService();

            var result = await service.ListCharacters(StarWarsCharacterDisplayService.CharacterSource.Api);

            _output.WriteLine(result);
        }
        */
        

        [Fact]
        public async Task DisplayCharactersFromFile()
        {
            string filename = @"People.json";
            var service = new StarWarsCharacterDisplayService(
                new FileSourceAdapter(filename, new CharacterFileSource()));

            var result = await service.ListCharacters();

            _output.WriteLine(result);
        }

        [Fact]
        public async Task DisplayCharactersFromApi()
        {
            var service = new StarWarsCharacterDisplayService(
                new APISourceAdapter(new StarWarsApi()));

            var result = await service.ListCharacters();

            _output.WriteLine(result);
        }
    }
}

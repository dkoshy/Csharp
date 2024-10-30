using Newtonsoft.Json;

namespace AdapterPatternExample
{
    public class StarWarsApi
    {
        public async Task<List<Person>> GetCharacters()
        {
            using (var client = new HttpClient())
            {
                string url = ApiConstants.SWAPI_PEOPLE_ENDPOINT;
                string result = await client.GetStringAsync(url);
                var people = JsonConvert.DeserializeObject<ApiResult<Person>>(result).Results;

                return people;
            }
        }
    }
}

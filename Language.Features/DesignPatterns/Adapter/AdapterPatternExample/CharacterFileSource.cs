﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AdapterPatternExample
{
    public class CharacterFileSource
    {
        public async Task<List<Person>> GetCharactersFromFile(string filename)
        {
            var characters = JsonConvert.DeserializeObject<List<Person>>(await File.ReadAllTextAsync(filename));

            return characters;
        }


        public async Task<List<Character>> GetCharactersAnotherFromFile(string filename)
        {
            var characters = JsonConvert.DeserializeObject<List<Character>>(await File.ReadAllTextAsync(filename));

            return characters;
        }
    }
}
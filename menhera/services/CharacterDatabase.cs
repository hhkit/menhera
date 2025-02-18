using System.Diagnostics;
using System.Text.Json;

namespace menhera
{

    public class CharacterDatabase : Service
    {
        readonly Dictionary<string, CharacterData> database = new();
        readonly JsonSerializerOptions options =
            new JsonSerializerOptions() { AllowTrailingCommas = true };

        public string Serialize(CharacterData charData)
        {
            return JsonSerializer.Serialize(charData, options);
        }

        public void LoadCharacterFromJson(string json)
        {
            var characterData = JsonSerializer.Deserialize<CharacterData>(json, options);
            Debug.WriteLine("name: " + characterData.Name);
            Debug.Assert(!database.ContainsKey(characterData.Name));
            database.Add(characterData.Name, characterData);
        }

        public void LoadCharacterFromFile(string filePath)
        {
            using FileStream openStream = File.OpenRead(filePath);
            var characterData = JsonSerializer.Deserialize<CharacterData>(openStream, options);
            Debug.Assert(!database.ContainsKey(characterData.Name));
            database.Add(characterData.Name, characterData);
        }
    }
}
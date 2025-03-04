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

        public void RegisterCharacter(CharacterData characterData)
        {
            database.Add(characterData.Id, characterData);
        }

        public bool LoadCharacterFromJson(string json)
        {
            var characterData = JsonSerializer.Deserialize<CharacterData>(json, options);
            Debug.Assert(!database.ContainsKey(characterData.Name));
            RegisterCharacter(characterData);
            return true;
        }

        public void LoadCharacterFromFile(string filePath)
        {
            using FileStream openStream = File.OpenRead(filePath);
            var characterData = JsonSerializer.Deserialize<CharacterData>(openStream, options);
            Debug.Assert(!database.ContainsKey(characterData.Name));
            RegisterCharacter(characterData);
        }

        public bool GetCharacter(string id, out CharacterData characterData)
        {
            return database.TryGetValue(id, out characterData);
        }
    }
}
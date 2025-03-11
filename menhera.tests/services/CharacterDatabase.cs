using System.Diagnostics;

namespace menhera.tests
{
    [TestClass]
    public sealed class TestCharacterDatabase
    {
        [TestMethod]
        public void TestSave()
        {
            ServiceLocator serviceLoc = new();
            var characterDb = serviceLoc.GetService<CharacterDatabase>();
            Assert.IsNotNull(characterDb);
            var charData = new CharacterData()
            {
                Id = "player_test",
                Name = "Test Kid",
                MaxHp = 100,
                StaggerThresholds = [80, 40],
                Skills = [
                    new SkillData() {
                        BasePower = 4,
                        CoinPower = 4,
                        CoinCount = 2,
                    }
                ]
            };

            var data = characterDb.Serialize(charData);
            Debug.WriteLine(data);
        }

        [TestMethod]
        public void TestLoad()
        {
            ServiceLocator serviceLoc = new();
            var characterDb = serviceLoc.GetService<CharacterDatabase>();
            Assert.IsNotNull(characterDb);
            var succ = characterDb.LoadCharacterFromJson(
                """
                {
                    "Id": "player_test",
                    "Name": "Test Kid",
                    "StaggerThresholds": [
                        80,
                        40,
                    ],
                    "Skills": [
                        {
                            "BasePower": 4,
                            "CoinPower": 3,
                            "CoinCount": 2,
                        }
                    ]
                }
                """
                           );

            Assert.IsTrue(succ);
            var getChar = characterDb.GetCharacter("player_test", out var characterData);
            Assert.IsTrue(getChar);
            Assert.AreEqual("player_test", characterData.Id);
            Assert.AreEqual("Test Kid", characterData.Name);
            Assert.AreEqual(2, characterData.StaggerThresholds.Length);
            Assert.AreEqual(80, characterData.StaggerThresholds[0]);
            Assert.AreEqual(40, characterData.StaggerThresholds[1]);
            Assert.AreEqual(4, characterData.Skills[0].BasePower);
            Assert.AreEqual(3, characterData.Skills[0].CoinPower);
            Assert.AreEqual(2, characterData.Skills[0].CoinCount);
        }
    }
}
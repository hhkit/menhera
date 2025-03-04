using System.Diagnostics;

namespace menhera.tests
{
    [TestClass]
    public sealed class TestCharacterDatabase
    {
        [TestMethod]
        public void TestSave()
        {
            ServiceLocator.Initialize();
            var serviceLoc = ServiceLocator.Main;
            var characterDb = serviceLoc.GetService<CharacterDatabase>();
            Debug.Assert(characterDb != null);
            var charData = new CharacterData()
            {
                Id = "player_test",
                Name = "Test Kid",
                StartingHp = 100,
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
            ServiceLocator.Initialize();
            var serviceLoc = ServiceLocator.Main;
            var characterDb = serviceLoc.GetService<CharacterDatabase>();
            Debug.Assert(characterDb != null);
            var succ = characterDb.LoadCharacterFromJson(
                """
                {
                    "Id": "player_test",
                    "Name": "Test Kid",
                    "StartingHp": 100,
                    "StaggerThresholds": [
                        80,
                        40,
                    ],
                    "Skills": [
                        {
                            "BasePower": 4,
                            "CoinPower": 4,
                            "CoinCount": 2,
                        }
                    ]
                }
                """
                           );
            Debug.Assert(succ);
        }
    }
}
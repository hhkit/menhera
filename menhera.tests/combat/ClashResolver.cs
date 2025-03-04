using System.Diagnostics;

namespace menhera.tests
{
    [TestClass]
    public sealed class TestClashResolver
    {
        [TestMethod]
        public void TestClash()
        {
            ServiceLocator services = new();
            var actorService = services.GetService<ActorService>();
            var messagingService = services.GetService<MessagingService>();

            var playerSkillData = new SkillData()
            {
                BasePower = 4,
                CoinPower = 0,
                CoinCount = 2,
            };

            var enemySkillData = new SkillData()
            {
                BasePower = 0,
                CoinPower = 2,
                CoinCount = 2,
            };

            var player = new CombatActor()
            {
                data = new CharacterData()
                {
                    Skills = [playerSkillData]
                },
                sanity = 0,
            };

            var enemy = new CombatActor()
            {
                data = new CharacterData()
                {
                    Skills = [enemySkillData]
                },
                sanity = 0,
            };
            actorService.Register(player, 0);
            actorService.Register(enemy, 1);
            var systemId = actorService.Register(new CombatActor(), -1);

            bool eventCalled = false;

            Clash clash = new(player, playerSkillData, enemy, enemySkillData);
            ClashResolver resolver = new(clash, services);

            messagingService.Listen(systemId, (OnClashWin onClashWin) =>
            {
                eventCalled = true;
                Assert.AreSame(onClashWin.Winner.Combatant, player, "player should always win");
                Assert.IsTrue(onClashWin.clashCount >= enemySkillData.CoinCount, $"there should be at least {enemySkillData.CoinCount} clashes");
            }
            , Scope.All);

            var clashResult = resolver.ResolveClash();
            Assert.AreEqual(ClashResolver.ClashResult.PlayerWin, clashResult, "Player should win this clash");
            Assert.IsTrue(eventCalled, "OnClashWin should be called");
        }
    }
}
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
                BasePower = 6,
                CoinPower = 2,
                CoinCount = 3,
            };

            var enemySkillData = new SkillData()
            {
                BasePower = 0,
                CoinPower = 3,
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
            messagingService.Listen(systemId, (OnClashWin onClashWin) =>
            {
                eventCalled = true;
                Debug.Assert(onClashWin.clashCount == enemySkillData.CoinCount, $"player should always win, therefore, we should clash {enemySkillData.CoinCount} times");
            }
            , Scope.All);

            Clash clash = new(player, playerSkillData, enemy, enemySkillData);
            ClashResolver resolver = new(clash, services);

            var clashResult = resolver.ResolveClash();
            Debug.Assert(clashResult == ClashResolver.ClashResult.PlayerWin, "Player should win this clash");
            Debug.Assert(eventCalled, "OnClashWin should be called");
        }
    }
}
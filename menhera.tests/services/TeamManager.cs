namespace menhera.tests
{
    [TestClass]
    public sealed class TestTeamManager
    {
        [TestMethod]
        public void TestTeamFlags()
        {

            ServiceLocator services = new();
            var actorService = services.GetService<ActorService>();
            var teamManager = services.GetService<TeamManager>();

            var player = actorService.Register(new CombatActor()
            {
                sanity = 0,
            }, 0);

            var allies = Enumerable.Range(0, 6).Select(ind => actorService.Register(new CombatActor(), 0)).ToArray();
            var enemies = Enumerable.Range(0, 6).Select(ind => actorService.Register(new CombatActor(), 1)).ToArray();

            {
                var playerEnemyFilter = teamManager.GetFilterFor(player, Scope.Self);
                Assert.IsTrue((player.Flag & playerEnemyFilter) != 0, "player should be in player's self filter");
                Assert.IsFalse(allies.All(ally => (ally.Flag & playerEnemyFilter) != 0), "allies should not be in player's self filter");
                Assert.IsFalse(enemies.All(enemy => (enemy.Flag & playerEnemyFilter) != 0), "enemies should not be in player's self filter");
            }


            {
                var playerEnemyFilter = teamManager.GetFilterFor(player, Scope.Enemy);
                Assert.IsFalse((player.Flag & playerEnemyFilter) != 0, "player should not be in player's enemy filter");
                Assert.IsFalse(allies.All(ally => (ally.Flag & playerEnemyFilter) != 0), "allies should not be in player's enemy filter");
                Assert.IsTrue(enemies.All(enemy => (enemy.Flag & playerEnemyFilter) != 0), "enemies should be in player's enemy filter");
            }

            {
                var playerAllyFilter = teamManager.GetFilterFor(player, Scope.Ally);
                Assert.IsTrue((player.Flag & playerAllyFilter) != 0, "player should be in player's ally filter");
                Assert.IsTrue(allies.All(ally => (ally.Flag & playerAllyFilter) != 0), "allies should be in player's ally filter");
                Assert.IsFalse(enemies.All(enemy => (enemy.Flag & playerAllyFilter) != 0), "enemies should not be in player's ally filter");
            }

            {
                var playerAllyButSelfFilter = teamManager.GetFilterFor(player, Scope.AllyButSelf);
                Assert.IsFalse((player.Flag & playerAllyButSelfFilter) != 0, "player should not be in player's allybutself filter");
                Assert.IsTrue(allies.All(ally => (ally.Flag & playerAllyButSelfFilter) != 0), "allies should be in player's allybutself filter");
                Assert.IsFalse(enemies.All(enemy => (enemy.Flag & playerAllyButSelfFilter) != 0), "enemies should not be in player's allybutself filter");
            }

            {
                var playerAllFilter = teamManager.GetFilterFor(player, Scope.All);
                Assert.IsTrue((player.Flag & playerAllFilter) != 0, "player should be in all filter");
                Assert.IsTrue(enemies.All(enemy => (enemy.Flag & playerAllFilter) != 0), "enemy should be in all filter");
                Assert.IsTrue(allies.All(ally => (ally.Flag & playerAllFilter) != 0), "ally should be in all filter");
            }
        }
    }
}
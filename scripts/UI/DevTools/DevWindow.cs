namespace GameNamespace.UI.DevTools
{
    using Godot;

    public partial class DevWindow : Control
	{
        public SpawnBox spawnBox;
        public CombatLogConsole combatLog;

        public override void _Ready()
        {
            spawnBox = GetNode<SpawnBox>("SpawnBox");
            spawnBox.Visible = false;

            combatLog = GetNode<CombatLogConsole>("CombatLogConsole");
            combatLog.Visible = false;
        }

        public void ToggleCombatLog()
        {
            combatLog.Visible = !combatLog.Visible;
        }

        public void ToggleSpawnBox()
        {
            spawnBox.Visible = !spawnBox.Visible;
        }

    }

}

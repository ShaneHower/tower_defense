namespace GameNamespace.UI
{
    using GameNamespace.DataBase;
    using GameNamespace.GameAssets;
    using GameNamespace.GameManager;
    using Godot;

    public partial class DevWindow : Control
	{
        // Spawner
        public Panel spawner;
        public HBoxContainer enemySelect;
        public HBoxContainer hpEdit;
        public HBoxContainer speedEdit;
        public Button spawn;
        public Button exit;

        // Combat Log
        public Panel combatLog;
        public ScrollContainer scrollContainer;
        public VBoxContainer scrollVBox;

        public override void _Ready()
        {
            // Spawner
            spawner = GetNode<Panel>("Spawner");
            VBoxContainer parentVBox = spawner.GetNode<VBoxContainer>("VBoxContainer");

            enemySelect = parentVBox.GetNode<HBoxContainer>("enemySelect");
            hpEdit = parentVBox.GetNode<HBoxContainer>("HPEdit");
            speedEdit = parentVBox.GetNode<HBoxContainer>("SpeedEdit");
            HBoxContainer buttons = parentVBox.GetNode<HBoxContainer>("Buttons");

            spawn = buttons.GetNode<Button>("Spawn");
            spawn.Pressed += SpawnEnemy;

            exit = buttons.GetNode<Button>("Exit");
            exit.Pressed += TurnOff;

            // Combat Log
            combatLog = GetNode<Panel>("CombatLog");
            scrollContainer = combatLog.GetNode<ScrollContainer>("ScrollContainer");
            scrollVBox = scrollContainer.GetNode<VBoxContainer>("VBoxContainer");

            GameCoordinator.Instance.devWindow = this;
        }

        private EnemyData GetUIValues()
        {
            EnemyData enemyData = ReadEnemySelect();
            string selectedHP = ReadLineEdit(hpEdit);
            string selectedSpeed = ReadLineEdit(speedEdit);

            if(selectedHP != "")
            {
                enemyData.health = int.Parse(selectedHP);
            }

            if(selectedSpeed != "")
            {
                enemyData.speed = int.Parse(selectedSpeed);
            }

            return enemyData;
        }

        private EnemyData ReadEnemySelect()
        {
            OptionButton button = enemySelect.GetNode<OptionButton>("PickEnemy");
            int enemyId = button.GetItemId(button.Selected);
            return GameDataBase.Instance.QueryEnemyData(enemyId.ToString()).Clone();
        }

        private static string ReadLineEdit(HBoxContainer box)
        {
            LineEdit lineEdit = box.GetNode<LineEdit>("LineEdit");
            return lineEdit.Text;
        }

        private void SpawnEnemy()
		{
            EnemyData enemyData = GetUIValues();

            Path2D levelPath = GetTree().Root.GetNode<Path2D>("Level/LevelPath");
			PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.enemyPrefabLoc}/{enemyData.prefab}");
			PathFollow2D enemyPathFollow = (PathFollow2D) prefab.Instantiate();
            Enemy enemy = enemyPathFollow.GetNode<Enemy>(enemyData.name);
            enemy.passedData = enemyData;
            levelPath.AddChild(enemyPathFollow);

            // Pass data to the game coordinator
            GameCoordinator.Instance.activeEnemies.Add(enemy);
		}

        private void TurnOff()
        {
            Visible = false;
        }

        public void WriteCombatLog(string message)
        {
            Label label = new();
            label.Text = message;

            // Set font size, arduous I know.
            Theme theme = new();
            Font font = label.GetThemeDefaultFont();
            theme.SetFont("font", "Label", font);
            theme.SetFontSize("font_size", "Label", 8);
            label.Theme = theme;

            scrollVBox.AddChild(label);
            scrollContainer.ScrollVertical = (int)scrollContainer.GetVScrollBar().MaxValue;
        }
    }
}

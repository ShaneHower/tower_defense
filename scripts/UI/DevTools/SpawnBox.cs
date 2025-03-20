namespace GameNamespace.UI.DevTools
{
    using GameNamespace.DataBase;
    using GameNamespace.GameAssets;
    using GameNamespace.GameManager;
    using Godot;
    public partial class SpawnBox: Panel
    {

        public HBoxContainer enemySelect;
        public HBoxContainer hpEdit;
        public HBoxContainer speedEdit;
        public Button spawn;
        public Button exit;

        public override void _Ready()
        {
            VBoxContainer vBox = GetNode<VBoxContainer>("SpawnerVBox");

            // Get all components of spawner UI Box
            enemySelect = vBox.GetNode<HBoxContainer>("enemySelect");
            hpEdit = vBox.GetNode<HBoxContainer>("HPEdit");
            speedEdit = vBox.GetNode<HBoxContainer>("SpeedEdit");
            HBoxContainer buttons = vBox.GetNode<HBoxContainer>("Buttons");

            // Set buttons
            spawn = buttons.GetNode<Button>("Spawn");
            spawn.Pressed += SpawnEnemy;

            exit = buttons.GetNode<Button>("Exit");
            exit.Pressed += () => Visible = false;
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

    }
}

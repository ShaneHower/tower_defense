namespace GameNamespace.UI.DevTools
{
    using GameNamespace.DataBase;
    using GameNamespace.GameManager;
    using Godot;
    public partial class SpawnBox: Panel
    {
        public LevelPath levelPath;
        public HBoxContainer enemySelect;
        public HBoxContainer hpEdit;
        public HBoxContainer speedEdit;
        public Button spawn;
        public Button exit;

        public override void _Ready()
        {
            levelPath = GetTree().Root.GetNode<LevelPath>("Level/LevelPath");
            VBoxContainer vBox = GetNode<VBoxContainer>("SpawnerVBox");

            // Get all components of spawner UI Box
            enemySelect = vBox.GetNode<HBoxContainer>("enemySelect");
            hpEdit = vBox.GetNode<HBoxContainer>("HPEdit");
            speedEdit = vBox.GetNode<HBoxContainer>("SpeedEdit");
            HBoxContainer buttons = vBox.GetNode<HBoxContainer>("Buttons");

            // Set buttons
            spawn = buttons.GetNode<Button>("Spawn");
            spawn.Pressed += () => {
                EnemyData enemyData = GetUIValues();
                levelPath.SpawnEnemy(enemyId:enemyData.id, passedData:enemyData);
            };

            exit = buttons.GetNode<Button>("Exit");
            exit.Pressed += () => Visible = false;
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

namespace GameNamespace.GameManager
{
    using Godot;
    using GameNamespace.Enemies;

    public partial class PathIndex : Area2D
    {
        public string indexDirection;
        public bool active;
        public bool stopAnimation;

        public override void _Ready()
        {
            indexDirection = (string)GetMeta("direction");
        }

        private void OnEnter(Enemy enemy)
        {
            active = true;
            enemy.direction = indexDirection;

            if(Name == "End")
            {
                // Remove enemy from active enemies and store ending trigger
                GameCoordinator.Instance.enemyBreach = true;
                GameCoordinator.Instance.breachNum++;
                enemy.Destroy();
            }
        }

        // Called when another body exits the area
        private void OnExit(Enemy enemy)
        {
            active = false;
        }

    }
}

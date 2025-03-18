namespace GameNamespace.GameManager
{
    using Godot;
    using GameNamespace.GameAssets;
    using Serilog;

    /// <summary>
    /// The PathIndex object is responsible for signalling to the enemies when they need to change animations. They are
    /// little colliders which are placed on each indice of the enemy path.  Once an enemy collides with one of these
    /// indices, this object will send a message to the enemy signalling that a new animation direction should be
    /// triggered.
    /// We also use this object to know when an enemy has hit the end of the level, thus removing health from the player.
    /// </summary>
    public partial class PathIndex : Area2D
    {
        // Class vars
        public string indexDirection;
        public bool active;
        public bool stopAnimation;
        private static readonly ILogger log = Log.ForContext<PathIndex>();

        public override void _Ready()
        {
            // The direction is stored on the game object within godot.
            indexDirection = (string)GetMeta("direction");
            log.Information($"PathIndex {this} with direction {indexDirection} instatiated.");
        }

        private void OnEnter(Enemy enemy)
        {
            active = true;
            enemy.direction = indexDirection;
            log.Information($"Changing enemy {enemy} direction to {indexDirection}.");

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

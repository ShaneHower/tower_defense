namespace GameNamespace.GameManager
{
    using Godot;
    using System.Collections.Generic;

    /// <summary>
    /// The PathIndex object is responsible for signalling to the enemies when they need to change animations. They are
    /// little colliders which are placed on each indice of the enemy path.  Once an enemy collides with one of these
    /// indices, this object will send a message to the enemy signalling that a new animation direction should be
    /// triggered.
    /// We also use this object to know when an enemy has hit the end of the level, thus removing health from the player.
    /// </summary>
    public partial class LevelPath : Area2D
    {
        // Class vars
        public Dictionary<int, PathIndex> pathIndices = new();
        public CollisionPolygon2D collider;
        public Node2D spawn;

        public override void _Ready()
        {
            collider = GetNode<CollisionPolygon2D>("Collider");
            spawn = GetNode<Node2D>("Spawn");
        }

    }
}

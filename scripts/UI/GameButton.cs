namespace GameNamespace.UI
{
    using System;

    using GameNamespace.GameManager;
    using Godot;

    /// <summary>
	/// Governs the everything that the player can interact with.
	/// </summary>
    public partial class GameButton : TextureButton
	{
        public string type;
        public string spriteSheet;
        public Vector2 spriteDimensions;
        public Vector2 initPosition;
        public bool shifted;

        public void SetVars(string buttonType)
        {
            type = buttonType;
            if(type == "tower")
            {
                spriteDimensions = new Vector2(56, 80);
                spriteSheet = $"{GameCoordinator.Instance.uiSpriteLoc}/TowerButton-Sheet.png";
            }
            else if (type == "Menu")
            {
                spriteDimensions = new Vector2(120, 35);
                spriteSheet = $"{GameCoordinator.Instance.uiSpriteLoc}/Button-Sheet.png";
            }
        }

        public void ShiftPositionUp()
        {
            if(shifted) {return;}

            initPosition = Position;
            int shiftHalf = (int)spriteDimensions.Y / 2;
            Position = new Vector2(Position.X, Position.Y - shiftHalf);
            shifted = true;
        }

        public void ShiftPositionDown()
        {
            if(shifted) {return;}

            initPosition = Position;
            int shiftHalf = (int)spriteDimensions.Y / 2;
            Position = new Vector2(Position.X, Position.Y + shiftHalf);
            shifted = true;
        }

        public void ResetToInitPosition()
        {
            if (shifted)
            {
                Position = initPosition;
                shifted = false;
            }
            else
            {
                return;
            }

        }

    }
}

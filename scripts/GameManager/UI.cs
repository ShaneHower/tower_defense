using GameNamespace.GameManager;
using Godot;
using System;

public partial class UI : Node
{
	public string uiPrefabLoc = "res://prefabs/ui";

	public Button CreateButton(Control window, string buttonText)
	{
		// Get current visible screen size
		int screenWidth = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
		int screenHeight = (int)ProjectSettings.GetSetting("display/window/size/viewport_height");

		PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/button.tscn");
		Button button = (Button) prefab.Instantiate();
		window.AddChild(button);
		button.Text = buttonText;

		float xPos = ((screenWidth - button.Size.X) / 2) - 200;
		float yPos = screenHeight - button.Size.Y - 100;
		button.Position = new Vector2(xPos, yPos);

		return button;
	}
}

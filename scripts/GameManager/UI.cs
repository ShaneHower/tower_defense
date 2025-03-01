using GameNamespace.GameManager;
using Godot;
using System;

public partial class UI : Node
{
	public string uiPrefabLoc = "res://prefabs/ui";

	public Button CreateButton(Control window, string buttonText)
	{
		// Get size of window
		float windowWidth = window.Size.X;
		float windowHeight = window.Size.Y;

		PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/button.tscn");
		Button button = (Button) prefab.Instantiate();
		window.AddChild(button);
		button.Text = buttonText;

		float xPos = (windowWidth - button.Size.X) / 2;
		float yPos = (windowHeight - button.Size.Y) / 2;
		button.Position = new Vector2(xPos, yPos);

		return button;
	}
}

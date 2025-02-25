using GameNamespace.GameManager;
using Godot;
using System;

public partial class UI : Node
{
	public string uiPrefabLoc = "res://prefabs/ui";

	public Button CreateButton(Control window, string name)
	{
		GD.Print($"Fired {window} {name}");

		// Get current visible screen size
		int screenWidth = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
		int screenHeight = (int)ProjectSettings.GetSetting("display/window/size/viewport_height");

		GD.Print(screenWidth);
		GD.Print(screenHeight);

		string buttonName = $"{name}_button";
		PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/{buttonName}.tscn");
		Button button = (Button) prefab.Instantiate();
		window.AddChild(button);

		float xPos = (screenWidth- button.Size.X) / 2;
		float yPos = 200 - button.Size.Y;
		button.Position = new Vector2(xPos, yPos);

		return button;
	}
}

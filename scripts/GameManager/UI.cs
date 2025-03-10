using GameNamespace.GameManager;
using Godot;
using System;

public partial class UI : CanvasLayer
{
	public string uiPrefabLoc = "res://prefabs/ui";
	public Control levelState;
	public Control waveHud;

    public override void _Ready()
    {
		levelState = GetNode<Control>("LevelState");
		waveHud = GetNode<Control>("WaveHud");
    }

    public Button CreateWaveButton(string buttonText)
	{
		// Get size of window
		float windowWidth = waveHud.Size.X;
		float windowHeight = waveHud.Size.Y;

		PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/button.tscn");
		Button button = (Button) prefab.Instantiate();
		waveHud.AddChild(button);
		button.Text = buttonText;

		float xPos = (windowWidth - button.Size.X) / 2;
		float yPos = (windowHeight - button.Size.Y) / 2;
		button.Position = new Vector2(xPos, yPos);

		return button;
	}

	public void UpdateGoldValue(int amt)
	{
		Sprite2D gold = levelState.GetNode<Sprite2D>("Gold");
		Label goldValue = gold.GetNode<Label>("GoldValue");
		goldValue.Text = amt.ToString();
	}

	public void UpdateHealthValue(int amt)
	{
		Sprite2D health = levelState.GetNode<Sprite2D>("Health");
		Label healthValue = health.GetNode<Label>("HealthValue");
		healthValue.Text = amt.ToString();
	}

	public Label SpawnWarning()
	{
		PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/warning_label.tscn");
		Label warning = (Label)prefab.Instantiate();
		return warning;
	}
}

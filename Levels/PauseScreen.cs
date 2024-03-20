using Godot;
using System;
using System.Linq;

public partial class PauseScreen : Node2D
{
	private LevelManager levelManager;
	private CanvasLayer layer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		layer = GetChild<CanvasLayer>(0); // should only have one child
		layer.Visible = false;

		// get references
		levelManager = GetNode<LevelManager>("/root/LevelManager");

	}

    public override void _Process(double delta)
    {
		if (Input.IsActionJustPressed("pause"))
		{
			GetTree().Paused = true;
			layer.Visible = true;
		}
    }

	public void QuitGame()
	{
		GetTree().Paused = false; // Unpause
		GetTree().Quit();
	}

	public void ResumeGame()
	{
		layer.Visible = false;
		GetTree().Paused = false;
	}

	public void MenuButtonPressed()
	{
		GetTree().Paused = false; // Unpause
		levelManager.LoadLevel(0); // load the main menu at level index 0
	}

}

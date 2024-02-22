using Godot;
using System;

public partial class Sensor : Area2D
{
	private bool isActive = false;

	private int noPlayersOnSensor = 0;

	private LevelManager levelManager;
	private Color startColor;


	public override void _Ready()
	{
		// Get reference to the Level Manager
		levelManager = GetNode<LevelManager>("/root/LevelManager");
		startColor = Modulate;

		if (levelManager == null)
		{
			GD.Print("LevelManager not found. Make sure it's autoloaded and named correctly.");
		}
		else
		{
			GD.Print("LevelManager reference acquired successfully.");
		}
	}


	// Function to activate the sensor
	public void Activate(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return;

		if (noPlayersOnSensor == 0)
		{
			isActive = true;
			GD.Print("Sensor activated!");
			// Perform additional actions when activated, if needed

			// Call "SetSensorActive()" in the level manager
			levelManager.SetSensorActive();
		}

		// Adjust the alpha of the color rect
		Color activeColor = startColor;
		activeColor.A += 0.5f;
		Modulate = activeColor;
		
		noPlayersOnSensor++;
	}

	// Function to deactivate the sensor
	public void Deactivate(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return;

		noPlayersOnSensor--;

		if (noPlayersOnSensor <= 0 && isActive)
		{
			noPlayersOnSensor = 0;
			isActive = false;
			GD.Print("Sensor deactivated!");
			// Perform additional actions when deactivated, if needed

			// Call "SetSensorInactive()" in the level manager
			levelManager.SetSensorInactive();

			// Reset to the default color
			Modulate = startColor;
		}
	}
}

 

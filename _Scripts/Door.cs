using Godot;
using System;

public partial class Door : StaticBody2D
{
	private int activeSensorCount = 0;

	public void OpenDoor()
	{
		// Disable collision and visibility
		CollisionLayer = 0;
		ColorRect colorRect = GetNode<ColorRect>("ColorRect");
		colorRect.Visible = false;
	}

	public void CloseDoor()
	{
		// Enable collision and visibility
		CollisionLayer = 1;
		ColorRect colorRect = GetNode<ColorRect>("ColorRect");
		colorRect.Visible = true;
	}

	public void OnSensorActive(Node2D body)
	{
		activeSensorCount++;

		if(activeSensorCount == 1) // First sensor activated
		{
			OpenDoor();
		}
	}

	public void OnSensorInactive(Node2D body)
	{
		activeSensorCount--;

		if (activeSensorCount == 0) // Last sensor deactivated{
		{
			CloseDoor();
		}
	}
}

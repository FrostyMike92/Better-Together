using Godot;
using System;

public partial class DoorSensor : Area2D
{
	// This will probably change into a button in the future to open doors etc. 


	[Export] public StaticBody2D door;


	public void OpenDoor(Node2D body)
	{
		// Disable collision and visibility of the door
		door.CollisionLayer = 0;
		door.Visible = false;
	}

	public void CloseDoor(Node2D body)
	{
		door.CollisionLayer = 1;
		door.Visible = true;
	}
}

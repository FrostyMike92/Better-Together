using Godot;
using System;

public partial class WeightedPlatform : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public float gravity = -10000;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
	}

	// Player entered
	public void PlayerEntered(Node2D body)
	{
		if(body is PlayerMovement)
		{
			// reverse the gravity
			gravity = -gravity;

			GD.Print("Player Entered");
		}
	}

	// Player exited
	public void PlayerExited(Node2D body)
	{
		if(body is PlayerMovement)
		{
			gravity = -gravity;

			GD.Print("Player exited");
		}
	}
}

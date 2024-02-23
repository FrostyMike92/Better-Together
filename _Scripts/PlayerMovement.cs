using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public partial class PlayerMovement : CharacterBody2D
{
	[Export] public bool isPlayerTwo = false; // Default to player one

	public const float Speed = 600f;
	public const float JumpVelocity = -700.0f;
	public const float AirResistance = 0.75f; // Adjust as needed (% momentum preserved between frames)

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 spawnPos, deathPos, deathVelocity;


	public override void _Ready()
	{
		// Save the player starting location for respawn
		spawnPos = Position;
	}



	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		else
			velocity.X = 0; // reset the x (sideways) velocity when we hit the ground

		// Set the right player index for the input actions
		int playerIndex = isPlayerTwo ? 2 : 1;

		// Handle Jump.
		if (Input.IsActionJustPressed($"p{playerIndex}_jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector($"p{playerIndex}_left", $"p{playerIndex}_right", $"p{playerIndex}_up", $"p{playerIndex}_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			// Apply air resistance to preserve sideways momentum.
			velocity.X *= AirResistance;

			// Ensure that the player maintains some minimum speed.
			velocity.X = Mathf.Clamp(velocity.X, -Speed, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}


	// Called when the player dies
	public void KillPlayer(Node2D body)
	{
		// Save death position and velocity for the particles
		deathPos = Position;
		deathVelocity = Velocity;

		// Use call deffered to schedule the addition after the physics update
		CallDeferred("SpawnDeathBlocks");

		// Respawn the player (animations etc can happen here)
		Position = spawnPos;
	}

	private void SpawnDeathBlocks()
	{
		// Do animations and whatever else here
		// spawn the death blocks at the player position
		Node2D deathBlockInstance = (Node2D)deathBlock.Instantiate();
		GetParent().AddChild(deathBlockInstance);

		deathBlockInstance.Modulate = Modulate; // Set the color of the blocks
		deathBlockInstance.Position = deathPos;

		// Set the velocity of each child 
		foreach (RigidBody2D rb in deathBlockInstance.GetChildren())
		{
			rb.AngularVelocity = GD.Randf() * 02;
			rb.LinearVelocity = deathVelocity;
		}
	}
}

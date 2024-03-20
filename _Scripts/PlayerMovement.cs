using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public partial class PlayerMovement : CharacterBody2D
{
	[Export] public bool isPlayerTwo = false; // Default to player one
    [Export] public Color playerColor;
	[Export] public PackedScene deathBlock;
    [Export] public AudioStream jumpSound, deathSound;

	public const float Speed = 500f;
	public const float JumpVelocity = -700.0f;
	public const float AirResistance = 0.75f; // Adjust as needed (% momentum preserved between frames)

    public bool isStacked = false;
    public PlayerMovement stackedPlayer, parentPlayer;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Vector2 spawnPos, deathPos, deathVelocity;
    private float stackedDistance;

	private bool hasJump = false; // This never set true in this script, only set by others.

    private Timer cooldownTimer;
    private SoundManager soundManager;

    public override void _Ready()
	{
        // Get refenerences
        soundManager = GetNode<SoundManager>("/root/SoundManager");

		// Save the player starting location for respawn
		spawnPos = Position;

        // set sprite to the color of the player
        Sprite2D playerSprite = GetNode<Sprite2D>("Sprite2D");
        playerSprite.SelfModulate = playerColor;

        // Get the cooldown timer
        cooldownTimer = GetNode<Timer>("StackCooldown");
    }

    public override void _Process(double delta)
    {
        // if stacked we track the position manually
        if (isStacked)
        {
            Vector2 trackPosition = Position;
            
            // Update the tracked position based on the parent player's position
            trackPosition.X = parentPlayer.Position.X;
            trackPosition.Y = parentPlayer.Position.Y + stackedDistance; // add a small buffer to avoid collisions

            if (Mathf.Abs(trackPosition.X - Position.X) > 20) return; // dont set the position if we are too far

            Position = trackPosition;            
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Set the right player index for the input actions
        int playerIndex = isPlayerTwo ? 2 : 1;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }         
        else
        {
            velocity.X = 0; // reset the x (sideways) velocity when we hit the ground
            hasJump = false; // Disable double jump
        }

        // Handle Jump.
        if (Input.IsActionJustPressed($"p{playerIndex}_jump"))
        {
            UnstackPlayer(); // unstack when input pressed

            if (IsOnFloor() || hasJump)
            {
                velocity.Y = JumpVelocity;
                hasJump = false;
                soundManager.Play(0, soundManager, jumpSound);
            }
        }

        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector($"p{playerIndex}_left", $"p{playerIndex}_right", $"p{playerIndex}_up", $"p{playerIndex}_down");
        if (direction != Vector2.Zero)
        {
            UnstackPlayer(); // unstack when input pressed
            velocity.X = direction.X * Speed;
        }
        else
        {
            // Apply air resistance to preserve sideways momentum.
            velocity.X *= AirResistance;

            // Ensure that the player maintains some minimum speed.
            velocity.X = Mathf.Clamp(velocity.X, -Speed, Speed);
        }

        if (!isStacked) // we only apply physics velocities if we are not stacked
        {
            Velocity = velocity;
            MoveAndSlide();
        }        
    }

    // Called when the player dies
    public void KillPlayer(Node2D body)
	{
        if (stackedPlayer != null) stackedPlayer.UnstackPlayer(); // unstack any other stacked players
        UnstackPlayer(); // make sure the player is unstacked if we die        

        // Save death position and velocity for the particles
		deathPos = Position;
		deathVelocity = Velocity;

		// Use call deffered to schedule the addition after the physics update
		CallDeferred("SpawnDeathBlocks");
        CallDeferred("RespawnPlayer");

        // Play sound effects
        soundManager.Play(0, soundManager, deathSound, -8, 0.8f);
	}

    private void RespawnPlayer()
    {
        // Respawn the player (animations etc can happen here)
        Position = spawnPos;
        Velocity = Vector2.Zero;
    }

	private void SpawnDeathBlocks()
	{
		// Do animations and whatever else here
		// spawn the death blocks at the player position
		Node2D deathBlockInstance = (Node2D)deathBlock.Instantiate();
		GetParent().AddChild(deathBlockInstance);

		// deathBlockInstance.Modulate = Modulate;
		deathBlockInstance.Position = deathPos;

		// Set the velocity of each child 
		foreach (RigidBody2D rb in deathBlockInstance.GetChildren().OfType<RigidBody2D>())
		{
				rb.Modulate = playerColor; // Set the color of the individual blocks
                rb.AngularVelocity = GD.Randf() * 2;
                rb.LinearVelocity = deathVelocity + new Vector2(50, 50) * GD.RandRange(-1, 1);			
		}
	}

    // Tell the other player that they are now stacked
    public void StackPlayer(Node2D player)
    {
        if (player is PlayerMovement playerToStack && !ReferenceEquals(playerToStack, this))
        {
            playerToStack.SetPlayerStacked(this);
            stackedPlayer = playerToStack;
        }
        else
        {
            UnstackPlayer(); // if some other object, unstack this player
        }
    }

    // Unstack this player from the parent
    public void UnstackPlayer()
    {
        if(isStacked) // only start the cooldown if we were previously stacked
        {
            cooldownTimer.Start();
        }       

        isStacked = false;
        // Re-enable collision with other players
        SetCollisionLayerValue(4, true);      
    }

    // Set this player to be stacked
    public void SetPlayerStacked(Node2D otherPlayer)
    {
        if (cooldownTimer.TimeLeft > 0) return; // if timer still running do not stack

        parentPlayer = (PlayerMovement)otherPlayer;
        
        SetCollisionLayerValue(4, false); // Disable collisions with other players
        stackedDistance = Position.Y - parentPlayer.Position.Y; // Get distance between the players
        
        // set initial position
        Vector2 position = otherPlayer.Position;
        position.Y += stackedDistance;
        Position = position;

        isStacked = true;        
    }
}

using Godot;
using System;

public partial class MovingPlatform : Node2D
{
	private CharacterBody2D platform; // The static body for the platform
	private Vector2 startPos, endPos;
	private Timer stopTimer; // Timer controls how long the platform stops at each end

	[Export] public float stopTime = 2; // How long platform stops
	[Export] public float tweenTime = 1; // How long the platform takes to move

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get references
		platform = GetNode<CharacterBody2D>("Platform"); // Might not need this

		// Get start and end positions
		Node2D startTween = GetNode<Node2D>("Start Tween");
		Node2D endTween = GetNode<Node2D>("End Tween");
		startTween.Visible = false; // Hide Position helpers
		endTween.Visible = false;
		startPos = startTween.GlobalPosition;
		endPos = endTween.GlobalPosition;
		GlobalPosition = startPos; // Set the current pos to the start pos

		stopTimer = new Timer();
		AddChild(stopTimer);
		stopTimer.WaitTime = stopTime;
		stopTimer.OneShot = true; // Timer will auto-reset after the timeout
		stopTimer.Timeout += MovePlatform; // Connect timeout signal

		stopTimer.Start(); // Start the timer
	}

	public void MovePlatform()
	{
		// Set up the tweens
		Tween posTween = GetTree().CreateTween();
		posTween.SetProcessMode(Tween.TweenProcessMode.Physics);

		Vector2 tweenPos = GlobalPosition == startPos ? endPos : startPos; // Are we tweening to the start or finish location

		posTween.SetTrans(Tween.TransitionType.Linear); // experiment with different transisiton and easing
		posTween.SetEase(Tween.EaseType.In);
		posTween.TweenProperty(this, "position", tweenPos, tweenTime);
		posTween.StepFinished += OnTweenStep;
		posTween.Finished += StopPlatform;
	}

	public void StopPlatform()
	{
		// Start the stop timer
		stopTimer.Start();
	}

	public void OnTweenStep(long idx)
	{
		// Round the position value to integers
		Vector2 roundedPosition = new Vector2(Mathf.RoundToInt(Position.X), Mathf.RoundToInt(Position.Y));
		Position = roundedPosition;
	}
}

using Godot;
using System;

public partial class DisappearingPlatform : StaticBody2D
{
	private AnimationPlayer animPlayer;
	private Timer timer;

	[Export] float platformRestoreTimer = 3f;

    public override void _Ready()
    {
		// Get references
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// create a timer
		timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = platformRestoreTimer;
		timer.OneShot = true;

		timer.Timeout += OnTimerTimeout; // connect signal
    }

    public void PlayerEntered(Node2D body)
	{
        if (body is not PlayerMovement) return;
        if (timer.TimeLeft > 0) { return; }
		
		// play the disappear animation
		// create a timer
		// after timeout restore the platform
		timer.Start();
		animPlayer.SpeedScale = 2; // adjust this to make it harder
		animPlayer.Play("trigger");
	}

	public void OnTimerTimeout()
	{
		// Play animation to restore collision an visibility
		animPlayer.Play("restore");
	}
}

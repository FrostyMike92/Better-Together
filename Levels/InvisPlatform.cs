using Godot;
using Godot.NativeInterop;
using System;

public partial class InvisPlatform : Node2D
{
    [Export] float platformTimer = 1f; // How long before the platform is active
    [Export] float tweenTimer = 0.2f; // How long to tween for (in and out)
    [Export] AudioStream triggerSound, resetSound;

    private StaticBody2D platform;
    private Area2D area;
    private Vector2 platformPos;
    private Timer resetTimer;
    private SoundManager soundManager;

    private bool isPlatformEnabled;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get References
        platform = GetNode<StaticBody2D>("Platform");
        area = GetNode<Area2D>("DetectionArea");
        soundManager = GetNode<SoundManager>("/root/SoundManager");

        resetTimer = new Timer();
        AddChild(resetTimer);
        resetTimer.WaitTime = platformTimer; // Set the delay time in seconds
        resetTimer.OneShot = true; // The timer will auto-reset after the timeout

        // Connect the timeout signal to the method that will reset the platform
        resetTimer.Connect("timeout", new Callable(this, MethodName.ResetPlatform));

        // Set all the platform variables to the reset position
        platformPos = platform.Position; // save position for tweening later
        platform.Position = area.Position; // move the platform to the detection position
        platform.Modulate = new Color(0, 0, 0, 0); // make platform invisible
        platform.CollisionLayer = 0; // disable the collision
    }

    public void TriggerPlatform(Node2D body)
    {
        // Conditions
        if (!body.IsInGroup("player")) return;
        if (isPlatformEnabled) return;

        isPlatformEnabled = true;

        // set up all the tweens
        Tween posTween = GetTree().CreateTween();
        posTween.SetTrans(Tween.TransitionType.Back); // experiment with transition and easing
        posTween.SetEase(Tween.EaseType.Out);
        posTween.TweenProperty(platform, "position", platformPos, tweenTimer);
        posTween.Connect("finished", new Callable(this, MethodName.EnablePlatform)); // Enable platform when the tween is done

        Tween colorTween = GetTree().CreateTween();
        colorTween.TweenProperty(platform, "modulate", new Color(0, 0, 0, 1), tweenTimer);

        // Play sound for triggering platform
        soundManager.Play(0, soundManager, triggerSound);
    }

    private void ResetPlatform()
    {
        platform.CollisionLayer = 0; // disable collision

        // Set up tweens
        Tween posTween = GetTree().CreateTween();
        Tween colorTween = GetTree().CreateTween();
        posTween.SetTrans(Tween.TransitionType.Expo); // experiment with transition and easing
        posTween.SetEase(Tween.EaseType.In);
        posTween.TweenProperty(platform, "position", area.Position, tweenTimer);
        colorTween.TweenProperty(platform, "modulate", new Color(0, 0, 0, 0), tweenTimer);

        isPlatformEnabled = false;

        soundManager.Play(0, soundManager, resetSound);
    }

    private void EnablePlatform()
    {
        platform.CollisionLayer = 1; // enable collision

        // Start the resetTimer
        resetTimer.Start();
    }
}


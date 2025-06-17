using Godot;
using System;

public partial class killzone : Area2D
{
	private Timer timer;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTimeout; // on connecte le signal Timeout
	}

	private void OnBodyEntered(Node2D body)
	{
		GD.Print("You died");
		Engine.TimeScale = 0.5;
		body.GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
		timer.Start(); // démarrer le compte à rebours
	}

	private void OnTimerTimeout()
	{
		Engine.TimeScale = 1.0;
		GetTree().ReloadCurrentScene(); // redémarre la scène
	}
}

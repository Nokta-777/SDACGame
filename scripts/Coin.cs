using Godot;
using System;

public partial class Coin : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Connect("body_entered", new Callable(this, nameof(_on_body_entered)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(Node body)
	{
		if (body is Player_mov player)
		{
			player.n_coins++;
			GD.Print($"+1 coin ! Total: {player.n_coins}");
			this.QueueFree();
		}
	}

}

using Godot;
using System;

public partial class Option : Node3D
{
	[Export]
	public Node3D target;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position=Position+(target.Position-Position)*0.4f;
	}
}

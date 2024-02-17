using Godot;
using System;

public partial class MoveTest : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private int i=0;
	[Export]
	public Vector3 pos;
	[Export]
	public int life;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		i++;
		if(i>life)
		{
			QueueFree();
		}
		Translate(pos);
		//GD.Print(GlobalPosition+GetChild<SimpleCollider>(1).points[0],SimpleCollider.DistancePoint(GlobalPosition+GetChild<SimpleCollider>(1).points[0],Vector3.Zero,Vector3.Up));
	}
}

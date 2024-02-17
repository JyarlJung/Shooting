using Godot;
using System;

public partial class Eraser : Node3D
{
	[Export]
	public SimpleCollider collider;
	private float radius;
	private int time, timeSet;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(time>timeSet)QueueFree();
		time++;
		collider.Width=radius * (float)time/timeSet;
	}
	public void CreateEraser(Vector3 pos,float rad, int time)
	{
		GlobalPosition=pos;
		collider.Width=0;
		radius=rad;
		this.time=0;
		timeSet=time;
	}
}

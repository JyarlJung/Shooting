using Godot;
using System;

public partial class BgMove : Node3D
{
	[Export]
	public float speed=1;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 moveVec=Position+(Vector3.Back * speed * Global.FrameDelta);
		if(moveVec.Z>8)moveVec.Z-=8.0f;
		Position=moveVec;
	}
}

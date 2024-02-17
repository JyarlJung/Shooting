using Godot;
using System;

public partial class Item : Node3D
{
	private int score=1;
	private int time = 0;
	private float speed;
	[Export]
	public PooledNode poolednode;
	private Node3D cha;
	private float rot;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position=Position+(Global.MoveVector(rot)* speed * Global.FrameDelta);
		if(time<40 && speed>0)
		{
			speed-=0.02f;
		}
		else if(time>=40)
		{
			rot=Global.GetArrow(GlobalPosition,cha.Position).Z;
			speed=4f;
		}
		if(GlobalPosition.DistanceTo(cha.Position)<0.2f && time>30)
		{
			Global.UiNode.score+=score;
			QueueFree();
		}
		time++;
	}
	public void CreateItem(Vector3 pos, float rot, Node3D ch, int sc=0,float speed=1.4f)
	{
		Position=pos;
		this.rot=rot;
		time=0;
		score=sc;
		this.speed=speed;
		cha=ch;
		Translate(Vector3.Right * 0.1f);
	}
}

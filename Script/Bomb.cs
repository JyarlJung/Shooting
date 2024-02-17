using Godot;
using System;

public partial class Bomb : Node3D
{
	[Export]
	private AnimationPlayer anime;
	[Export]
	private Ui ui;
	[Export]
	private SimpleCollider collider;
	private float speed=0;
	private Node3D enemy;
	// Called when the node enters the scene tree for the first time.

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(speed>0)
		{
			speed-=0.002f;
			ui.ShakeCamera((1.2f-speed)*0.1f);
		}
		enemy=(Node3D)GetTree().GetFirstNodeInGroup("Enemy");
		if(enemy!=null)
		{
			Vector3 arrow = (enemy.GlobalPosition-GlobalPosition).Normalized();
			Position=Position+(arrow*speed*Global.FrameDelta);
			Collision coll =collider.HitTestFirst();
			if(coll!=null)
			{
				if(coll.Target.RootNode is Boss boss)
				{
					boss.Hit(1,true);
				}
			}
		}
	}
	public bool StartBomb(Vector3 pos)
	{
		if(anime.CurrentAnimation!="Start")
		{
			speed=0.5f;
			Position=pos;
			anime.Play("Start");
			return true;
		}
		return false;
	}
}

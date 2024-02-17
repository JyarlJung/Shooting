using Godot;
using System;

public partial class SimpleTanmak : Tanmak
{
	private bool grazeable;
	[Export]
	AnimationPlayer anime;
	private bool isLive=true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		grazeable=true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		Move();
		if(isLive)
		{
			foreach(Collision col in collider.HitTestAll())
			{
				if(col.Target.InLayer(5))
				{
					DeleteTanmak();
				}
				if(col.Target.InLayer(0))
				{
					if(cha.hitShape.Equals(col.Target))
					{
						cha.Hit();
						if(Eraseable)EraseTanmak();
					}
					if(cha.grazeShape.Equals(col.Target) && grazeable)
					{
						cha.Graze(GlobalPosition,true);
						grazeable=false;
					}
				}
				if(col.Target.InLayer(3) && Eraseable)
				{
					EraseTanmak();
					cha.Graze(collider.GlobalPosition+collider.GetPos(col.Index),false);
				}
			}
		}
	}
	public override void CreateTanmak(Vector3 pos, Color color, float rot = 0, float sp = 1, bool add = false)
	{
		base.CreateTanmak(pos, color, rot, sp, add);
		anime.Play("Start");
		isLive=false;
	}
	
	public void EraseTanmak()
	{
		anime.Play("Delete");
		isLive=false;
	}
	public void AnimeFinished(string name)
	{
		if(name=="Start")
		{
			isLive=true;
			anime.Play("Idle");
		}
		if(name=="Delete")DeleteTanmak();
	}
}

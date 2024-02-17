using Godot;
using Godot.Collections;
using System;

public partial class Cha : Node3D
{
	[Export]
	public float speed=3f;
	[Export]
	public float slowSpeed=1.5f;
	[Export]
	public AnimationPlayer anime;
	[Export]
	public AudioStreamPlayer audioPlayer, grazeAudio;
	[Export]
	public SimpleCollider hitShape, grazeShape;
	[Export]
	private Bomb bomb;
	private bool moveable=true;
	public bool bombable=true;
	private int invisible= 0;
	private int time=0;
	private Vector3 arrow;


	public override void _Ready()
	{
		anime.Play("invisible");
		time=0;
		invisible=20;
	}
	public override void _Process(double delta)
	{
		if(moveable)
		{
			arrow = Vector3.Zero;
			arrow.Y = Input.IsActionPressed("ui_up") ? 1.0f : arrow.Y;
			arrow.Y = Input.IsActionPressed("ui_down") ? -1.0f : arrow.Y;
			arrow.X = Input.IsActionPressed("ui_right") ? 1.0f : arrow.X;
			arrow.X = Input.IsActionPressed("ui_left") ? -1.0f : arrow.X;
			arrow=arrow.Normalized();

			if(!Input.IsKeyPressed(Key.Shift))
			{
				
				Translate(arrow * speed * Global.FrameDelta);
			}
			else
			{
				Translate(arrow * slowSpeed * Global.FrameDelta);
			}
		}
		if(bombable)
		{
			if(Input.IsKeyPressed(Key.X) && Global.UiNode.bomb>=1 && bomb.StartBomb(GlobalPosition))
			{
				invisible=270;
				anime.Play("RESET");
				Global.UiNode.bomb--;
				moveable=true;
				bombable=true;
			}
		}
		ShotProcess();

		Vector3 pos = Position;
		pos.X=Mathf.Clamp(pos.X,Global.Box.X,Global.Box.Z);
		pos.Y=Mathf.Clamp(pos.Y,Global.Box.Y,Global.Box.W);
		Position=pos;

		if(String.Compare(anime.CurrentAnimation,"idle")==0)
		{
			moveable=true;
		}
		if(invisible>0)
		{
			invisible--;
			if(anime.CurrentAnimation=="idle")anime.Play("invisible");
		}
		else if(invisible==0)
		{
			invisible=-1;
			anime.Play("idle");
		}
		time++;
	}
	public void Resurrection()
	{
		Position=new Vector3(-1f,-2f,0f);
		moveable=true;
		bombable=true;
		anime.Play("idle");
		Global.UiNode.hp--;
		Global.UiNode.bomb+=0.5f;
	}
	private void CreateShot(Vector3 pos, float rot, float sp, String type, int cooltime=3, String group=null)
	{
		if(time%cooltime==0)
		{
			Bullet bullet = (Bullet)Global.Container.CreateNode(type);
			bullet.CreateBullet(Position+pos,rot,sp);
			if(group!=null)bullet.AddToGroup(group);
		}
	}
	private void ShotProcess()
	{
		if(Input.IsKeyPressed(Key.Z) && moveable)
		{
			SoundManager.OverPlay(audioPlayer,0.39f,null,true);
			CreateShot(new Vector3(0.08f,0.2f,0f),90f,20f,"Shot1");
			CreateShot(new Vector3(-0.08f,0.2f,0f),90f,20f,"Shot1");
			if(!Input.IsKeyPressed(Key.Shift))
			{
				CreateShot(new Vector3(0.2f,0.2f,0f),84f,7f,"Shot2",4);
				CreateShot(new Vector3(-0.2f,0.2f,0f),96f,7f,"Shot2",4);
				CreateShot(new Vector3(0.25f,0.2f,0f),77f,7f,"Shot2",6);
				CreateShot(new Vector3(-0.25f,0.2f,0f),103f,7f,"Shot2",6);
				CreateShot(new Vector3(0.3f,0.2f,0f),72f,7f,"Shot2",8);
				CreateShot(new Vector3(-0.3f,0.2f,0f),108f,7f,"Shot2",8);
			}
			else
			{
				CreateShot(new Vector3(0.2f,0.2f,0f),30f,9f,"Shot2",6,"ShotType1");
				CreateShot(new Vector3(-0.2f,0.2f,0f),150f,9f,"Shot2",6,"ShotType2");
				CreateShot(new Vector3(0.25f,0.2f,0f),40f,11f,"Shot2",9,"ShotType1");	
				CreateShot(new Vector3(-0.25f,0.2f,0f),140f,10f,"Shot2",9,"ShotType2");				
			}
		}

		foreach(Bullet bullet in GetTree().GetNodesInGroup("ShotType1"))
		{
			bullet.RotateZ(Global.GetNegPos(bullet.time%14-7)*-0.32f);
		}
		foreach(Bullet bullet in GetTree().GetNodesInGroup("ShotType2"))
		{
			bullet.RotateZ(Global.GetNegPos(bullet.time%14-7)*0.32f);
		}
	}
	public void Hit()
	{
		if(invisible<=0)
		{
			anime.Play("die");
			moveable=false;
			invisible=240;
			Eraser eraser = (Eraser)Global.Container.CreateNode("Eraser");
			eraser.CreateEraser(Position,1,50);
			Global.Sound.Play("Die");
		}
	}
	public void Graze(Vector3 pos, bool gz)
	{
		if(invisible<60 && gz)
		{
			Item item=(Item)Global.Container.CreateNode("Item");
			item.CreateItem(pos,Global.GetArrow(Position, pos).Z+GD.Randf()*40f,this,10);
			Global.UiNode.Graze();
			grazeAudio.Play();
		}
		if(!gz)
		{
			Item item=(Item)Global.Container.CreateNode("Item");
			item.CreateItem(pos,GD.Randf()*60f+60f,this,10,GD.Randf()*0.6f+0.3f);
		}
	}
	public void SetNotBomb()
	{
		bombable=false;
	}
}

using Godot;
using Godot.Collections;
using System;

public partial class Boss : Node3D
{
	[Export]
	public float hp=2000;
	[Export]
	public CircleBar hpbar;
	[Export]
	public Node3D pin;
	[Export]
	public AnimationPlayer spellbg;
	[Export]
	public SimpleCollider collider;
	[Export]
	public Array<Vector4> patternInfo;
	[Export]
	public Array<Vector3> posInfo;
	[Export]
	public Array<String> nameInfo;
	[Export]
	public AudioStreamPlayer audioPlayer;
	[Export]
	public AudioStreamPlayer tanAudio;
	private int time=0;
	public AnimationPlayer anime;
	private Cha cha;
	public Vector3 moveVec;
	public float def=1.0f;
	private int pat=-1;
	private Array<float> vars=new Array<float>();
	public override void _Ready()
	{
		anime=GetChild(2).GetChild<AnimationPlayer>(0);
		moveVec=Position;
		cha = (Cha)GetTree().GetFirstNodeInGroup("Player");
		for(int i=0;i<10;i++)
		{
			vars.Add(0);
		}
	}
	public override void _Process(double delta)
	{
		time++;
		hpbar.value=(hp%1001f)/1000f;
		Move();
		if(pat==patternInfo.Count-1)
		{
			if(hp<=0 && def!=0)
			{
				time=-80;
				def=0;
				Global.Sound.Play("Enep");
				Eraser eraser = (Eraser)Global.Container.CreateNode("Eraser");
				eraser.CreateEraser(Position,7,140);
			}
		}
		else if(pat<patternInfo.Count-1)
		{
			if(Global.UiNode.startTime<0)hp=patternInfo[pat+1].X;
			pin.Position=(Global.MoveVector(90f+(patternInfo[pat+1].X%1001f)*0.36f)) * hpbar.radius;
			if(patternInfo[pat+1].X>=hp && time>0)
			{
				EndPattern(pat);
				pat++;
				StartPattern(pat);
			}
		}
		foreach(Collision col in collider.HitTestAll())
		{
			if(col.Target.InLayer(0))
			{
				if(cha.hitShape.Equals(col.Target))
				{
					cha.Hit();
				}
			}
		}
		if(time>=0 || pat==patternInfo.Count-1)
		{
			switch (pat)
			{
				case 0:
					Pat0();
				break;

				case 1:
					Pat1();
				break;

				case 2:
					Pat2();
				break;

				case 3:
					Pat3();
				break;

				case 5:
					Pat5();
				break;
			}
		}
	}
	private void Pat0()
	{
		if(time==0)
		{
			anime.Play("Start");
			vars[0]=2.5f;
		}		
		if(time%45==20)
		{
			SoundManager.OverPlayName(tanAudio,0.0f,"Tan1");
			vars[0] *= -1f;
			Tanmak tanmak;
			float rot=Global.GetArrow(Position,cha.Position).Z;
			Color color=Colors.Blue;
			for(int i=0 ;i<30;i++)
			{
				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot+vars[0]*2,1.5f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot+vars[0],1.25f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot,1f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot-vars[0],0.75f);
			}
		}
		if(time%150==110)
		{
			moveVec+= (GD.Randf()*0.3f+0.3f) * ((moveVec.X>-0.7) ? Vector3.Left : Vector3.Right);
			moveVec+= (GD.Randf()*0.2f+0.2f) * ((moveVec.Y>0.8) ? Vector3.Down : Vector3.Up);
		}
	}
	private void Pat1()
	{
		if(time==0)vars[0]=7;
		if(time==300)vars[0]=6;
		if(time==550)vars[0]=5;
		if(time==850)vars[0]=4;
		if(time==1100)vars[0]=3;
		if(time%vars[0]==0)
		{
			Tanmak tanmak;
			SoundManager.OverPlayName(tanAudio,0.0f,"Tan1");
			Vector3 pos = new Vector3();
			pos.X=Global.Box.X-0.15f;
			pos.Y=Global.Box.Y-0.15f + GD.Randf()*(cha.Position.Y-Global.Box.Y+0.1f);
			tanmak=(Tanmak)Global.Container.CreateNode("Tanmak1_ADD");
			tanmak.CreateTanmak(pos,Colors.Yellow,0,GD.Randf()*0.3f+0.2f);
			pos = cha.GlobalPosition;
			pos.X=Global.Box.Z+0.15f;
			pos.Y=Global.Box.Y-0.15f + GD.Randf()*(cha.Position.Y-Global.Box.Y+0.1f);	
			tanmak=(Tanmak)Global.Container.CreateNode("Tanmak1_ADD");
			tanmak.CreateTanmak(pos,Colors.Yellow,180,GD.Randf()*0.3f+0.2f);
		}
		
		if(time%360==220)
		{
			Node3D node = Global.Container.CreateNode("Charge");
			Global.Sound.Play("Charge");
			node.GlobalPosition=cha.Position;
		}
		if(time%360==280)
		{
			anime.Play("Pop");
			Global.Sound.Play("Tan1");
			foreach(SimpleTanmak tanmak in GetTree().GetNodesInGroup("Tanmak"))
			{
				if(Mathf.Abs(tanmak.GlobalPosition.X-cha.GlobalPosition.X)<=0.6f && !tanmak.IsInGroup("Type1"))
				{
					tanmak.AddToGroup("Type1");
					tanmak.vars[0]=tanmak.speed + Mathf.Max(0.7f-(tanmak.GlobalPosition-cha.GlobalPosition).Length(),0.0f);
					tanmak.vars[1]=tanmak.GlobalPosition.X;
					tanmak.speed=0f;
					tanmak.rot=90.0f;
					tanmak.time=0;
				}
			}
		}
		foreach(SimpleTanmak tanmak in GetTree().GetNodesInGroup("Type1"))
		{

			if(tanmak.time<30)
			{
				Vector3 pos= tanmak.GlobalPosition;
				pos.X+=(GD.Randf()*0.06f-0.03f)*(30.0f-tanmak.time)*0.05f;
				tanmak.GlobalPosition=pos;
			}
			if(tanmak.time<100)	tanmak.speed+=tanmak.vars[0]*0.01f;
			if(tanmak.time>100 && tanmak.speed>-1.0f)tanmak.speed-=0.01f;
		}
	}
	private void Pat2()
	{	
		if(time%30==20)
		{
			SoundManager.OverPlayName(tanAudio,0.0f,"Tan1");
			Tanmak tanmak;
			float rot=Global.GetArrow(Position,cha.Position).Z;
			Color color=Colors.Blue;
			for(int i=0 ;i<30;i++)
			{
				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot,1.75f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot,1.5f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot,1.25f);

				tanmak=(Tanmak)Global.Container.CreateNode("Tanmak3");
				tanmak.CreateTanmak(Position,color,i*12f+rot,1.0f);
			}
		}
		if(time%150==110)
		{
			moveVec+= (GD.Randf()*0.3f+0.3f) * ((moveVec.X>-0.7) ? Vector3.Left : Vector3.Right);
			moveVec+= (GD.Randf()*0.2f+0.2f) * ((moveVec.Y>0.8) ? Vector3.Down : Vector3.Up);
		}
	}
	private void Pat3()
	{
		if(time==0)vars[0]=4;
		if(time%640<120)
		{
			if(time%30==20)
			{
				SoundManager.OverPlayName(tanAudio,0.0f,"Tan1");
				SimpleTanmak tanmak;
				float rot=GD.Randf()*15+37.5f;
				float sp=GD.Randf()*0.7f+0.7f;
				Color color=Colors.Blue;
				for(int i=0 ;i<4;i++)
				{
					tanmak=(SimpleTanmak)Global.Container.CreateNode("Tanmak1");
					tanmak.CreateTanmak(Position,color,i*90f+rot,sp);
					tanmak.AddToGroup("Type1");
					tanmak.Eraseable=false;
				}
			}
			if(time%60==20)
			{
				moveVec+= (GD.Randf()*0.3f+0.4f) * ((moveVec.X>-0.7) ? Vector3.Left : Vector3.Right);
				moveVec+= (GD.Randf()*0.3f+0.4f) * ((moveVec.Y>0.4) ? Vector3.Down : Vector3.Up);
			}
		}
		if(time%640==180)
		{
			Node3D node = Global.Container.CreateNode("Charge");
			Global.Sound.Play("Charge");
			node.GlobalPosition=GlobalPosition;
		}
		if(time%640==240)
		{
			anime.Play("Pop");
			Global.Sound.Play("Laser1");
			TrailLaser tanmak;
			for(int i=0 ;i<4;i++)
			{
				tanmak=(TrailLaser)Global.Container.CreateNode("Laser");
				tanmak.CreateTanmak(Position,Colors.Green,0,2.3f);
				tanmak.AddToGroup("Type3");
				tanmak.vars[0]=i;
				tanmak.Eraseable=false;
			}
		}
		foreach(SimpleTanmak tanmak in GetTree().GetNodesInGroup("Type1"))
		{
			if(tanmak.time<40)
			{
				if(tanmak.GlobalPosition.X<Global.Box.X+0.15f || tanmak.GlobalPosition.X>Global.Box.Z-0.15f)tanmak.time=40;
				if(tanmak.GlobalPosition.Y<-0.4f || tanmak.GlobalPosition.Y>Global.Box.W-0.15f)tanmak.time=40;
			}
			if(tanmak.time==40)
			{
				tanmak.EraseTanmak();
				SimpleTanmak temp =(SimpleTanmak)Global.Container.CreateNode("Tanmak4");
				temp.CreateTanmak(tanmak.GlobalPosition,Colors.Blue,0,0.0f);
				temp.AddToGroup("Type2");
				temp.Eraseable=false;
			}
		}
		foreach(TrailLaser tanmak in GetTree().GetNodesInGroup("Type3"))
		{
			if(tanmak.time==1)
			{
				tanmak.varsTanmak=(SimpleTanmak)GetTree().GetFirstNodeInGroup("Type2");
				if(tanmak.varsTanmak!=null)
				{
					tanmak.rot=Global.GetArrow(tanmak.GlobalPosition,tanmak.varsTanmak.GlobalPosition).Z;
					tanmak.varsTanmak.RemoveFromGroup("Type2");
				}
			}
			if(tanmak.varsTanmak!=null)
			{
				SimpleTanmak target = (SimpleTanmak)tanmak.varsTanmak;
				if((target.GlobalPosition-tanmak.GlobalPosition).Length()<0.3f)
				{
					target.EraseTanmak();
					SoundManager.OverPlayName(tanAudio,0.0f,"Tan1");
					Tanmak temp;
					float rot=Global.GetArrow(Position,cha.Position).Z;
					Color color=Colors.Blue;
					for(int i=0 ;i<20;i++)
					{
						temp=(Tanmak)Global.Container.CreateNode("Tanmak2");
						temp.CreateTanmak(target.Position,color,i*18f+rot,0.6f);
						temp=(Tanmak)Global.Container.CreateNode("Tanmak2");
						temp.CreateTanmak(target.Position,color,i*18f+rot,0.8f);

					}
					tanmak.varsTanmak=null;
				}
			}
			if(tanmak.varsTanmak==null)
			{
				if(tanmak.GlobalPosition.X<Global.Box.X-0.1f || tanmak.GlobalPosition.X>Global.Box.Z+0.1f)tanmak.time=0;
				if(tanmak.GlobalPosition.Y<Global.Box.Y-0.1f || tanmak.GlobalPosition.Y>Global.Box.W+0.1f)tanmak.time=0;
			}
		}
	}
	private void Pat5()
	{
		if(time==0)
		{
			vars[0]=90;
			time=60;
			if(hp<=0)
			{
				Global.Sound.Play("Enep");
				EndPattern(pat);
				QueueFree();
				return;
			}
		}
		if(hp<800 && vars[0]==90)
		{
			anime.Play("Pop");
			Global.Sound.Play("Tan1");
			vars[0] = 75;
			time=1;
		}
		if(hp<550 && vars[0]==75)
		{
			anime.Play("Pop");
			Global.Sound.Play("Tan1");
			vars[0] = 60;
			time=1;
		}
		if(hp<350 && vars[0]==60)
		{
			anime.Play("Pop");
			Global.Sound.Play("Tan1");
			vars[0] = 50;
			time=1;
		}
		if(hp<150 && vars[0]==50)
		{
			anime.Play("Pop");
			Global.Sound.Play("Tan1");
			vars[0] = 40;
			time=1;
		}
		if(time%vars[0]==vars[0]-3)
		{
			TrailLaser tanmak;
			float rot=GD.Randf()*9;
			SoundManager.OverPlayName(tanAudio,0f,"Laser1");
			vars[1]=Mathf.Clamp(vars[0]-25,20,50);
			for(int i=0 ;i<40;i++)
			{
				tanmak=(TrailLaser)Global.Container.CreateNode("Laser");
				tanmak.CreateTanmak(Position,Colors.Yellow,i*9f+rot,2.2f,(int)vars[1]);
				tanmak.AddToGroup("Type1");
			}
			rot=GD.Randf()*9;
			for(int i=0 ;i<40;i++)
			{
				tanmak=(TrailLaser)Global.Container.CreateNode("Laser");
				tanmak.CreateTanmak(Position,Colors.Green,i*9f+rot,2.2f,(int)vars[1]);
				tanmak.AddToGroup("Type2");
			}
		}
		foreach(TrailLaser tanmak in GetTree().GetNodesInGroup("Type1"))
		{
			if(tanmak.time<65 && tanmak.time>25)
			{
				tanmak.speed-=0.045f;
				tanmak.rot+=1.5f;
			}
			tanmak.rot+=Global.GetNegPos(tanmak.time%40-20)*3f;
		}
		foreach(TrailLaser tanmak in GetTree().GetNodesInGroup("Type2"))
		{
			if(tanmak.time<65 && tanmak.time>25)
			{
				tanmak.speed-=0.045f;
				tanmak.rot-=1.5f;
			}
			tanmak.rot-=Global.GetNegPos(tanmak.time%40-20)*3f;
		}
	}
	public void Move(bool clamp=true)
	{
		if(clamp)
		{
			moveVec.X=Mathf.Clamp(moveVec.X,Global.Box.X,Global.Box.Z);
			moveVec.Y=Mathf.Clamp(moveVec.Y,0.0f,Global.Box.W);
		}
		Vector3 temp=(moveVec-Position)*0.05f;
		if(temp.Length()>0.05f)temp=temp.Normalized()*0.05f;
		Position+=temp;
	}
	public void StartPattern(int ind)
	{
		time=-70;
		if(posInfo[ind]==Vector3.Zero)
		{
			moveVec=new Vector3(-0.7f,0.8f,0.0f);
		}
		else
		{
			moveVec=posInfo[ind];
		}
		if(patternInfo[ind].Z==1.0f)
		{
			spellbg.Play("Start");
		}
		def=patternInfo[ind].Y;
		Global.UiNode.SpellStart(nameInfo[ind],patternInfo[ind]);
	}
	public void EndPattern(int ind)
	{
		if(ind==-1)return;
		if(patternInfo[ind].Z==1.0f)
		{
			spellbg.Play("RESET");
			Global.UiNode.SpellEnd();
			Global.Sound.Play("Get");
		}
		foreach(Tanmak tanmak in GetTree().GetNodesInGroup("Tanmak"))
		{
			tanmak.Eraseable=true;
		}
		Eraser eraser = (Eraser)Global.Container.CreateNode("Eraser");
		if(patternInfo[ind].Z==1.0f)
		{
			spellbg.Play("RESET");
			Global.UiNode.SpellEnd();
			Global.Sound.Play("Get");
			eraser.CreateEraser(Position,5,20);
		}
		else
		{
			eraser.CreateEraser(Position,5,70);
		}
	}
	public void Hit(float dmg, bool isBomb)
	{
		if(def<=0 || time<=0)return;
		hp-=def*dmg;
		float nexthp = (pat==patternInfo.Count-1) ? 0:patternInfo[pat+1].X;
		AudioStream hitsound=((hp-nexthp)*(1.0/def)<120) ? Global.Sound.GetSound("Hit2") : Global.Sound.GetSound("Hit1");
		SoundManager.OverPlay(audioPlayer,0.14f,hitsound);
	}
}

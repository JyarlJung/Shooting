using Godot;
using Godot.Collections;
using System;

public partial class Tanmak : Node3D
{
	[Export]
	protected GeometryInstance3D graphic;
	[Export]
	public bool useRotation;
	[Export]
	public int lifetime=500;
	[Export]
	public SimpleCollider collider;
	public float rot=0, speed =1;
	public int time=0;
	private Color color;
	protected Cha cha;
	public Array<float> vars = new Array<float>{0f,0f,0f,0f,0f};
	public Tanmak varsTanmak=null;
	public bool Eraseable=true;
	public override void _Ready()
	{
		cha=(Cha)GetTree().GetFirstNodeInGroup("Player");
	}
	public override void _Process(double delta)
	{
		if(useRotation)graphic.RotationDegrees=Vector3.Back*rot;
		if(time >= lifetime)
		{
			time=0;
			DeleteTanmak();
		}
		time++;
	}
	public virtual void Move(float scale =1.0f)
	{
		Position=Position+(Global.MoveVector(rot)* speed * Global.FrameDelta * scale);
		if(useRotation)
		{
			graphic.RotationDegrees=Vector3.Back*rot;
		}
	}
	public virtual void CreateTanmak(Vector3 pos, Color color, float rot=0f, float sp=1f, bool add=false)
	{
		Position=pos;
		this.rot=rot;
		speed=sp;
		time=-1;
		graphic.SetInstanceShaderParameter("color",color);
		graphic.SortingOffset=GetParent().GetChildCount()/1000f;
		AddToGroup("Tanmak");
	}
		public virtual void DeleteTanmak()
	{
		QueueFree();
	}
}

using Godot;
using Godot.Collections;
using System;

public partial class Bullet : Node3D
{
	public float speed = 10f;
	public int time =0;
	private bool hit =false;
	private ShaderMaterial mat;
	[Export]
	private SimpleCollider collider;
	[Export]
	private GeometryInstance3D mesh;
	[Export]
	public Color color, rimColor=Colors.White;
	public override void _Process(double delta)
	{
		Translate(Vector3.Right * speed * Global.FrameDelta);
		if(time >= 60)
		{
			time=0;
			DeleteBullet();
		}
		Collision coll =collider.HitTestFirst();
		if(coll!=null && !hit)
		{
			if(coll.Target.RootNode is Boss boss)
			{
				boss.Hit(1,false);
			}
			HitTanmak();
		}
		if(hit)
		{
			speed-=0.02f;
			mesh.Transparency+=0.04f;
			if(time>=25)
			{
				DeleteBullet();
			}
		}
		time++;
	}
	public void CreateBullet(Vector3 pos, float rot=0f, float sp=10f)
	{
		Position=pos;
		RotationDegrees=Vector3.Back*rot;
		speed=sp;
		time=0;
		hit=false;
		mesh.SetInstanceShaderParameter("setTime",Global.SecTime);
	}
	private void HitTanmak()
	{
		foreach(String group in GetGroups())
		{
			if(group.StartsWith('_'))continue;
			RemoveFromGroup(group);
		}
		hit=true;
		time=0;
		speed=0.5f;
	}
	public void DeleteBullet()
	{
		QueueFree();
	}
}

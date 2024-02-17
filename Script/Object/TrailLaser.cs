using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class TrailLaser : Tanmak
{
	private int MAX_SIZE=16;
	[Export]
	public Trail trail;
	[Export(PropertyHint.Range, "2,100,")]
	public int length=40;
	private int grazeCount=0;
	private float segmentScale=1;
	private List<Vector3> points=new List<Vector3>();
	public override void _Process(double delta)
	{
		Move();
		base._Process(delta);
		PointProcess();
		SegmentProcess();
		
		foreach(Collision col in collider.HitTestAll())
		{
			if(col.Target.InLayer(5))
			{
				collider.SetSegment(col.Index,false);
			}
			if(col.Target.InLayer(0))
			{
				if(cha.hitShape.Equals(col.Target))
				{
					cha.Hit();
					if(Eraseable)collider.SetSegment(col.Index,false);
				}
				if(cha.grazeShape.Equals(col.Target) && time%(int)segmentScale==0 && grazeCount>0)
				{
					cha.Graze(collider.GlobalPosition+collider.GetPos(col.Index),true);
					grazeCount--;
				}
			}
			if(col.Target.InLayer(3) && Eraseable)
			{
				collider.SetSegment(col.Index,false);
				cha.Graze(collider.GlobalPosition+collider.GetPos(col.Index),false);
			}
		}
	}
	private void PointProcess()
	{
		Vector3 pos=GlobalPosition;
		pos.Z=rot-180f;
		if(points.Count<length)
		{
			points.Add(pos);
		}
		else
		{
			for(int i=0; i<points.Count-1;i++)
			{
				points[i]=points[i+1];
			}
			points[points.Count-1]=pos;
		}
	}
	private void SegmentProcess()
	{
		List<Vector3> pos = new List<Vector3>();
		if(collider.Segments.Count * segmentScale <= points.Count)
		{
			collider.AddSegment(Vector3.Zero);
			grazeCount++;
		}
		bool segBool = collider.GetEnable(0);
		bool delBool=true;
		pos.Clear();
		trail.Clear();
		for(int i=0; i<collider.Segments.Count;i++)
		{
			int index = points.Count-Mathf.RoundToInt(i * segmentScale)-1;
			if(i==collider.Segments.Count-1)index=0;
			Vector3 collpos=points[index]-GlobalPosition;
			collpos.Z=0;
			collider.SetSegment(i,collpos);
			if(collider.GetEnable(i))
			{
				pos.Add(points[index]);
				segBool=true;
				delBool=false;
			}
			else if(segBool)
			{
				segBool=false;
				trail.DrawTrail(pos,collider.Width+0.01f);
				pos.Clear();
			}
		}
		if(delBool)DeleteTanmak();
		if(segBool && pos.Count>0)trail.DrawTrail(pos,collider.Width+0.01f);
	}
	public void CreateTanmak(Vector3 pos, Color color, float rot=0f, float sp=10f, int length = 40, float width = 0.02f)
	{
		base.CreateTanmak(pos,color,rot,sp);
		this.length=length;
		int size = Mathf.Min((int)(length*0.25f),MAX_SIZE);
		grazeCount=0;
		segmentScale=(float)length/(size-1);
		collider.Width=width;
	}
}

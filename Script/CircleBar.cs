using Godot;
using System;
using System.Collections.Generic;

public partial class CircleBar : Trail
{
	[Export]
	public float value=1, width=0.02f, radius=1;
	private List<Vector3> points=new List<Vector3>(31);
	private float currentValue=0;
    public override void _Ready()
    {
        base._Ready();
		for(int i=0; i<points.Capacity;i++)
		{
			points.Add(Global.MoveVector(90f+value*(i*18f)) * radius);
		}
    }
    
	public override void _Process(double delta)
	{
		if(currentValue!=value)
		{
			for(int i=0; i<points.Capacity;i++)
			{
				Vector3 vec=GlobalPosition+(Global.MoveVector(90f+value*(i*12f))) * radius;
				vec.Z=180+value*(i*12f);
				points[i]=vec;
			}
			DrawTrail(points,width,false);
		}
	}
}

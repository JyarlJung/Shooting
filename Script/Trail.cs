using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Trail : MeshInstance3D
{
	[Export]
	public Curve line;
	private static SurfaceTool st=new SurfaceTool();
	private static List<ImmediateMesh> meshList=new List<ImmediateMesh>();
	private Vector3 GetNormal(Vector3 arrow, float axisZ=0.6f)
	{
		arrow.Z=0;
		return (arrow.Normalized()+(Vector3.Back*axisZ)).Normalized();
	}
	private Vector3 GetPos(Vector3 arrow, float width=0.05f)
	{
		arrow.Z=0;
		return arrow.Rotated(Vector3.Forward,0.5f*Mathf.Pi) * width;
	}
	private Vector3 RemoveZ(Vector3 pos)
	{
		pos.Z=0;
		return pos;
	}
	public override void _Ready()
	{
		if(meshList.Count==0)
		{
			Mesh=new ImmediateMesh();
		}
		else
		{
			Mesh=meshList[0];
			meshList.RemoveAt(0);
		}
	}
	public override void _ExitTree()
	{
		ImmediateMesh lineMesh=(ImmediateMesh)Mesh;
		lineMesh.ClearSurfaces();
		meshList.Add(lineMesh);
	}

	public void DrawTrail(List<Vector3> points, float width, bool cont=true)
	{
		ImmediateMesh lineMesh=(ImmediateMesh)Mesh;
		if(!cont)
		{
			lineMesh.ClearSurfaces();
		}
		Vector3 arrow, pos, normal;
		lineMesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip,MaterialOverride);
		float bakewidth= (line==null) ? 1 : line.Sample(0);
		Global.MoveVector(points[0].Z);
		arrow = (Global.MoveVector(points[0].Z)).Normalized();
		pos = GetPos(arrow,bakewidth*width);
		normal = GetNormal(-arrow);
		lineMesh.SurfaceSetNormal(normal);
		lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[0])+pos)-(arrow*width));
		lineMesh.SurfaceSetNormal(normal);
		lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[0])-pos)-(arrow*width));		

		for(int i=0; i<points.Count;i++)
		{
			arrow = Global.MoveVector(points[i].Z);
			bakewidth= (line==null) ? 1 : line.Sample((float)(i+1)/(points.Count+1));
			pos = GetPos(arrow,bakewidth * width);
			normal = GetNormal(pos);
			lineMesh.SurfaceSetNormal(normal);
			lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[i])+pos));
			normal = GetNormal(-pos);
			lineMesh.SurfaceSetNormal(normal);
			lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[i])-pos));			
		}
		arrow = Global.MoveVector(points[points.Count-1].Z);
		bakewidth= (line==null) ? 1 : line.Sample(1);
		pos = GetPos(arrow,bakewidth*width);
		normal = GetNormal(arrow);
		lineMesh.SurfaceSetNormal(normal);
		lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[points.Count-1])+pos+(arrow*width)));
		lineMesh.SurfaceSetNormal(normal);
		lineMesh.SurfaceAddVertex(ToLocal(RemoveZ(points[points.Count-1])-pos+(arrow*width)));		


		lineMesh.SurfaceEnd();
	}
	public void Clear()
	{
		ImmediateMesh lineMesh=(ImmediateMesh)Mesh;
		lineMesh.ClearSurfaces();
	}
}

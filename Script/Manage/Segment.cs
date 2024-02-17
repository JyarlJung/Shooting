using Godot;
using System;

public class Segment
{
	//Property
	public Vector3 Pos;
	public bool Enable = true;
	
	public Segment(Vector3 pos, bool enable)
	{
		Pos=pos;
		Enable=enable;
	}
}
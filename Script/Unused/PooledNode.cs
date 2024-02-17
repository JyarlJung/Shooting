using Godot;
using System;

public partial class PooledNode : Node3D
{
	public NodePool pool;
	public void PushItem()
	{
		if(pool!=null)
		{
			pool.PushItem(GetParent<Node3D>().GetParent<Node3D>());
		}
	}
}

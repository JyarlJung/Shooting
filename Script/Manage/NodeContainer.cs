using Godot;
using System;
using Godot.Collections;

public partial class NodeContainer : Node3D
{
	[Export]
	private Array<String> _names;
	[Export]
	private Array<PackedScene> _nodes;

	public override void _Ready()
	{
		Global.Container=this;
	}
	
	public Node3D CreateNode(String name)
	{
		Node3D item;
		for(int i=0;i<_nodes.Count;i++)
		{
			if(_names[i].CompareTo(name)==0)
			{
				item=(Node3D)_nodes[i].Instantiate();
				AddChild(item);
				return item;
			}
		}
		return null;
	}
	public Node3D CreateNode(int index)
	{
		Node3D item=(Node3D)_nodes[index].Instantiate();
		AddChild(item);
		return item;
	}
}

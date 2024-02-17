using Godot;
using System;
using System.Collections.Generic;

public partial class NodePool : Node3D
{
	[Export]
	public string itemName;
	public Queue<Node3D> itemList = new Queue<Node3D>();
	[Export]
	public int count=100;
	private int time=0;
	private PackedScene pack;
	public override void _Ready()
	{
		pack=GD.Load<PackedScene>(itemName);
		Node3D item;
		for(int i =0; i<count;i++)
		{
			item=(Node3D)pack.Instantiate();
			SearchPooledNode(item).pool=this;
			AddChild(item);
			PushItem(item);
		}
		
	}
	public override void _Process(double delta)
	{
		time++;
	}
	public Node3D PopItem(Node3D parent = null)
	{
		Node3D item;
		if(parent==null)parent=this;
		if(itemList.Count==0)
		{
			item=(Node3D)pack.Instantiate();
			SearchPooledNode(item).pool=this;
			parent.AddChild(item);
		}
		else
		{
			item=itemList.Dequeue();
		}
		item.SetProcess(true);
		item.ProcessMode=Node.ProcessModeEnum.Inherit;
		item.Show();
		return item;
	}
	public Node3D PushItem(Node3D item)
	{
		itemList.Enqueue(item);
		item.SetProcess(false);
		item.ProcessMode=Node.ProcessModeEnum.Disabled;
		item.Hide();
		return item;
	}
	private PooledNode SearchPooledNode(Node3D item)
	{
		Node3D child=item.GetChild<Node3D>(0);
		PooledNode poolednode;
		for(int i=0; i<child.GetChildCount();i++)
		{
			poolednode=child.GetChild<PooledNode>(i);
			if(poolednode!=null)
			{
				return poolednode;
			}
		}
		return null;
	}
	
}

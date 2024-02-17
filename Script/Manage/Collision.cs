using Godot;
using System;

public class Collision
{
	//Member
	private SimpleCollider _target;
	private int _index;

	//MemberGetter
	public SimpleCollider Target { get => _target;}
    public int Index { get => _index;}
	
	public Collision(SimpleCollider target, int index)
	{
		_target=target;
		_index=index;
	}

}
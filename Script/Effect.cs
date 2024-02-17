using Godot;
using System;

public partial class Effect : AnimationPlayer
{

	public override void _Ready()
	{
		AnimationFinished += AnimationFinish;
	}
	private void AnimationFinish(StringName name)
	{
		GetNode(RootNode).QueueFree();
	}
}

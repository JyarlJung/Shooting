using Godot;
using System;

public partial class SimpleAnimator : AnimationPlayer
{
	[Export]
	public string startAnime="";
	[Export]
	public string tailAnime="";
	public override void _Ready()
	{
		if(startAnime!="")Play(startAnime);
		if(tailAnime=="")tailAnime=startAnime;
	}
	public void Finished(String name)
	{
		if(name!=tailAnime)Play(tailAnime);
	}
}

using Godot;
using System;
using Godot.Collections;

public partial class SoundManager : Node3D
{
	private static int MAX_CHANNEL=10;
	[Export]
	private Array<String> _names;
	[Export]
	private Array<AudioStream> _sounds;
	private int ind=0;
	public override void _Ready()
	{
		Global.Sound=this;
		for(int i=0; i<MAX_CHANNEL;i++)
		{
			AddChild(new AudioStreamPlayer());
		}
	}
	public AudioStream GetSound(String name)
	{
		for(int i=0;i<_sounds.Count;i++)
		{
			if(_names[i].CompareTo(name)==0)
			{
				return _sounds[i];
			}
		}
		return null;
	}
	public void Play(String name)
	{
		ind=ind%MAX_CHANNEL;
		GetChild<AudioStreamPlayer>(ind).Stream=GetSound(name);
		GetChild<AudioStreamPlayer>(ind).Play();
		ind++;
	}
	public static void OverPlay(AudioStreamPlayer player, float time, AudioStream sound=null, bool useRatio=false)
	{
		if(sound!=null)
		{
			if(player.Stream!=sound)player.Stream=sound;
		}
		if(player.Playing)
		{
			if(!useRatio)
			{
				if(player.GetPlaybackPosition()>=time)
				{
					player.Play();
				}
			}
			else
			{
				if(player.GetPlaybackPosition()/player.Stream.GetLength() >=time)
				{
					player.Play();
				}
			}
		}
		else
		{
			player.Play();
		}
	}
	public static void OverPlayName(AudioStreamPlayer player, float time, String name, bool useRatio=false)
	{
		AudioStream sound = Global.Sound.GetSound(name);
		if(sound!=null)
		{
			if(player.Stream!=sound)player.Stream=sound;
		}
		if(player.Playing)
		{
			if(!useRatio)
			{
				if(player.GetPlaybackPosition()>=time)
				{
					player.Play();
				}
			}
			else
			{
				if(player.GetPlaybackPosition()/player.Stream.GetLength() >=time)
				{
					player.Play();
				}
			}
		}
		else
		{
			player.Play();
		}
	}
}

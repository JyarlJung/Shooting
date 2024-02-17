using Godot;
using System;

public partial class Ui : Control
{
	[Export]
	public float hp=2f, bomb=2f;
	[Export]
	public ProgressBar hpBar, bombBar, grazeBar;
	[Export]
	public Label hpLabel, bombLabel,scoreLabel, grazeLabel,comboLabel,debug,spellName,spellInfo;
	[Export]
	public AnimationPlayer anime;
	private int comboTime=0, comboReset=0;
	private float shake=0f;
	public int graze=0, score=0; 
	public float startTime=0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.UiNode=this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		hpBar.Value+=Mathf.Ceil((hp*100f-hpBar.Value)/10f);
		bombBar.Value+=Mathf.Ceil((bomb*100f-bombBar.Value)/10f);
		grazeBar.Value=comboTime;

		hpLabel.Text=((int)(hp*100f)).ToString()+"%";
		bombLabel.Text=((comboTime>=100) ? "+":"") + ((int)(bomb*100f)).ToString()+"%";
		grazeLabel.Text=graze.ToString("0000");
		scoreLabel.Text=score.ToString("00000000");
		comboLabel.Text="+"+Mathf.Min(10+comboTime/10,20).ToString()+" Score";
		debug.Text=Engine.GetFramesPerSecond().ToString()+"fps, "+GetTree().GetNodesInGroup("Tanmak").Count;
		if(comboReset>0)comboReset--;
		if(comboTime>0 && comboReset==0)
		{
			comboTime--;
		}
		if(comboTime>100)bomb+=0.0005f;
		StyleBoxFlat bar=(StyleBoxFlat)hpBar.Get("theme_override_styles/fill");
		Color color = bar.BgColor;
		color.A= hp>=1f ? 1f : 0.5f;
		bar.BgColor=color;
		bar=(StyleBoxFlat)bombBar.Get("theme_override_styles/fill");
		color = bar.BgColor;
		color.A= bomb>=1f ? 1f : 0.5f;
		bar.BgColor=color;
		bar=(StyleBoxFlat)grazeBar.Get("theme_override_styles/fill");
		color = bar.BgColor;
		color.A= comboTime>=100 ? 1f : 0.5f;
		bar.BgColor=color;
		if(shake>0)
		{
			Vector3 value= new Vector3(GD.Randf()*shake,GD.Randf()*shake,5f);
			Global.Camera.Position=value;
			shake-=0.002f;
		}
		else
		{
			Global.Camera.Position=Vector3.Back*5f;
		}
		if(startTime>0)
		{
			startTime-=Global.FrameDelta;
			spellInfo.Text=startTime.ToString("F");
		}
		else
		{
			spellInfo.Text="";
		}
	}
	public void Graze()
	{
		graze++;
		score+=Mathf.Min(comboTime/10,10);
		comboTime=Mathf.Min(130,comboTime+7);
		comboReset=20;
	}
	public void ShakeCamera(float power)
	{
		shake=power;
	}
	public void SpellStart(String name,Vector4 info)
	{
		spellName.Text=name;
		startTime=info.W;
		if(info.Z!=0)
		{
			anime.Play("Start");
		}
	}
	public void SpellEnd()
	{
		anime.Play("RESET");
		startTime=0f;
	}
}

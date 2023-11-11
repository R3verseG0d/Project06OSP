using STHLua;
using UnityEngine;

public class GrindFX : MonoBehaviour
{
	public enum Type
	{
		Metal = 0,
		Wind = 1,
		Nature = 2
	}

	[Header("Framework")]
	public Type RailType;

	public ParticleSystem[] ManipulatedFX;

	public ParticleSystem[] FX;

	[Header("Settings")]
	public Transform Pivot;

	public AudioSource[] Sources;

	internal PlayerBase Player;

	private Vector3 ForwardMag;

	private bool Stopped;

	private float Speed;

	private void Update()
	{
		if (Stopped)
		{
			return;
		}
		switch (RailType)
		{
		case Type.Metal:
		{
			ForwardMag = Vector3.Lerp(ForwardMag, base.transform.forward, Time.deltaTime * 10f);
			float num = Mathf.Clamp(Vector3.Dot(ForwardMag, base.transform.right), -35f, 35f);
			Speed = ((Player.CurSpeed > 0f) ? Player.CurSpeed : (0f - Player.CurSpeed));
			if (ManipulatedFX != null)
			{
				for (int i = 0; i < ManipulatedFX.Length; i++)
				{
					ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ManipulatedFX[i].velocityOverLifetime;
					AnimationCurve animationCurve = new AnimationCurve();
					animationCurve.AddKey(0f, 0f);
					animationCurve.AddKey(1f, (0f - num) * 5f);
					velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(25f, animationCurve);
					ParticleSystem.MainModule main = ManipulatedFX[i].main;
					main.startSpeed = Mathf.Lerp(0f, 45f, Speed / Sonic_New_Lua.c_grind_speed_max);
					main.gravityModifier = Mathf.Lerp(-0f, -10f, Speed / Sonic_New_Lua.c_grind_speed_max);
				}
			}
			Pivot.forward = ((Player.CurSpeed > 0f) ? base.transform.forward : (-base.transform.forward));
			bool flag = Player.GrindSpeed > Player.GrindSpeedOrg || (Player.GetPrefab("snow_board") && Singleton<RInput>.Instance.P.GetButton("Button A"));
			Sources[0].pitch = Mathf.Min(1f, Player.GrindSpeed / Player.GrindSpeedOrg) * 0.5f + 0.5f;
			Sources[0].volume = ((Player.GetState() == "Grinding" && !Player.GrindTrick && !Player.RailSwitch) ? 0.65f : 0f);
			Sources[1].volume = ((Player.GetState() == "Grinding" && !Player.GrindTrick && !Player.RailSwitch) ? Mathf.Lerp(Sources[1].volume, flag ? 0.65f : 0f, Time.deltaTime * 2f) : 0f);
			break;
		}
		case Type.Wind:
			Sources[0].volume = Mathf.Lerp(Sources[0].volume, (Player.GetState() == "Grinding" && !Player.GrindTrick && !Player.RailSwitch) ? 0.6f : 0f, Time.deltaTime * 3f);
			break;
		case Type.Nature:
			Sources[0].volume = ((Player.GetState() == "Grinding" && !Player.GrindTrick && !Player.RailSwitch) ? 1f : 0f);
			break;
		}
		if (FX != null)
		{
			for (int j = 0; j < FX.Length; j++)
			{
				ParticleSystem.EmissionModule emission = FX[j].emission;
				emission.enabled = Player.GetState() == "Grinding" && !Player.GrindTrick && !Player.RailSwitch;
			}
		}
	}

	public void StopRailFX()
	{
		Stopped = true;
		if (FX != null)
		{
			for (int i = 0; i < FX.Length; i++)
			{
				FX[i].Stop();
			}
		}
		for (int j = 0; j < Sources.Length; j++)
		{
			Sources[j].Stop();
		}
	}
}

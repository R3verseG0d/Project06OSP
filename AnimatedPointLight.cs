using System.Collections;
using UnityEngine;

public class AnimatedPointLight : MonoBehaviour
{
	public enum Type
	{
		Random = 0,
		PingPong = 1
	}

	public struct LightParam
	{
		public Params Parameters;

		public float TimeStart;

		public float TimeTarget;

		public float ValueStart;

		public float ValueTarget;

		public float Value;

		public LightParam(Params _Parameters, float _Value)
		{
			Parameters = _Parameters;
			TimeStart = 0f;
			TimeTarget = 0f;
			ValueStart = 0f;
			ValueTarget = 0f;
			Value = _Value;
		}

		public void Calculate(float AmountTime)
		{
			TimeStart = AmountTime;
			TimeTarget = AmountTime + Random.Range(Parameters.TimeMin, Parameters.TimeMax);
			ValueStart = ValueTarget;
			ValueTarget = Random.Range(Parameters.Min, Parameters.Max);
		}

		public void Update(float AmountTime)
		{
			if (AmountTime > TimeTarget)
			{
				Calculate(AmountTime);
			}
			Value = Mathf.Lerp(ValueStart, ValueTarget, (Time.time - TimeStart) / (TimeTarget - TimeStart));
		}
	}

	[Header("Framework")]
	public Light Light;

	public Type EffectType;

	public Params _Intensity;

	public Params _Range;

	public float PingPongMin;

	public float PingPongMax;

	public float PingPongSpeed;

	public float RangeMin;

	public float RangeMax;

	public float RangeSpeed;

	private void Start()
	{
		if (EffectType == Type.Random)
		{
			StartCoroutine(Flicker());
		}
	}

	private void Update()
	{
		if (EffectType == Type.PingPong)
		{
			Light.range = Mathf.Lerp(RangeMin, RangeMax, Mathf.Abs(Mathf.Cos(Time.time * RangeSpeed)));
			Light.intensity = Mathf.Lerp(PingPongMin, PingPongMax, Mathf.Abs(Mathf.Cos(Time.time * PingPongSpeed)));
		}
	}

	private IEnumerator Flicker()
	{
		LightParam LightIntensity = new LightParam(_Intensity, Light.intensity);
		LightParam LightRange = new LightParam(_Range, Light.range);
		while (true)
		{
			LightIntensity.Update(Time.time);
			Light.intensity = LightIntensity.Value;
			LightRange.Update(Time.time);
			Light.range = LightRange.Value;
			yield return new WaitForFixedUpdate();
		}
	}
}

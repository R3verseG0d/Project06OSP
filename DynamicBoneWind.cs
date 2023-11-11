using UnityEngine;

public class DynamicBoneWind : MonoBehaviour
{
	[Header("Framework")]
	public Vector2 Strength = new Vector2(-0.01f, 0.01f);

	public float Interval = 0.1f;

	[Header("Optional")]
	public bool IsShadow;

	public PlayerManager PM;

	public Vector2 ChaosBoostStrength = new Vector2(0.005f, 0.025f);

	private DynamicBone Bone;

	private float TimeInterval;

	private float WindYVel;

	private float WindFactor;

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.JiggleBones == 0)
		{
			Object.Destroy(this);
		}
		Bone = GetComponent<DynamicBone>();
	}

	private void Update()
	{
		TimeInterval += Time.deltaTime;
		if (TimeInterval > Interval)
		{
			TimeInterval = 0f;
			if (IsShadow)
			{
				WindFactor = Random.Range(PM.shadow.IsChaosBoost ? ChaosBoostStrength.x : Strength.x, PM.shadow.IsChaosBoost ? ChaosBoostStrength.y : Strength.y);
			}
			else
			{
				WindFactor = Random.Range(Strength.x, Strength.y);
			}
		}
		WindYVel = Mathf.Lerp(WindYVel, WindFactor, Time.deltaTime * 9f);
		Bone.m_Force = new Vector3(Bone.m_Force.x, WindYVel, Bone.m_Force.z);
	}
}

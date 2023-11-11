using UnityEngine;

public class JiggleBonesController : MonoBehaviour
{
	[Header("Framework")]
	public DynamicBone[] DynamicBones;

	public DynamicBone[] ExtraBones;

	private PlayerManager PM;

	private string PlayerName;

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.JiggleBones == 0)
		{
			for (int i = 0; i < DynamicBones.Length; i++)
			{
				DynamicBones[i].gameObject.SetActive(value: false);
			}
			if (ExtraBones == null)
			{
				return;
			}
			for (int j = 0; j < ExtraBones.Length; j++)
			{
				if (Singleton<Settings>.Instance.settings.JiggleBones == 0)
				{
					ExtraBones[j].gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			PM = GetComponent<PlayerManager>();
			if ((bool)PM)
			{
				PlayerName = PM.Base.PlayerPrefab.ToString();
			}
		}
	}

	private void Update()
	{
		if (!PM || Singleton<Settings>.Instance.settings.JiggleBones == 0)
		{
			return;
		}
		for (int i = 0; i < DynamicBones.Length; i++)
		{
			if (PlayerName == "sonic_new")
			{
				DynamicBones[i].enabled = PM.Base.GetState() != "SpinDash" || (PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 1);
			}
			if (PlayerName == "shadow")
			{
				DynamicBones[i].enabled = PM.Base.GetState() != "SpinDash" || (PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 1);
			}
			for (int j = 0; j < DynamicBones[i].m_ParticleTrees.Count; j++)
			{
				DynamicBone.ParticleTree particleTree = DynamicBones[i].m_ParticleTrees[j];
				for (int k = 0; k < particleTree.m_Particles.Count; k++)
				{
					DynamicBone.Particle particle = particleTree.m_Particles[k];
					if (PlayerName == "sonic_new")
					{
						particle.m_Stiffness = (PM.sonic.IsSuper ? 0.6f : 0f);
					}
					if (PlayerName == "sonic_fast")
					{
						particle.m_Stiffness = (PM.sonic_fast.IsSuper ? 0.6f : 0f);
					}
				}
			}
		}
	}
}

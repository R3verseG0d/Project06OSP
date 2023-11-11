using System.Collections.Generic;
using UnityEngine;

public class Aqa_Magnet : MonoBehaviour
{
	public enum Type
	{
		Timed = 0,
		Switch = 1
	}

	[Header("Framework")]
	public Type Kind;

	public float Radius;

	public float Force;

	public float OffTime;

	[Header("Prefab")]
	public AudioSource Audio;

	public AudioSource AudioLoop;

	public AudioClip[] Clips;

	public AudioClip[] LoopClips;

	public Transform AttractPoint;

	public Animator[] OffMagnets;

	public Animator[] OnMagnets;

	public Animator[] RepelMagnets;

	public ParticleSystem AttractStartFX;

	public ParticleSystem RepelStartFX;

	public ParticleSystem[] AttractFX;

	public ParticleSystem[] RepelFX;

	private List<Rigidbody> ObjsToAttract;

	private bool Activated;

	private bool RepelMode;

	private bool PlayTimerCountdownSound;

	private float StartTime;

	public void SetParameters(int _Kind, float _Radius, float _Force, float _OffTime)
	{
		Kind = (Type)(_Kind - 1);
		Radius = _Radius;
		Force = _Force;
		OffTime = _OffTime;
	}

	private void Start()
	{
		ManageModels(TurnMode: false);
		ObjsToAttract = new List<Rigidbody>();
	}

	private void Update()
	{
		if (Activated)
		{
			if (!AudioLoop.isPlaying)
			{
				AudioLoop.clip = LoopClips[1];
				AudioLoop.loop = true;
				AudioLoop.Play();
			}
			if ((Kind == Type.Timed && OffTime != 0f && Time.time - StartTime > OffTime) || (RepelMode && Time.time - StartTime > 1.5f))
			{
				ManageModels(TurnMode: false);
				PlayMagnetAnim(OnOff: false);
				Audio.PlayOneShot(Clips[2], Audio.volume);
				AudioLoop.Stop();
				if (RepelMode)
				{
					RepelMode = false;
				}
				Activated = false;
			}
			if (Kind == Type.Timed && OffTime != 0f && Time.time - StartTime > OffTime - 3f && !PlayTimerCountdownSound)
			{
				Audio.PlayOneShot(Clips[1], Audio.volume);
				PlayTimerCountdownSound = true;
			}
		}
		for (int i = 0; i < AttractFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = AttractFX[i].emission;
			emission.enabled = Activated && !RepelMode;
		}
		if (Kind == Type.Switch)
		{
			for (int j = 0; j < RepelFX.Length; j++)
			{
				ParticleSystem.EmissionModule emission2 = RepelFX[j].emission;
				emission2.enabled = Activated && RepelMode;
			}
		}
	}

	private void FixedUpdate()
	{
		if (!Activated || ObjsToAttract.Count == 0)
		{
			return;
		}
		for (int i = 0; i < ObjsToAttract.Count; i++)
		{
			if ((bool)ObjsToAttract[i])
			{
				Vector3 vector = ((!RepelMode) ? (AttractPoint.position - ObjsToAttract[i].position).normalized : (ObjsToAttract[i].position - AttractPoint.position).normalized);
				ObjsToAttract[i].AddForce(vector * Force, ForceMode.Impulse);
			}
		}
	}

	private void ManageModels(bool TurnMode, bool Repel = false)
	{
		OffMagnets[(int)Kind].gameObject.SetActive(!TurnMode);
		OnMagnets[(int)Kind].gameObject.SetActive(TurnMode && !Repel);
		RepelMagnets[(int)Kind].gameObject.SetActive(TurnMode && Repel);
	}

	private void PlayMagnetAnim(bool OnOff, bool Repel = false)
	{
		if (!Repel)
		{
			if (OnOff)
			{
				OnMagnets[(int)Kind].SetTrigger("On Activated");
			}
			else
			{
				OffMagnets[(int)Kind].SetTrigger("On Activated");
			}
		}
		else
		{
			RepelMagnets[(int)Kind].SetTrigger("On Activated");
		}
	}

	private void OnFlash()
	{
		OnHit(new HitInfo(Object.FindObjectOfType<PlayerBase>().transform, Vector3.zero, 10));
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!Activated)
		{
			ManageModels(TurnMode: true);
			PlayMagnetAnim(OnOff: true);
			Audio.PlayOneShot(Clips[0], Audio.volume);
			AudioLoop.clip = LoopClips[0];
			AudioLoop.loop = false;
			AudioLoop.Play();
			PlayTimerCountdownSound = false;
			Collider[] array = Physics.OverlapSphere(AttractPoint.position, Radius);
			if (array != null)
			{
				ObjsToAttract.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					if ((bool)array[i].GetComponent<EnemyBase>())
					{
						array[i].gameObject.SendMessage("OnCommandDestroy", new HitInfo(HitInfo.player, Vector3.zero, 10), SendMessageOptions.DontRequireReceiver);
					}
					if (array[i].gameObject.tag == "Magnetable")
					{
						ObjsToAttract.Add(array[i].GetComponent<Rigidbody>());
					}
					array[i].SendMessage("OnDeflect", base.transform, SendMessageOptions.DontRequireReceiver);
				}
			}
			StartTime = Time.time;
			AttractStartFX.Play();
			Activated = true;
		}
		else if (Kind == Type.Switch && !RepelMode && Time.time - StartTime > 1f)
		{
			ManageModels(TurnMode: true, Repel: true);
			PlayMagnetAnim(OnOff: true, Repel: true);
			Audio.Stop();
			Audio.PlayOneShot(Clips[3], Audio.volume);
			StartTime = Time.time;
			RepelStartFX.Play();
			RepelMode = true;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(AttractPoint.position, Radius);
	}
}

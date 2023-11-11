using UnityEngine;

public class LaserBeam : MonoBehaviour
{
	public BoxCollider BoxCollider;

	public Transform BeamTip;

	public AudioSource Audio;

	public ParticleSystem[] Particles;

	[Range(0f, 1f)]
	public float Thickness = 1f;

	public bool DynamicScale;

	internal float BeamScale;

	internal int State = 2;

	private void FixedUpdate()
	{
		if (State == 0)
		{
			BeamScale += Time.fixedDeltaTime * 5f;
			if (BeamScale >= 1f)
			{
				State = 1;
			}
			if (!Audio.isPlaying)
			{
				Audio.Play();
			}
		}
		else if (State == 1)
		{
			BeamScale = (DynamicScale ? (0.8f + Mathf.Sin(Time.time * 50f) / 2.5f) : 1f);
		}
		else if (State == 2)
		{
			BeamScale -= Time.fixedDeltaTime * 10f;
			if (Audio.isPlaying)
			{
				Audio.Stop();
			}
		}
		BeamScale = Mathf.Clamp(BeamScale, 0f, 1f);
		base.transform.localScale = new Vector3(BeamScale * Thickness, BeamScale * Thickness, 1f);
		BoxCollider.enabled = State == 1;
		for (int i = 0; i < Particles.Length; i++)
		{
			ParticleSystem.EmissionModule emission = Particles[i].emission;
			emission.enabled = State == 1;
		}
	}

	public void UpdateBeam(Vector3 TargetPos)
	{
		Vector3 forward = TargetPos - base.transform.position;
		base.transform.forward = forward;
		BeamTip.position = TargetPos;
		BeamTip.forward = forward;
		float num = Vector3.Distance(base.transform.position, TargetPos);
		BoxCollider.center = Vector3.forward * num * 0.5f;
		BoxCollider.size = new Vector3(1f, 1f, num);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Vehicle")
		{
			collider.gameObject.transform.SendMessage("OnVehicleHit", 3f, SendMessageOptions.DontRequireReceiver);
		}
	}
}

using UnityEngine;

public class Common_Key : EventStation
{
	[Header("Framework")]
	public string Event;

	[Header("Prefab")]
	public Animator Animator;

	public AudioSource Audio;

	public Transform Mesh;

	public GameObject InterestPoint;

	public ParticleSystem[] FX;

	public GameObject ObtainFX;

	internal ObjectHuntingManager Manager;

	internal bool Obtained;

	public void SetParameters(string _Event)
	{
		Event = _Event;
	}

	private void Update()
	{
		Mesh.Rotate(0f, ((!Obtained) ? 120f : 420f) * Time.deltaTime, 0f);
		if (Obtained)
		{
			base.transform.rotation = Quaternion.identity;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !Obtained)
		{
			Animator.SetTrigger("On Obtain");
			Audio.Play();
			CallEvent(Event, collider.gameObject);
			Manager.AddScore(player);
			for (int i = 0; i < FX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = FX[i].emission;
				emission.enabled = false;
			}
			ObtainFX.SetActive(value: true);
			base.transform.position = player.transform.position + player.transform.up * 0.25f;
			base.transform.SetParent(player.transform);
			Object.Destroy(InterestPoint);
			Object.Destroy(base.gameObject, 2.75f);
			Obtained = true;
		}
	}
}

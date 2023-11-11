using UnityEngine;

public class ContactObject : MonoBehaviour
{
	[Header("Sound")]
	public float Volume = 0.5f;

	public bool RandomPitch;

	public AudioClip[] ContactClip;

	public GameObject AudioSourcePrefab;

	private Rigidbody _Rigidbody;

	private Vector3 position = Vector3.zero;

	private float Timer = -1f;

	private void OnCollisionEnter(Collision collision)
	{
		if (Timer == -1f)
		{
			Timer = Time.time;
		}
		if (Time.time - Timer < 0.5f || collision.collider.transform.parent == base.transform.parent)
		{
			return;
		}
		Timer = Time.time;
		if (ContactClip != null)
		{
			AudioSource component = Object.Instantiate(AudioSourcePrefab, base.transform.position, Quaternion.identity).GetComponent<AudioSource>();
			component.clip = ContactClip[Random.Range(0, ContactClip.Length)];
			component.spatialBlend = 1f;
			if (RandomPitch)
			{
				component.pitch = Random.Range(0.75f, 1.25f);
			}
			if (!_Rigidbody)
			{
				_Rigidbody = base.transform.GetComponent<Rigidbody>();
			}
			if (!_Rigidbody)
			{
				_Rigidbody = base.transform.parent.GetComponent<Rigidbody>();
			}
			component.volume = Mathf.Min(_Rigidbody.velocity.magnitude / 10f, 1f) * Volume;
			component.Play();
		}
	}
}

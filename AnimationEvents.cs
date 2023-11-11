using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEvents : MonoBehaviour
{
	[Header("Particles")]
	public ParticleSystem[] Particles;

	[Header("Audio Sources")]
	public AudioSource[] AudioSources;

	public float Volume;

	[Header("Audio Clips")]
	public AudioSource Audio;

	public AudioClip[] ClipPool;

	public float ClipVolume;

	[Header("Instantiation")]
	public GameObject Prefab;

	public Vector3 Position;

	private bool Enabled;

	private void Update()
	{
		if (Particles != null)
		{
			for (int i = 0; i < Particles.Length; i++)
			{
				ParticleSystem.EmissionModule emission = Particles[i].emission;
				emission.enabled = Enabled;
			}
		}
		if (AudioSources != null)
		{
			for (int j = 0; j < AudioSources.Length; j++)
			{
				AudioSources[j].volume = Mathf.Lerp(AudioSources[j].volume, Enabled ? Volume : 0f, Time.deltaTime * 5f);
			}
		}
	}

	public void EnableComponents()
	{
		Enabled = true;
	}

	public void DisableComponents()
	{
		Enabled = false;
	}

	public void PlayAudioClip(int Index)
	{
		Audio.PlayOneShot(ClipPool[Index], ClipVolume);
	}

	public void PlayRandomAudioClip()
	{
		Audio.PlayOneShot(ClipPool[Random.Range(0, ClipPool.Length)], ClipVolume);
	}

	public void InstantiateObject()
	{
		Object.Instantiate(Prefab, base.transform.position + Position, base.transform.rotation);
	}

	public void DestroyThis()
	{
		Object.Destroy(base.gameObject);
	}

	public void ChangeScene(string GoTo)
	{
		Singleton<GameManager>.Instance.SetLoadingTo(GoTo, Game.MenuLoadMode);
	}

	public void ChangeSceneBlankLoading(string GoTo)
	{
		Singleton<GameManager>.Instance.SetLoadingTo(GoTo, Game.BlankLoadMode);
	}

	public void ChangeSceneNoLoading(string GoTo)
	{
		SceneManager.LoadScene(GoTo, LoadSceneMode.Single);
	}

	private void OnDrawGizmosSelected()
	{
		if (Prefab != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position + Position, 1f);
		}
	}
}

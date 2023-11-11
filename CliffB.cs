using UnityEngine;

public class CliffB : MonoBehaviour
{
	public Animation Animation;

	public Animator Animator;

	public bool StopRigidbodies;

	public bool DestroyObj;

	public GameObject[] DisableObjs;

	private void Update()
	{
		if ((bool)Animation && !Animation.isPlaying)
		{
			CommonFunctions();
			Animation = null;
		}
		if ((bool)Animator && !Animator.GetCurrentAnimatorStateInfo(0).IsName("BreakAnim"))
		{
			CommonFunctions();
			Object.Destroy(Animator);
		}
	}

	private void CommonFunctions()
	{
		if (StopRigidbodies)
		{
			Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].velocity = Vector3.zero;
			}
		}
		if (DestroyObj)
		{
			Object.Destroy(base.gameObject, 10f);
		}
		if (DisableObjs != null)
		{
			for (int j = 0; j < DisableObjs.Length; j++)
			{
				DisableObjs[j].SetActive(value: false);
			}
		}
	}

	public void OnCreate(HitInfo hitInfo)
	{
	}
}

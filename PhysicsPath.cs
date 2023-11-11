using UnityEngine;

public class PhysicsPath : PsiObject
{
	[Header("Framework")]
	public string ObjectType;

	public string SplineName;

	public float Speed;

	[Header("Prefab")]
	public Transform Target;

	public GameObject[] Objects;

	[Header("Optional")]
	public bool AutoStart;

	public bool StartOnCollision;

	public bool PingPong;

	public bool StartBack;

	public bool PlayAnim;

	private BezierCurve Curve;

	private Renderer[] ActiveObjRenderers;

	private bool StartMovement;

	private bool EnablePsiEffect;

	private float Progress;

	private bool SwitchDir;

	public void SetParameters(string _ObjectType, string _SplineName, float _Speed)
	{
		ObjectType = _ObjectType;
		SplineName = _SplineName;
		Speed = _Speed;
	}

	private void Start()
	{
		if ((bool)GameObject.Find(SplineName))
		{
			Curve = GameObject.Find(SplineName).GetComponent<BezierCurve>();
		}
		for (int i = 0; i < Objects.Length; i++)
		{
			if (Objects[i].gameObject.name == ObjectType)
			{
				Objects[i].gameObject.SetActive(value: true);
				ActiveObjRenderers = Objects[i].GetComponentsInChildren<Renderer>();
				if (PlayAnim && (bool)Objects[i].GetComponent<Animation>())
				{
					Objects[i].GetComponent<Animation>().Play();
				}
			}
		}
		if (AutoStart)
		{
			OnEventSignal();
		}
		if (StartBack)
		{
			Progress = Curve.FindNearestPointToProgress(base.transform.position);
		}
	}

	private void Update()
	{
		if (!Curve)
		{
			return;
		}
		if (StartMovement)
		{
			if (!SwitchDir)
			{
				Progress += Speed / Curve.Length() * Time.deltaTime;
			}
			else
			{
				Progress -= Speed / Curve.Length() * Time.deltaTime;
			}
		}
		if (Progress > 1f)
		{
			if (PingPong)
			{
				SwitchDir = true;
			}
			Progress = ((!StartBack) ? 1f : 0f);
		}
		if (Progress < 0f && SwitchDir)
		{
			SwitchDir = false;
		}
		float num = Progress / 1f;
		num = num * num * (3f - 2f * num);
		Target.position = Curve.GetPosition((!StartBack) ? num : Progress);
		if (ActiveObjRenderers == null)
		{
			return;
		}
		for (int i = 0; i < ActiveObjRenderers.Length; i++)
		{
			if ((bool)ActiveObjRenderers[i])
			{
				OnPsiFX(ActiveObjRenderers[i], EnablePsiEffect && !AutoStart && !StartOnCollision);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!StartMovement && StartOnCollision && (bool)GetPlayer(collision.transform))
		{
			StartMovement = true;
		}
	}

	private void OnEventSignal()
	{
		StartMovement = true;
	}

	private void PsiEffect(bool Enable)
	{
		EnablePsiEffect = Enable;
	}
}

using UnityEngine;

public class SnowBoardJump : MonoBehaviour
{
	[Header("Framework")]
	public float Power;

	public float Pitch;

	public float Rate;

	public float BPower_Rate;

	public float Time;

	public void SetParameters(float _Power, float _Pitch, float _Rate, float _BPower_Rate, float _Time)
	{
		Power = _Power;
		Pitch = _Pitch;
		Rate = _Rate;
		BPower_Rate = _BPower_Rate;
		Time = _Time;
	}

	private void OnTriggerEnter(Collider collider)
	{
		SnowBoard component = collider.GetComponent<SnowBoard>();
		if ((bool)component)
		{
			component.IsOnRamp = true;
			component.OnRampEnter(base.transform.forward * Power * 2f + base.transform.up * Pitch * 0.5f, Time, Rate, BPower_Rate);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player" && (bool)collider.gameObject.GetComponent<SnowBoard>())
		{
			collider.gameObject.GetComponent<SnowBoard>().IsOnRamp = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + (base.transform.forward * Power + base.transform.up * Pitch * 0.5f) * Time;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, vector);
		int num = 8 * (int)Power;
		float num2 = 0.01f;
		Vector3 vector2 = base.transform.forward * Power + base.transform.up * Pitch * 0.5f;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		for (int i = 0; i < num; i++)
		{
			vector2.y -= 9.81f * num2;
			vector3 += vector2 * num2;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}

using UnityEngine;

[ExecuteInEditMode]
public class WorldSpaceRotation : MonoBehaviour
{
	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
	}
}

using UnityEngine;

public class CameraEvents : ObjectBase
{
	public CameraParameters cameraParameters;

	public bool ImportantVolume;

	internal bool DestroyOnExit;

	private PlayerBase PlayerDummy;

	private bool IsActive;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (Singleton<Settings>.Instance.settings.NoCameraVolumes != 1 || ImportantVolume))
		{
			IsActive = true;
			PlayerDummy = player;
			player.SetCameraParams(cameraParameters);
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (Singleton<Settings>.Instance.settings.NoCameraVolumes != 1 || ImportantVolume))
		{
			if (player.Camera.IsOnEvent)
			{
				IsActive = false;
			}
			else if (!IsActive)
			{
				IsActive = true;
				PlayerDummy = player;
				player.SetCameraParams(cameraParameters);
			}
			if (!IsActive && !player.Camera.IsOnEvent)
			{
				PlayerDummy = player;
				player.SetCameraParams(cameraParameters);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (Singleton<Settings>.Instance.settings.NoCameraVolumes != 1 || ImportantVolume))
		{
			IsActive = false;
			PlayerDummy = null;
			player.DestroyCameraParams(cameraParameters);
			if (DestroyOnExit)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void DestroyEvent()
	{
		if ((bool)PlayerDummy && (Singleton<Settings>.Instance.settings.NoCameraVolumes != 1 || ImportantVolume))
		{
			PlayerDummy.DestroyCameraParams(cameraParameters);
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (cameraParameters != null)
		{
			if (cameraParameters.Mode == 1 || cameraParameters.Mode == 10 || cameraParameters.Mode == 11)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, (!cameraParameters.ObjTarget) ? cameraParameters.Position : cameraParameters.ObjTarget.position);
			}
			if (cameraParameters.Mode == 4 || cameraParameters.Mode == 40 || cameraParameters.Mode == 41 || cameraParameters.Mode == 42 || cameraParameters.Mode == 5 || cameraParameters.Mode == 50)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
			}
			if (cameraParameters.Mode == 2)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
				Gizmos.DrawLine(base.transform.position, cameraParameters.Target);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
				Gizmos.DrawWireSphere(cameraParameters.Target, 0.5f);
				Gizmos.DrawLine(cameraParameters.Position, cameraParameters.Target);
			}
			if (cameraParameters.Mode == 3 || cameraParameters.Mode == 30 || cameraParameters.Mode == 31)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, (!cameraParameters.ObjPosition) ? cameraParameters.Position : cameraParameters.ObjPosition.position);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere((!cameraParameters.ObjPosition) ? cameraParameters.Position : cameraParameters.ObjPosition.position, 0.5f);
				Gizmos.DrawLine((!cameraParameters.ObjPosition) ? cameraParameters.Position : cameraParameters.ObjPosition.position, (!cameraParameters.ObjTarget) ? cameraParameters.Target : cameraParameters.ObjTarget.position);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere((!cameraParameters.ObjTarget) ? cameraParameters.Target : cameraParameters.ObjTarget.position, 1f);
			}
			if (cameraParameters.Mode == 4 || cameraParameters.Mode == 40 || cameraParameters.Mode == 41 || cameraParameters.Mode == 42)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
			}
		}
	}
}

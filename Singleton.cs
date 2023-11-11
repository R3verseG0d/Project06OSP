using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	private static bool applicationIsQuitting = false;

	public static T Instance
	{
		get
		{
			T result;
			if (applicationIsQuitting)
			{
				Debug.LogWarning(string.Concat("[Singleton] Instance '", typeof(T), "' already destroyed on application quit. Won't create again - returning null."));
				result = null;
			}
			else
			{
				lock (_lock)
				{
					if ((Object)_instance == (Object)null)
					{
						_instance = (T)Object.FindObjectOfType(typeof(T));
						if (Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
							result = _instance;
							return result;
						}
						if ((Object)_instance == (Object)null)
						{
							GameObject gameObject = new GameObject();
							_instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T).ToString();
							Object.DontDestroyOnLoad(gameObject);
							Debug.Log(string.Concat("[Singleton] An instance of ", typeof(T), " is needed in the scene, so '", gameObject, "' was created with DontDestroyOnLoad."));
						}
						else
						{
							Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
						}
					}
					result = _instance;
				}
			}
			return result;
		}
	}

	public void OnDestroy()
	{
		applicationIsQuitting = true;
	}
}

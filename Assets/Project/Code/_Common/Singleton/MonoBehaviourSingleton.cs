using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {
	private static object _lock = new object();
	private static bool _applicationIsQuitting = false;
	
	private static T _instance;
	public static T Instance {
		get {
			if (_applicationIsQuitting) {
				Debug.LogWarning(string.Format("[MonoBehaviourSingleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T)));
				return null;
			}
			
			lock(_lock) {
				if (_instance == null) {
					_instance = CreateInstance();
				}
				
				return _instance;
			}
		}
	}
	
	private static T CreateInstance() {
		T sInstance = FindObjectOfType<T>();
 
		if (FindObjectsOfType<T>().Length > 1) {
			Debug.LogError("[MonoBehaviourSingleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
		}

		if (sInstance == null) {
			GameObject goInstance = new GameObject();
			sInstance = goInstance.AddComponent<T>();
		}

		sInstance.gameObject.name = "(singleton) " + typeof(T).ToString();
		DontDestroyOnLoad(sInstance.gameObject);
		
		return sInstance;
	}
 
	public virtual void OnDestroy () {
		_applicationIsQuitting = true;
	}
}
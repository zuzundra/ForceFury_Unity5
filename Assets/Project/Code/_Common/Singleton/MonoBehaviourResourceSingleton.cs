using System;
using UnityEngine;

//ATTENTION!
//private static string field "_path" must be defined in every inheritor (path to prefab in resources folder)
public abstract class MonoBehaviourResourceSingleton<T> : MonoBehaviour where T : MonoBehaviour {
	private static object _lock = new object();
	private static bool _applicationIsQuitting = false;
	
	private static T _instance;
	public static T Instance {
		get {
			if (_applicationIsQuitting) {
				Debug.LogWarning(string.Format("[MonoBehaviourResourceSingleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T)));
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
			Debug.LogError("[MonoBehaviourResourceSingleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
		}

		if (sInstance == null) {
			GameObject goInstance = null;

			System.Reflection.FieldInfo fiPath = typeof(T).GetField("_path", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			if (fiPath != null) {
				string path = fiPath.GetValue(null).ToString();
				GameObject goPrefab = Resources.Load(path) as GameObject;
				if (goPrefab != null) {
					if (goPrefab.GetComponent<T>() != null) {
						goInstance = GameObject.Instantiate(goPrefab) as GameObject;
						sInstance = goInstance.GetComponent<T>();
					} else {
						Debug.LogError(string.Format("[MonoBehaviourResourceSingleton] Can't find component \"{0}\" on loaded prefab", typeof(T)));
					}
				} else {
					Debug.LogError(string.Format("[MonoBehaviourResourceSingleton] Can't load resource: \"{0}\"", path));
				}
			} else {
				Debug.LogError(string.Format("[MonoBehaviourResourceSingleton] Can't find \"Path\" static field in class \"{0}\"", typeof(T)));
			}
			
			if(goInstance == null) {
				goInstance = new GameObject();
				sInstance = goInstance.AddComponent<T>();
			}
		}

		sInstance.gameObject.name = "(singleton) " + typeof(T).ToString();
		DontDestroyOnLoad(sInstance.gameObject);
		
		return sInstance;
	}
 
	public virtual void OnDestroy () {
		if (_instance != null) {
			_applicationIsQuitting = true;
		}
	}
}
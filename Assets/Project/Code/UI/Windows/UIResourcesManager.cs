using UnityEngine;
using System.Collections.Generic;

public class UIResourcesManager {
	private class ResourceUsageInfo {
		public Object resource = null;
		public int usageAmount = 0;

		public ResourceUsageInfo(Object res, int initialUsages) {
			resource = res;
			usageAmount = initialUsages;
		}
	}

	#region singleton
	private static UIResourcesManager _instance;
	public static UIResourcesManager Instance {
		get {
			if (_instance == null) {
				_instance = new UIResourcesManager();
			}
			return _instance;
		}
	}
	#endregion

	private Dictionary<string, ResourceUsageInfo> _loadedResources = new Dictionary<string, ResourceUsageInfo>();

	public T GetResource<T>(string path) where T : Object {
		if (_loadedResources.ContainsKey(path) && _loadedResources[path].resource != null) {
            _loadedResources[path].usageAmount++;
			return (T)_loadedResources[path].resource;
		}

		Object resource = Resources.Load(path, typeof(T)); 
		if (resource != null) {
			if (resource is T) {
				_loadedResources.Add(path, new ResourceUsageInfo(resource, 1));
				return (T)resource;
			} else {
				Debug.LogError(string.Format("Resource at path \"{0}\" is not a resource of \"{1}\" type", path, typeof(T)));
			}
		} else {
			Debug.LogError(string.Format("Attempt to load \"{0}\" resource failed - resource not found", path));
		}

		return null;
	}

	public void FreeResource(string path) {
		if (_loadedResources.ContainsKey(path)) {
			if (_loadedResources[path].resource == null) {
				_loadedResources.Remove(path);
			} else {
				_loadedResources[path].usageAmount--;
				if (_loadedResources[path].usageAmount <= 0) {
					if (_loadedResources[path].resource != null) {
						Resources.UnloadAsset(_loadedResources[path].resource);
					}
					_loadedResources.Remove(path);
				}
			}
		}
	}

	public void FreeResource(object resource) {
		string path = string.Empty;
		foreach (KeyValuePair<string, ResourceUsageInfo> kvp in _loadedResources) {
			if (kvp.Value.resource == resource) {
				path = kvp.Key;
				break;
			}
		}

		if (!path.Equals(string.Empty)) {
			FreeResource(path);
		}
	}

	public void Clear() {
		foreach (KeyValuePair<string, ResourceUsageInfo> kvp in _loadedResources) {
			if (kvp.Value != null && kvp.Value.resource != null) {
				Resources.UnloadAsset(kvp.Value.resource);
			}
		}
		_loadedResources.Clear();
	}
}

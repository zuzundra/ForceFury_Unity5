using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesSwitcher : MonoBehaviourSingleton<ScenesSwitcher> {
	private class SceneLoadAction {
		public Action action = null;
		public int framesSkip = 0;
		public float waitForSeconds = 0f;

		public SceneLoadAction(Action a, int fs, float wfs) {
			action = a;
			framesSkip = fs;
			waitForSeconds = wfs;
		}
	}

	private List<SceneLoadAction> _loadActionsList = new List<SceneLoadAction>();

	public void OnLevelWasLoaded(int levelIndex) {
		if (_loadActionsList.Count != 0) {
			for (int i = 0; i < _loadActionsList.Count; i++) {
				StartCoroutine(RunTask(_loadActionsList[i]));
			}
			_loadActionsList.Clear();
		}
	}

	public override void OnDestroy() {
		base.OnDestroy();
		StopAllCoroutines();
		_loadActionsList.Clear();
	}

	public void AddLevelLoadCallback(Action callback, int framesSkip) {
		if (callback != null) {
			_loadActionsList.Add(new SceneLoadAction(callback, framesSkip, 0f));
		}
	}

	public void AddLevelLoadCallback(Action callback, float waitForSeconds) {
		if (callback != null) {
			_loadActionsList.Add(new SceneLoadAction(callback, 0, waitForSeconds));
		}
	}

	private IEnumerator RunTask(SceneLoadAction loadAction) {
		if (loadAction.framesSkip != 0) {
			for (int i = 0; i < loadAction.framesSkip; i++) {
				yield return null;
			}
		} else if(loadAction.waitForSeconds != 0f) {
			yield return new WaitForSeconds(loadAction.waitForSeconds);
		}

		loadAction.action();
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitDialogs : MonoBehaviourResourceSingleton<UnitDialogs> {
#pragma warning disable 0414
	private static string _path = "Config/FightDialogs";
#pragma warning restore 0414

	[SerializeField]
	private UnitsDialogScene[] _data;

	public UnitsDialogScene GetScene(EMissionKey missionKey, int mapIndex) {
		for (int i = 0; i < _data.Length; i++) {
			if (_data[i].MissionKey == missionKey && _data[i].MapIndex == mapIndex) {
				return _data[i];
			}
		}
		return null;
	}

	#region playing
	private UnitsDialogScene _missionScene = null;
	private Action _callback = null;
	private int _sceneActionIndex = -1;

	private Dictionary<string, UnitMonolog> _monologInstances = null;
	private UnitMonolog _activeMonologInstance = null;

	public void Play(EMissionKey missionKey, int mapIndex, Action callback) {
		UnitsDialogScene missionScene = GetScene(missionKey, mapIndex);
		if (missionScene == null) {
			if (callback != null) {
				callback();
			}
			return;
		}

		PlayInternal(missionScene, callback);
	}

	private void PlayInternal(UnitsDialogScene missionScene, Action callback) {
		_missionScene = missionScene;
		_callback = callback;
		_sceneActionIndex = -1;

		_monologInstances = new Dictionary<string, UnitMonolog>();
		for (int i = 0; i < _missionScene.DialogData.Length; i++) {
			if (!_monologInstances.ContainsKey(_missionScene.DialogData[i].PrefabPath)) {
				_monologInstances.Add(_missionScene.DialogData[i].PrefabPath, (GameObject.Instantiate(Resources.Load(_missionScene.DialogData[i].PrefabPath)) as GameObject).GetComponent<UnitMonolog>());
			}
		}

		PlayNext();
	}

	private void PlayNext() {
		if (_activeMonologInstance != null) {
			_activeMonologInstance.Hide();
		}

		_sceneActionIndex++;
		if (_missionScene.DialogData.Length > _sceneActionIndex) {
			_activeMonologInstance = _monologInstances[_missionScene.DialogData[_sceneActionIndex].PrefabPath];
			_activeMonologInstance.Show(_missionScene.DialogData[_sceneActionIndex], PlayNext);
		} else {
			End();
		}
	}

	private void End() {
		Action callback = _callback;

		_missionScene = null;
		_callback = null;
		_sceneActionIndex = -1;

		_activeMonologInstance = null;
		foreach (KeyValuePair<string, UnitMonolog> kvp in _monologInstances) {
			if (kvp.Value != null) {
				GameObject.Destroy(kvp.Value.gameObject);
			}
		}
		_monologInstances.Clear();
		_monologInstances = null;

		callback();
	}
	#endregion
}

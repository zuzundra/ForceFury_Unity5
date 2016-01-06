using UnityEngine;
using System;
using System.Collections;

public class GameTimer : MonoBehaviourSingleton<GameTimer> {
	private class TimerListener {
		public int TimeLeft { get; set; }
		public Action Callback { get; set; }

		public TimerListener(int timeLeft, Action callback) {
			TimeLeft = timeLeft;
			Callback = callback;
		}
	}

	#region unity funcs
	public void Awake() {
		ExtendCallbacksList();
	}

	public void Update() {
		_oneSecond += Time.deltaTime;
		if (_oneSecond >= 1f) {
			_oneSecond -= 1f;

			if (_updateCallbacks != null) {
				TimerTick();
			}
		}
	}

	public override void OnDestroy() {
		_updateCallbacks = null;
		StopAllCoroutines();
		base.OnDestroy();
	}
	#endregion

	#region one-second timer
	private TimerListener[] _updateCallbacks;
	private float _oneSecond = 0f;

	public void AddListener(int timeLeft, Action callback) {
		for (int i = 0; i < _updateCallbacks.Length; i++) {
			if (_updateCallbacks[i] == null) {
				_updateCallbacks[i] = new TimerListener(timeLeft, callback);
				return;
			}
		}

		ExtendCallbacksList();
		AddListener(timeLeft, callback);
	}

	public void RemoveListener(Action callback) {
		for (int i = 0; i < _updateCallbacks.Length; i++) {
			if (_updateCallbacks[i].Callback == callback) {
				_updateCallbacks[i] = null;
			}
		}
	}

	private void TimerTick() {
		for (int i = 0; i < _updateCallbacks.Length; i++) {
			if (_updateCallbacks[i] != null) {
				_updateCallbacks[i].TimeLeft--;
				if (_updateCallbacks[i].TimeLeft <= 0) {
					Action callback = _updateCallbacks[i].Callback;
					_updateCallbacks[i] = null;
					callback();
				}
			}
		}
	}

	private void ExtendCallbacksList() {
		if (_updateCallbacks == null) {
			_updateCallbacks = new TimerListener[10];
		} else {
			TimerListener[] newCallbacks = new TimerListener[_updateCallbacks.Length + 10];
			Array.Copy(_updateCallbacks, newCallbacks, _updateCallbacks.Length);
			_updateCallbacks = newCallbacks;
		}
	}
	#endregion

	#region coroutines for external usage
	public void RunCoroutine(IEnumerator func) {
		StartCoroutine(func);
	}

	public void FinishCoroutine(IEnumerator func) {
		StopCoroutine(func);
	}
	#endregion
}

using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Animation))]
public class SkillStunGrenadeView : MonoBehaviour {
	[SerializeField]
	private ParticleSystem _particlesPrefab;

	private Animation _animation;

	private Action<Vector3> _callback = null;

	private Transform _cachedTransform;

	public bool IsInFlight {
		get { return _animation.isPlaying; }
	}

	public void Awake() {
		_cachedTransform = transform;

		_animation = gameObject.GetComponent<Animation>();
		_animation.Stop();

		EventsAggregator.Fight.AddListener(EFightEvent.MapComplete, OnMapEnd);
		EventsAggregator.Fight.AddListener(EFightEvent.MapFail, OnMapEnd);
	}

	public void OnDestroy() {
		EventsAggregator.Fight.RemoveListener(EFightEvent.MapComplete, OnMapEnd);
		EventsAggregator.Fight.RemoveListener(EFightEvent.MapFail, OnMapEnd);
	}

	//public void OnGUI() {
	//	if (GUI.Button(new Rect(5, 100, 100, 50), "Throw")) {
	//		Throw(2f, transform.position, transform.position + new Vector3(2f, 0f, 0f), 0.5f);
	//	}
	//}

	public void Throw(float time, Vector3 startPosition, Vector3 targetPosition, float height, Action<Vector3> callback) {
		_callback = callback;

		gameObject.SetActive(true);

		float tangentX = (targetPosition.x - startPosition.x) / time;
		float tangentY = height * 4f / time;
		float tangentZ = (targetPosition.z - startPosition.z) / time;

		AnimationClip clip = _animation.GetClip("Throw");
		if(clip == null) {
			_animation.AddClip(new AnimationClip(), "Throw");
			clip = _animation.GetClip("Throw");
		}
		clip.ClearCurves();

		AnimationCurve cX = new AnimationCurve(new Keyframe(0f, startPosition.x, 0f, tangentX), new Keyframe(time * 0.5f, (targetPosition.x + startPosition.x) * 0.5f, tangentX, tangentX), new Keyframe(time, targetPosition.x, tangentX, 0f));
		clip.SetCurve("", typeof(Transform), "localPosition.x", cX);

		AnimationCurve cY = new AnimationCurve(new Keyframe(0f, startPosition.y, 0f, tangentY), new Keyframe(time * 0.5f, startPosition.y + height), new Keyframe(time, 0f + 0.1f, -tangentY, 0f));
		clip.SetCurve("", typeof(Transform), "localPosition.y", cY);

		AnimationCurve cZ = new AnimationCurve(new Keyframe(0f, startPosition.z, 0f, tangentZ), new Keyframe(time * 0.5f, (targetPosition.z + startPosition.z) * 0.5f, tangentZ, tangentZ), new Keyframe(time, targetPosition.z, tangentZ, 0f));
		clip.SetCurve("", typeof(Transform), "localPosition.z", cZ);

		_animation.Play("Throw");
		_animation.wrapMode = WrapMode.Once;

		Invoke("OnFlightEnd", time);
	}

	public void Stop() {
		if (IsInvoking("OnFlightEnd")) {
			CancelInvoke("OnFlightEnd");
		}

		if (_animation.isPlaying) {
			_animation.Stop("Throw");
		}

		gameObject.SetActive(false);
	}

	private void OnFlightEnd() {
		if (_callback != null) {
			_callback(_cachedTransform.position);
			_callback = null;
		}

		Stop();

		ParticleSystem ps = (GameObject.Instantiate(_particlesPrefab.gameObject) as GameObject).GetComponent<ParticleSystem>();
		ps.transform.position = transform.position;
	}

	private void OnMapEnd() {
		Stop();
	}
}

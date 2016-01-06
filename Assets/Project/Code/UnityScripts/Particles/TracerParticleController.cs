using System;
using System.Collections;
using UnityEngine;

public class TracerParticleController : MonoBehaviour {
	private float _speed = 18f;

	private float _timeEnd;
	private Vector3 _positionEnd;

	private Transform _cachedTransform;

	private Action<Vector3> _flightEndCallback;

	private Mesh[] _meshes;

	public void Awake() {
		_cachedTransform = transform;

		_meshes = new Mesh[_cachedTransform.childCount];
		for (int i = 0; i < _cachedTransform.childCount; i++) {
			_meshes[i] = _cachedTransform.GetChild(i).GetComponent<MeshFilter>().mesh;
		}
	}

	public void Update() {
		_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, _positionEnd, _speed * Time.deltaTime);
		if (Time.time >= _timeEnd) {
            Blow();
		}
	}

	public void Play(float distance, Vector3 startPosition, Action<Vector3> flightEndCallback) {
		_flightEndCallback = flightEndCallback;

		_cachedTransform.position = startPosition;
		_positionEnd = _cachedTransform.position + _cachedTransform.forward * distance;
		_timeEnd = Time.time + distance / _speed;

		gameObject.SetActive(true);
	}

	private void Blow() {
		Stop();

		if (_flightEndCallback != null) {
			_flightEndCallback(_positionEnd);
			_flightEndCallback = null;
		}
	}

	public void Stop() {
		gameObject.SetActive(false);
	}

	public void UpdateColor(Color color) {
		for (int i = 0; i < _meshes.Length; i++) {
			Vector3[] vertices = _meshes[i].vertices;
			Color[] colors = new Color[vertices.Length];

			for (var j = 0; j < vertices.Length; j++) {
				colors[j] = color;
			}

			_meshes[i].colors = colors;
		}
	}
}

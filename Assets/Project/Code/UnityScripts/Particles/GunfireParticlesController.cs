using UnityEngine;
using System.Collections;
using System;

public class GunfireParticlesController : MonoBehaviour {
	private static Transform _explosionsRoot;
	private static Transform ExplosionsRoot {
		get {
			if (_explosionsRoot == null) {
				_explosionsRoot = new GameObject("Explosions").transform;
			}
			return _explosionsRoot;
		}
	}

	[SerializeField]
	private GameObject _gunfireParticlePrefab;
	[SerializeField]
	private GameObject _tracerParticlePrefab;
	[SerializeField]
	private GameObject _explosionPrefab;
	
	[SerializeField]
	private int _pelletsPerAttack = 1;
	[SerializeField]
	private float _firstPelletDelay = 0f;
	[SerializeField]
	private float _nextPelletDelay = 0f;

	[SerializeField]
	private float _pelletWorldLength = 0;

	[SerializeField]
	private Color _defaultProjectileColor = Color.white;

	private float _pelletStartOffset = 0f;
	private int _pelletExtendAmount = 4;

	private GameObject _gunfireParticleInstance;
	private TracerParticleController[] _tracerParticleInstances;
	private GameObject _explosionInstance;

	private WaitForSeconds _wfsGunfireDuration;
	private WaitForSeconds _wfsFirstPellek;
	private WaitForSeconds _wfsNextPellek;
	private WaitForSeconds _wfsExplosionDuration;

	private Transform _tracersRoot;

	private Coroutine _explosionRoutine;

	public void Awake() {
        _wfsGunfireDuration = new WaitForSeconds(0.035f);
		if (_firstPelletDelay > 0f) {
			_wfsFirstPellek = new WaitForSeconds(_firstPelletDelay);
		}
		if(_nextPelletDelay > 0f) {
			_wfsNextPellek = new WaitForSeconds(_nextPelletDelay);
		}
		_wfsExplosionDuration = new WaitForSeconds(0.085f);
	}

	public void Setup(Transform gunfireRoot, Transform tracersRoot, float pelletStartOffset) {
		_tracersRoot = tracersRoot;
		_pelletStartOffset = pelletStartOffset;

		_gunfireParticleInstance = (GameObject.Instantiate(_gunfireParticlePrefab) as GameObject);
		_gunfireParticleInstance.transform.SetParent(gunfireRoot);
		_gunfireParticleInstance.transform.localPosition = Vector3.zero;
		_gunfireParticleInstance.transform.localRotation = Quaternion.identity;
		_gunfireParticleInstance.SetActive(false);

		ExtendPelletsPool();

		_explosionInstance = (GameObject.Instantiate(_explosionPrefab) as GameObject);
		_explosionInstance.transform.SetParent(ExplosionsRoot);
		_explosionInstance.SetActive(false);
	}

	public void Play(float distanceToTarget) {
		Play(distanceToTarget, _tracersRoot.position);
	}

	public void Play(float distanceToTarget, Vector3 startPosition) {
		distanceToTarget = distanceToTarget - _pelletStartOffset - _pelletWorldLength;

		StartCoroutine(PlayInternal(distanceToTarget, startPosition));
	}

	public void Stop() {
		StopAllCoroutines();

		_gunfireParticleInstance.SetActive(false);
		for (int i = 0; i < _tracerParticleInstances.Length; i++) {
			_tracerParticleInstances[i].gameObject.SetActive(false);
		}
		_explosionInstance.SetActive(false);
	}

	public void UpdateProjectileColor(Color color) {
		for (int i = 0; i < _tracerParticleInstances.Length; i++) {
			_tracerParticleInstances[i].UpdateColor(color);
		}
	}

	public void ResetProjectileColor() {
		UpdateProjectileColor(_defaultProjectileColor);
	}

	private IEnumerator PlayInternal(float distanceToTarget, Vector3 pelletStartPosition) {
		TracerParticleController currentPellet = null;

		if (_wfsFirstPellek != null) {
			yield return _wfsFirstPellek;
		}

		for (int i = 0; i < _pelletsPerAttack; i++) {
			currentPellet = GetFreePellet();
            if (currentPellet == null)
            {
                ExtendPelletsPool();
                currentPellet = GetFreePellet();
            }
			currentPellet.Play(distanceToTarget, pelletStartPosition, PlayExplosion);
			_gunfireParticleInstance.SetActive(true);
            yield return _wfsGunfireDuration;
			_gunfireParticleInstance.SetActive(false);
			if (_wfsNextPellek != null) {
				yield return _wfsNextPellek;
			}
		}
    }

	private void PlayExplosion(Vector3 position) {
		if (_explosionRoutine != null) {
			StopCoroutine(_explosionRoutine);
		}
		_explosionRoutine = StartCoroutine(PlayExplosionInternal(position));
	}

	private IEnumerator PlayExplosionInternal(Vector3 position) {
		_explosionInstance.transform.position = position;
		_explosionInstance.SetActive(true);
		yield return _wfsExplosionDuration;
		_explosionInstance.SetActive(false);

		_explosionRoutine = null;
	}

	private void ExtendPelletsPool() {
		int i = 0;

		TracerParticleController[] pellets = null;
		if(_tracerParticleInstances != null) {
			pellets = new TracerParticleController[_tracerParticleInstances.Length + _pelletExtendAmount];
			Array.Copy(_tracerParticleInstances, pellets, _tracerParticleInstances.Length);
			i = _tracerParticleInstances.Length;
		} else {
			pellets = new TracerParticleController[_pelletExtendAmount];
		}

		GameObject tracerInstance = null;
		for (; i < pellets.Length; i++) {
			tracerInstance = (GameObject.Instantiate(_tracerParticlePrefab) as GameObject);
			tracerInstance.transform.SetParent(_tracersRoot);
			tracerInstance.transform.localPosition = Vector3.zero;
			tracerInstance.transform.localRotation = Quaternion.identity;

			pellets[i] = tracerInstance.AddComponent<TracerParticleController>();
			pellets[i].Stop();
		}

		_tracerParticleInstances = pellets;
	}

	private TracerParticleController GetFreePellet() {
		for (int i = 0; i < _tracerParticleInstances.Length; i++) {
			if (!_tracerParticleInstances[i].gameObject.activeInHierarchy) {
				return _tracerParticleInstances[i];
			}
		}
		return null;
	}
}

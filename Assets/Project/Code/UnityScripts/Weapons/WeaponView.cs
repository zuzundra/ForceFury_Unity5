using System.Collections;
using UnityEngine;

public class WeaponView : MonoBehaviour {
	[SerializeField]
	private Transform _tracerParticleParent;
	[SerializeField]
	private Transform _gunfireParticleParent;
	public Transform GunfireParticleParent {
		get { return _gunfireParticleParent; }
	}

	[SerializeField]
	private GunfireParticlesController _particlesController;

	public void Setup(Transform tracersParent) {
		float qwe = tracersParent.InverseTransformPoint(_tracerParticleParent.TransformPoint(_tracerParticleParent.localPosition)).x;
		_tracerParticleParent.SetParent(tracersParent);
		_particlesController.Setup(_gunfireParticleParent, _tracerParticleParent, qwe);
	}

	public void PlayShot(float distanceToTarget) {
		if (distanceToTarget > 0f) {
			_particlesController.Play(distanceToTarget);
		}
	}

	public void PlayShotFromPosition(float distanceToTarget, Vector3 position) {
		if (distanceToTarget > 0f) {
			_particlesController.Play(distanceToTarget, position);
		}
	}

	public void StopShot() {
		_particlesController.Stop();
	}

	public void UpdateProjectileColor(Color color) {
		_particlesController.UpdateProjectileColor(color);
	}

	public void ResetProjectileColor() {
		_particlesController.ResetProjectileColor();
	}
}

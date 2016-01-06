using UnityEngine;
using System.Collections;

public class SkillExplosiveChargesView : MonoBehaviour {
	[SerializeField]
	private ParticleSystem _particlePrefab;

	[SerializeField]
	private Color _projectilesColor = Color.red;

	private ParticleSystem _particleInstance;
	private BaseUnitBehaviour _caster;

	public void Awake() {
		_particleInstance = (GameObject.Instantiate(_particlePrefab.gameObject) as GameObject).GetComponent<ParticleSystem>();
		_particleInstance.transform.SetParent(transform);
		_particleInstance.playOnAwake = false;
		_particleInstance.Stop();
		_particleInstance.gameObject.GetComponent<CFX_AutoDestructShuriken>().OnlyDeactivate = true;
		_particleInstance.gameObject.SetActive(false);
	}

	public void OnDestroy() {
		_caster = null;

		if (_particleInstance != null) {
			GameObject.Destroy(_particleInstance.gameObject);
			_particleInstance = null;
		}

		EventsAggregator.Fight.RemoveListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnAttack);
	}

	public void Run(BaseUnitBehaviour caster) {
		_caster = caster;
		transform.SetParent(_caster.transform);
		transform.localPosition = Vector3.zero;

		_caster.ModelView.UpdateProjectileColor(_projectilesColor);

		EventsAggregator.Fight.AddListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnAttack);
	}

	public void End() {
		_caster.ModelView.ResetProjectileColor();

		StopAllCoroutines();
		GameObject.Destroy(gameObject);
	}

	private void OnAttack(BaseUnitBehaviour attacker, BaseUnitBehaviour target) {
		if (attacker == _caster) {
			_particleInstance.transform.localPosition = _particleInstance.transform.parent.InverseTransformPoint(target.transform.position) + new Vector3(0f, target.ModelView.ModelHeight * 0.5f, 0f);
			_particleInstance.gameObject.SetActive(true);
			_particleInstance.Simulate(0f);
			_particleInstance.Play(true);
		}
	}
}

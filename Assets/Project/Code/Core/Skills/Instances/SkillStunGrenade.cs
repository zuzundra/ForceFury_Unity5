using System.Collections;
using UnityEngine;

public class SkillStunGrenade : BaseUnitSkill {
	private string _grenadePrefabPath = "Skills/StunGrenade_grenade";
	private float _minThrowTime = 0.65f;

	private SkillStunGrenadeView _grenadeView = null;

	public SkillStunGrenade(SkillParameters skillParameters) : base(skillParameters) { }

	public override void Use(BaseUnitBehaviour caster) {
		//check caster is alive
		if (caster.UnitData.IsDead) {
			return;
		}

		//check aggro price
		BaseHero heroData = caster.UnitData as BaseHero;
		if (heroData.AggroCrystals < _skillParameters.AggroCrystalsCost) {
			return;
		}

		//check cooldown
		if (_lastUsageTime != 0f && Time.time - _lastUsageTime < _skillParameters.CooldownTime) {
			return;
		}

		//check already in use
		if (_isUsing) {
			return;
		}

		base.Use(caster);
		StartUsage();
	}

	public override void Break() {
		base.Break();
	}

	protected override void StartUsage() {
		BaseUnitBehaviour target = GetFarthestOpponent();
		if (target != null) {
			CreateGrenade();
			if (_grenadeView != null && !_grenadeView.IsInFlight) {
				(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
				StartCooldown();
				_isUsing = true;

				GameTimer.Instance.RunCoroutine(ThrowGrenade(target));
			}
		}
	}

	protected override void EndUsage() {
		_caster = null;
		_isUsing = false;
	}

	public override void OnCasterStunned() { }

	public override void OnCasterDeath() { }

	public override void OnCasterTargetDeath() { }

	private BaseUnitBehaviour GetFarthestOpponent() {
		BaseUnitBehaviour result = null;
		ArrayRO<BaseUnitBehaviour> opposedUnits = _caster.IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits;
		for (int i = 0; i < opposedUnits.Length; i++) {
			if (opposedUnits[i] != null && !opposedUnits[i].UnitData.IsDead) {
				if (result == null) {
					result = opposedUnits[i];
				} else if (Vector3.Distance(_caster.CachedTransform.position, opposedUnits[i].CachedTransform.position) > Vector3.Distance(_caster.CachedTransform.position, result.CachedTransform.position)) {
					result = opposedUnits[i];
				}
			}
		}
		return result;
	}

	private void CreateGrenade() {
		if (_grenadeView == null) {
			GameObject grenadeGO = GameObject.Instantiate(Resources.Load(_grenadePrefabPath) as GameObject) as GameObject;
			_grenadeView = grenadeGO.GetComponent<SkillStunGrenadeView>();
			if (_grenadeView == null) {
				grenadeGO.AddComponent<SkillStunGrenadeView>();
			}
			_grenadeView.transform.SetParent(_caster.CachedTransform.parent);
		}
		_grenadeView.transform.position = _caster.ModelView.WeaponBoneRight.position;
		_grenadeView.gameObject.SetActive(false);
	}

	private IEnumerator ThrowGrenade(BaseUnitBehaviour target) {
		_caster.StopTargetAttack(false);
		_caster.ModelView.PlaySkillAnimation(ESkillKey.StunGrenade);

		yield return new WaitForSeconds(0.55f);

		_grenadeView.Throw(Mathf.Max(Vector3.Distance(_caster.CachedTransform.position, target.CachedTransform.position) * 0.1f, _minThrowTime), _grenadeView.transform.position, target.CachedTransform.position, 2f, OnGrenadeTargetReached);

		yield return new WaitForSeconds(0.3f);

		_caster.StartTargetAttack();
	}

	private void OnGrenadeTargetReached(Vector3 position) {
		ArrayRO<BaseUnitBehaviour> opposedUnits = _caster.IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits;
		for (int i = 0; i < opposedUnits.Length; i++) {
			if (opposedUnits[i] != null && !opposedUnits[i].UnitData.IsDead) {
				if (Vector3.Distance(position, opposedUnits[i].CachedTransform.position) <= _skillParameters.Radius) {
					opposedUnits[i].Stun(_skillParameters.Duration);
				}
			}
		}

		EndUsage();
	}
}

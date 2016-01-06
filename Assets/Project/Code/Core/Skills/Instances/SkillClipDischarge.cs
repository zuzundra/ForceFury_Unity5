using System.Collections;
using UnityEngine;

public class SkillClipDischarge : BaseUnitSkill {
	private string _viewPrefabPath = "Skills/ClipDischarge_tracers";

	private SkillClipDischargeView _skillView = null;

	public SkillClipDischarge(SkillParameters skillParameters) : base(skillParameters) { }

	private int _shotsLeft = -1;

	private WaitForSeconds _wfs = null;

	public override void Use(BaseUnitBehaviour caster) {
		//check caster is alive
		if (caster.UnitData.IsDead) {
			return;
		}

		//check aggro price
		if ((caster.UnitData as BaseHero).AggroCrystals < _skillParameters.AggroCrystalsCost) {
			return;
		}

        //check target locked
        if (caster.UnitAttack.State != EUnitAttackState.WatchTarget)
        //if (caster.UnitPathfinder.CurrentState != EUnitMovementState.WatchEnemy)
                return;

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

		GameTimer.Instance.FinishCoroutine(Shoot());
		Clear();
	}

	protected override void StartUsage() {
		base.StartUsage();

		_shotsLeft = (int)_skillParameters.Duration;
		_wfs = new WaitForSeconds(0.1f);			//duration betweeen attacks
		StartCooldown();

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_caster.StopTargetAttack(true);

		GameObject skillViewResource = Resources.Load(_viewPrefabPath) as GameObject;
		if (skillViewResource != null) {
			_skillView = (GameObject.Instantiate(skillViewResource) as GameObject).GetComponent<SkillClipDischargeView>();
		}

		GameTimer.Instance.RunCoroutine(Shoot());
	}

	protected override void EndUsage() {
		if (_caster != null) {
			base.EndUsage();

			if (_skillView != null) {
				GameObject.Destroy(_skillView.gameObject);
				_skillView = null;
			}

			BaseUnitBehaviour caster = _caster;

			GameTimer.Instance.FinishCoroutine(Shoot());
			Clear();

			caster.StartTargetAttack();
		}
	}

	protected override void Clear() {
		base.Clear();

		_shotsLeft = -1;
		_wfs = null;
	}

	public override void OnCasterStunned() {
		Break();
	}

	public override void OnCasterDeath() {
		Break();
	}

	public override void OnCasterTargetDeath() {
		EndUsage();
	}

	#region shooting
	private IEnumerator Shoot() {
		_caster.ModelView.PlaySkillAnimation(ESkillKey.ClipDischarge, _caster.DistanceToTarget);

		yield return new WaitForSeconds(0.25f);
		if (_skillView != null) {
			_skillView.StoreWeaponPosition(_caster);
		}
		while (_shotsLeft > 0) {
			PerformShot();
			_shotsLeft--;
			if (_shotsLeft > 0) {
				yield return _wfs;
			}
		}
		yield return new WaitForSeconds(0.6f);

		EndUsage();
	}

	private void PerformShot() {
		EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, _caster, _caster.TargetUnit);
		if (_skillView != null) {
			_skillView.Play(_caster);
		}
	}
	#endregion
}

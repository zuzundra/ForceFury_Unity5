using System.Collections;
using UnityEngine;

public class SkillExplosiveCharges : BaseUnitSkill {
	private string _viewPrefabPath = "Skills/ExplosiveCharges_hitEffect";

	private SkillExplosiveChargesView _skillView = null;

	public SkillExplosiveCharges(SkillParameters skillParameters) : base(skillParameters) { }

	private int _shotsLeft = -1;

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
		GameTimer.Instance.FinishCoroutine(SkillPrepare());
		EndUsage();
	}

	protected override void StartUsage() {
		base.StartUsage();

		(_caster.UnitData as BaseHero).UseSkill(_skillParameters);
		_shotsLeft = (int)_skillParameters.Duration;

		EventsAggregator.Fight.AddListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);

		GameTimer.Instance.RunCoroutine(SkillPrepare());
	}

	protected override void EndUsage() {
		base.EndUsage();

		Clear();

		EventsAggregator.Fight.RemoveListener<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, OnUnitAttack);

		StartCooldown();
	}

	protected override void Clear() {
		base.Clear();

		if (_skillView != null) {
			_skillView.End();
			_skillView = null;
		}

		_shotsLeft = -1;
	}

	public override void OnCasterStunned() { }

	public override void OnCasterDeath() {
		Break();
	}

	public override void OnCasterTargetDeath() { }

	private IEnumerator SkillPrepare() {
        if (_caster.UnitAttack.State == EUnitAttackState.WatchTarget)
        //if (_caster.UnitPathfinder.CurrentState == EUnitMovementState.WatchEnemy)
                _caster.StopTargetAttack(true);
		_caster.CastingSkill = true;

		_caster.ModelView.PlaySkillAnimation(ESkillKey.ExplosiveCharges);
		yield return new WaitForSeconds(_skillParameters.CastTime);

		GameObject skillViewGO = GameObject.Instantiate(Resources.Load(_viewPrefabPath) as GameObject) as GameObject;
		if (skillViewGO != null) {
			_skillView = skillViewGO.GetComponent<SkillExplosiveChargesView>();
			_skillView.Run(_caster);
		}

		_caster.CastingSkill = false;
        if (_caster.UnitAttack.State == EUnitAttackState.WatchTarget)
        //if (_caster.UnitPathfinder.CurrentState == EUnitMovementState.WatchEnemy)
                _caster.StartTargetAttack();
	}

	private void OnUnitAttack(BaseUnitBehaviour attacker, BaseUnitBehaviour target) {
		if (attacker == _caster) {
			AttackInfo attackInfo = _caster.UnitData.GetAttackInfo(true);//, false);			//aoe damage
			ArrayRO<BaseUnitBehaviour> opposedUnits = _caster.IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits;
			for (int i = 0; i < opposedUnits.Length; i++) {
				if (opposedUnits[i] !=null && !opposedUnits[i].UnitData.IsDead && Vector3.Distance(_caster.CachedTransform.position, opposedUnits[i].CachedTransform.position) <= _skillParameters.Radius) {
					opposedUnits[i].UnitData.ApplyDamage(attackInfo);
				}
			}

			_shotsLeft--;
			if (_shotsLeft == 0) {
				EndUsage();
			}
		}
	}
}

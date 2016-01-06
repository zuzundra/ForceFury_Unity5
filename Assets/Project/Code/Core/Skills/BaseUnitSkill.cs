using System;
public abstract class BaseUnitSkill {
	protected SkillParameters _skillParameters = null;
	public SkillParameters SkillParameters {
		get { return _skillParameters; }
	}

	protected BaseUnitBehaviour _caster = null;

	protected bool _isUsing = false;
	protected float _lastUsageTime = 0f;

	public BaseUnitSkill(SkillParameters skillParameters) {
		_skillParameters = skillParameters;
	}

	public virtual void Use(BaseUnitBehaviour caster) {
		_caster = caster;
	}

	public virtual void Break() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
		_isUsing = false;
	}

	protected virtual void StartUsage() {
		_caster.UnitData.ActiveSkills.RegisterSkill(this);
		_isUsing = true;
	}

	protected virtual void EndUsage() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
		_isUsing = false;
	}

	protected virtual void Clear() {
		_caster = null;
		_isUsing = false;
	}

	public virtual void OnCasterStunned() { }

	public virtual void OnCasterDeath() {
		_caster.UnitData.ActiveSkills.UnregisterSkill(this);
	}

	public virtual void OnCasterTargetDeath() { }

	public virtual void StartCooldown() {
		_lastUsageTime = UnityEngine.Time.time;
		if (_caster != null && _caster.UnitData == Global.Instance.Player.Heroes.Current) {
			EventsAggregator.UI.Broadcast<ESkillKey, float>(EUIEvent.StartSkillCooldown, _skillParameters.Key, _skillParameters.CooldownTime);
		}
	}
}

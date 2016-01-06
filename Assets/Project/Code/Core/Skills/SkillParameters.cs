using UnityEngine;

[System.Serializable]
public class SkillParameters {
	[SerializeField]
	protected ESkillKey _key = ESkillKey.None;
	public ESkillKey Key {
		get { return _key; }
	}

	[SerializeField]
	protected float _aggroCrystalsCost = 0f;
	public float AggroCrystalsCost {
		get { return _aggroCrystalsCost; }
	}

	[SerializeField]
	protected bool _targetSelectionRequired = false;
	public bool TargetSelectionRequired {
		get { return _targetSelectionRequired; }
	}

	[SerializeField]
	protected bool _canCastOnSelf = false;
	public bool CanCastOnSelf {
		get { return _canCastOnSelf; }
	}

	[SerializeField]
	protected ESkillTarget _skillTarget = ESkillTarget.None;
	public ESkillTarget SkillTarget {
		get { return _skillTarget; }
	}

	[SerializeField]
	protected float _castTime = 0f;
	public float CastTime {
		get { return _castTime; }
	}

	[SerializeField]
	protected float _cooldownTime = 0f;
	public float CooldownTime {
		get { return _cooldownTime; }
	}

	[SerializeField]
	protected bool _resetAttackTimer = true;
	public bool ResetAttackTimer {
		get { return _resetAttackTimer; }
	}

	[SerializeField]
	protected float _duration = 0f;	//can be used for buff duration in seconds, buff duration in shots, stun duration from grenade, etc
	public float Duration {
		get { return _duration; }
	}

	[SerializeField]
	protected float _radius = 0f;
	public float Radius {
		get { return _radius; }
	}

	[SerializeField]
	protected int _nativeDamage = 0;
	public int NativeDamage {
		get { return _nativeDamage; }
	}

	//unit buffs (direct)
	[SerializeField]
	protected int _modHealthAmount = 0;
	public int ModHealthAmount {
		get { return _modHealthAmount; }
	}

	[SerializeField]
	protected int _modArmorAmount = 0;
	public int ModArmorAmount {
		get { return _modArmorAmount; }
	}

	[SerializeField]
	protected int _modDamageAmount = 0;
	public int ModDamageAmount {
		get { return _modDamageAmount; }
	}

	[SerializeField]
	protected float _modDamageRangeAmount = 0;
	public float ModDamageRangeAmount {
		get { return _modDamageRangeAmount; }
	}

	[SerializeField]
	protected float _modDamageSpeedAmount = 0;
	public float ModDamageSpeedAmount {
		get { return _modDamageSpeedAmount; }
	}

	[SerializeField]
	protected int _modCritChanceAmount = 0;
	public int ModCritChanceAmount {
		get { return _modCritChanceAmount; }
	}

	[SerializeField]
	protected float _modCritDamageMultiplierAmount = 0;
	public float ModCritDamageMultiplierAmount {
		get { return _modCritDamageMultiplierAmount; }
	}

	//unit buffs (percents)
	[SerializeField]
	protected float _modHealthPercents = 0;
	public float ModHealthPercents {
		get { return _modHealthPercents; }
	}

	[SerializeField]
	protected float _modArmorPercents = 0;
	public float ModArmorPercents {
		get { return _modArmorPercents; }
	}

	[SerializeField]
	protected float _modDamagePercents = 0;
	public float ModDamagePercents {
		get { return _modDamagePercents; }
	}

	[SerializeField]
	protected float _modDamageRangePercents = 0;
	public float ModDamageRangePercents {
		get { return _modDamageRangePercents; }
	}

	[SerializeField]
	protected float _modDamageSpeedPercents = 0;
	public float ModDamageSpeedPercents {
		get { return _modDamageSpeedPercents; }
	}

	[SerializeField]
	protected float _modCritChancePercents = 0;
	public float ModCritChancePercents {
		get { return _modCritChancePercents; }
	}

	[SerializeField]
	protected float _modCritDamageMultiplierPercents = 0;
	public float ModCritDamageMultiplierPercents {
		get { return _modCritDamageMultiplierPercents; }
	}

	[SerializeField]
	protected string _iconPath = string.Empty;
	public string IconPath {
		get { return _iconPath; }
	}
}

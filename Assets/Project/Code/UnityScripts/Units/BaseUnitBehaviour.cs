using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BaseUnitBehaviour : MonoBehaviour, IComparable {
    [SerializeField]
    private UnitAttack _unitAttack;
    public UnitAttack UnitAttack
    {
        get { return _unitAttack; }
    }

	[SerializeField]
	private UnitModelView _model;
	public UnitModelView ModelView {
		get { return _model; }
	}
	
	[SerializeField]
	private Vector3 _healthBarPosition = new Vector3(0f, 1.2f, 0f);
	
	private UnitUI _ui;
	
	private BaseUnit _unitData = null;
	public BaseUnit UnitData {
		get { return _unitData; }
	}

    [SerializeField]
    protected UnitPlace _place = new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.None }; //base place on field
    public UnitPlace Place
    {
        get { return _place; }
        set { _place = value; }
    }
	
	private BaseUnitBehaviour _targetUnit, _lastTargetUnit;
	public BaseUnitBehaviour TargetUnit {
		get { return _targetUnit; }
	}
	
	private bool _isAlly = false;
	public bool IsAlly {
		get { return _isAlly; }
	}
	
	private float _attackTime = 0f;
	
	private WaitForSeconds _cachedWaitForSeconds;
    private float _lastAttackTime = 0f;

	private Coroutine _corTargetAttack;
	
	private Transform _cachedTransform = null;
	public Transform CachedTransform {
		get { return _cachedTransform; }
	}

	public float DistanceToTarget {
		get { return _targetUnit != null ? Vector3.Distance(_cachedTransform.position, _targetUnit.CachedTransform.position) : 0f; }
	}
    //public bool TargetInRange {
    //    get { return _targetUnit != null && DistanceToTarget <= _unitData.AR; }
    //}
	
	private Dictionary<ESkillKey, BaseUnitSkill> _skills;
	public bool CastingSkill { get; set; }
	
	public void Awake() {
		_cachedTransform = transform;
		
		if (_model == null) {
			_model = gameObject.GetComponentInChildren<UnitModelView>();
		}
		
		EventsAggregator.Units.AddListener<BaseUnit, HitInfo>(EUnitEvent.HitReceived, OnHitReceived);
		EventsAggregator.Units.AddListener<BaseUnit>(EUnitEvent.DeathCame, OnUnitDeath);
		EventsAggregator.Fight.AddListener(EFightEvent.Pause, OnFightPause);
		EventsAggregator.Fight.AddListener(EFightEvent.Resume, OnFightResume);
		EventsAggregator.Fight.AddListener(EFightEvent.MapComplete, OnMapComplete);
		EventsAggregator.Fight.AddListener(EFightEvent.MapFail, OnMapFail);
	}
	
	private bool _isStarted = false;
	private Action _onStart = null;
	public IEnumerator Start() {
        _model.SimulateAttack();
		
		//if (_model.Animator.GetCurrentAnimatorClipInfo(0).Length == 0) {
			yield return null;
        //}

        _model.SetupWeapon();
		
		_isStarted = true;
		if (_onStart != null) {
			_onStart();
			_onStart = null;
		}
        EventsAggregator.Units.Broadcast<BaseUnitBehaviour>(EUnitEvent.ReadyToFight, this);
    }

    public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<BaseUnit, HitInfo>(EUnitEvent.HitReceived, OnHitReceived);
		EventsAggregator.Units.RemoveListener<BaseUnit>(EUnitEvent.DeathCame, OnUnitDeath);
		EventsAggregator.Fight.RemoveListener(EFightEvent.Pause, OnFightPause);
		EventsAggregator.Fight.RemoveListener(EFightEvent.Resume, OnFightResume);
		EventsAggregator.Fight.RemoveListener(EFightEvent.MapComplete, OnMapComplete);
		EventsAggregator.Fight.RemoveListener(EFightEvent.MapFail, OnMapFail);
		
		if (_isAlly && UnitsConfig.Instance != null && UnitsConfig.Instance.IsHero(_unitData.Data.Key)) {
			EventsAggregator.Units.RemoveListener<ESkillKey>(EUnitEvent.SkillUsage, UseSkill);
		}
		
		_unitData = null;
        _lastTargetUnit = _targetUnit = null;
        _unitAttack = null;
        //_unitPathfinder = null;
        _model = null;
		_ui = null;
	}
	
	public void Setup(BaseUnit unitData, Dictionary<ESkillKey, BaseUnitSkill> skills, string tag, GameObject uiResource, int unitNumber) {
        _unitData = unitData;
        gameObject.tag = tag;
        _isAlly = gameObject.CompareTag(GameConstants.Tags.UNIT_ALLY);

        _attackTime = FightManager.SceneInstance.AttackInterval;// 1f / unitData.AttackSpeed;
        _cachedWaitForSeconds = new WaitForSeconds(_attackTime - _model.ShootPositionTimeOffset);

        _skills = skills != null ? skills : new Dictionary<ESkillKey, BaseUnitSkill>();

        if (_isAlly && UnitsConfig.Instance.IsHero(_unitData.Data.Key))
        {
            EventsAggregator.Units.AddListener<ESkillKey>(EUnitEvent.SkillUsage, UseSkill);
        }

        if (_ui == null)
        {
            _ui = (GameObject.Instantiate(uiResource) as GameObject).GetComponent<UnitUI>();
            _ui.transform.SetParent(transform, false);
            _ui.transform.localPosition = _healthBarPosition;
            _ui.transform.localRotation = Quaternion.Euler(GameConstants.CAMERA_ROTATION);
        }
        else {
            _ui.Reset();
        }

        if (unitData.DamageTaken > 0)
        {
            _ui.UpdateHealthBar(Mathf.Max(unitData.HealthPoints - unitData.DamageTaken, 0) / (unitData.HealthPoints * 1f));
        }

        if (_isStarted)
        {
            EventsAggregator.Units.Broadcast<BaseUnitBehaviour>(EUnitEvent.ReadyToFight, this);
        }
    }

    public void Stun(float duration) {
		_model.PlayStunAnimation();
		
		for (int i = 0; i < UnitData.ActiveSkills.ActiveSkills.Count; i++) {
			UnitData.ActiveSkills.ActiveSkills[i].OnCasterStunned();
		}		
		StopTargetAttack(false);
		_targetUnit = null;
        _unitAttack.Reset(true);
		//_unitPathfinder.Reset(true);
		
		if (IsInvoking("Run")) {
            CancelInvoke("Run");
		}
		Invoke("Run", duration);
	}
	
	public void Run() {
        if (!_isStarted)         
        {
			_onStart += Run;
			return;
		}
        FindTarget();
	}

    public void FindTarget()
    {
        _unitAttack.FindTarget(this, _lastTargetUnit, IsAlly ? FightManager.SceneInstance.EnemyUnits : FightManager.SceneInstance.AllyUnits,
            OnTargetFound, OnTargetAttack);
    }

    public void SetPosition(Vector3 position)
    {
        UnitAttack.SetPosition(this, position);
    }

    public void SetPlace(EUnitRange range, EUnitPosition position)
    {
        Place = UnitData.TemplatePlace.Range != EUnitRange.None && UnitData.TemplatePlace.Position != EUnitPosition.None
            ? UnitData.TemplatePlace : new UnitPlace() { Range = range, Position = position };
    }

	public void GoToMapEnd() {
        _unitAttack.WalkIntoSunset();
		//_unitPathfinder.WalkIntoSunset();
	}
	
	public void StopAllActions() {
        _unitAttack.Reset(true);
		//_unitPathfinder.Reset(true);
		_model.PlayIdleAnimation();
	}
	
	public void UseSkill(ESkillKey skillKey) {
		if (FightManager.SceneInstance.Status == EFightStatus.InProgress && _skills.ContainsKey(skillKey) && _skills[skillKey] != null) {
			_skills[skillKey].Use(this);
		}
	}

    public void StartTargetAttack()
    {
        if (CastingSkill)
            return;
        if (_lastAttackTime == 0f || Time.time - _lastAttackTime > _attackTime)
        {
            _corTargetAttack = StartCoroutine(AttackTarget());
        }
    }
	
	public void StopTargetAttack(bool resetAttackTimer) {
		if (resetAttackTimer) {
			_lastAttackTime = 0f;
		}
		if (IsInvoking("StartTargetAttack")) {
			CancelInvoke("StartTargetAttack");
		}
		if (_corTargetAttack != null) {
			StopCoroutine(_corTargetAttack);
			_corTargetAttack = null;
		}
		_model.StopAttackAnimation();
	}
	
	#region unit controller
	private void OnTargetFound(BaseUnitBehaviour target) {		
        _targetUnit = target;
	}

	private void OnTargetAttack() {
         StartTargetAttack();
	}
	
	private void OnTargetDeath() {
		for (int i = 0; i < UnitData.ActiveSkills.ActiveSkills.Count; i++) {
			UnitData.ActiveSkills.ActiveSkills[i].OnCasterTargetDeath();
		}		
		StopTargetAttack(false);
		_targetUnit = null;
        FindTarget();
	}
	
	private void OnSelfDeath() {
		for (int i = 0; i < UnitData.ActiveSkills.ActiveSkills.Count; i++) {
			UnitData.ActiveSkills.ActiveSkills[i].OnCasterDeath();
		}		
		if (IsInvoking("Run")) {
			CancelInvoke("Run");
		}
		StopTargetAttack(false);//(true);
		_lastTargetUnit = _targetUnit = null;
        _unitAttack.Reset(true);
		
		EventsAggregator.Fight.Broadcast<BaseUnit>(gameObject.tag == GameConstants.Tags.UNIT_ALLY ? EFightEvent.AllyDeath : EFightEvent.EnemyDeath, _unitData);
		
		_model.PlayDeathAnimation(OnDeathAnimationEnd);
	}
	
	private void OnMapEnd() {
        for (int i = 0; i < UnitData.ActiveSkills.ActiveSkills.Count; i++)
        {
			UnitData.ActiveSkills.ActiveSkills[i].Break();
		}
        if (IsInvoking("Run"))
        {
			CancelInvoke("Run");
		}		
		StopTargetAttack(true);
		_lastTargetUnit = _targetUnit = null;
        _unitAttack.Reset(true);
		
		if (_unitData != null && !_unitData.IsDead) {
			_model.PlayWinAnimation();
            _unitAttack.LookIntoSunset();
		}
	}

    bool _performPlay = false;
    bool _performAttack = false;
    bool _performWait = false;
    private IEnumerator AttackTarget()
    {
        _lastAttackTime = Time.time;
        _lastTargetUnit = _targetUnit;

        if (_model.WFSAttackDelay != null)
            yield return _model.WFSAttackDelay;
        if (!_performPlay)
        {
            _model.PlayAttackAnimation(DistanceToTarget);
            _performPlay = true;
        }
        if (!_performAttack)
        {
            EventsAggregator.Fight.Broadcast<BaseUnitBehaviour, BaseUnitBehaviour>(EFightEvent.PerformAttack, this, _targetUnit);
            _unitAttack.ToNextAttackUnit(this);
            _performAttack = true;            
        }
        if (_unitAttack.State == EUnitAttackState.AttackTarget)
        {
            if (!_performWait)
            {
                _performWait = true;
                yield return _cachedWaitForSeconds;
            }
            _model.StopCurrentAnimation();

            StopTargetAttack(false);
            _targetUnit = null;
            _unitAttack.Reset(true);

            _corTargetAttack = null;
        }
        _performPlay = _performAttack = _performWait = false;
    }
	
	private void OnHitReceived(BaseUnit unit, HitInfo hitInfo) {
		if (unit == _unitData) {
			_model.PlayHitAnimation(unit.HealthPoints, hitInfo);
			_ui.ApplyDamage(unit.HealthPoints, hitInfo);
		}
	}

	private IEnumerator Vanish() {
		yield return new WaitForSeconds(1f);
		
		GameObject.Destroy(gameObject);
	}
	#endregion
	
	#region listeners
	private void OnUnitDeath(BaseUnit unitData) {
		if (unitData == _unitData) {
			OnSelfDeath();
		} else if (_targetUnit != null && unitData == _targetUnit._unitData) {
			OnTargetDeath();
		}
	}
	
	private void OnFightPause() {
		
	}
	
	private void OnFightResume() {
		
	}
	
	private void OnMapComplete() {
		OnMapEnd();
	}
	
	private void OnMapFail() {
		OnMapEnd();
	}
	
	private void OnDeathAnimationEnd() {
		StartCoroutine(Vanish());
	}
	#endregion

    #region Члены IComparable

    public int CompareTo(object obj)
    {
        BaseUnitBehaviour unitBehaviour = obj as BaseUnitBehaviour;
        if (unitBehaviour != null)
        {
            if (IsAlly == unitBehaviour.IsAlly)
            {
                if (UnitsConfig.Instance.IsHero(UnitData.Data.Key))
                    return 1;
                else if (UnitsConfig.Instance.IsHero(unitBehaviour.UnitData.Data.Key))
                    return -1;
                else
                {
                    if (UnitData.Data.BaseRange == unitBehaviour.UnitData.Data.BaseRange)
                    {
                        return UnitData.HealthPoints.CompareTo(unitBehaviour.UnitData.HealthPoints);
                    }
                    else if (UnitData.Data.BaseRange == EUnitRange.Melee)
                        return 1;
                    else
                        return -1;
                }
            }
            else if (IsAlly)
                return 1;
            else
                return -1;
        }
        else
        {
            throw new ArgumentException("Cравниваемый объект не является BaseUnitBehaviour!");
        }
    }

    #endregion
}

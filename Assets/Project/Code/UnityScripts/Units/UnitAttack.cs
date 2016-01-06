using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAttack : MonoBehaviour
{
    [SerializeField]
    private UnitModelView _model;
    Transform _modelTransform = null;
    Transform _transform = null;

    float _speed = 1f;
    float _rotationSpeed = 5f;

    static BaseUnitBehaviour _lastAttackUnit = null;
    BaseUnitBehaviour _target = null;
    Transform _targetTransform = null;

    Vector3 _destinationPosition = Vector3.zero;

    Dictionary<EUnitAttackState, Action> _attackActions = new Dictionary<EUnitAttackState, Action>();

    Action<BaseUnitBehaviour> _onTargetFound = null;
    Action _onTargetAttacked = null;

    EUnitAttackState _state = EUnitAttackState.None;
    public EUnitAttackState State
    {
        get { return _state; }
        private set
        {
            _state = value;
            Update();
        }
    }

    public void Awake()
    {
		if (_model == null) 
			_model = gameObject.GetComponentInChildren<UnitModelView>();
        _model.MovementSpeed = _speed;
        _modelTransform = _model.transform;
        _transform = transform;

        _attackActions.Add(EUnitAttackState.NoAttack, MoveToPosition); 
        _attackActions.Add(EUnitAttackState.WatchTarget, WatchTarget);
        _attackActions.Add(EUnitAttackState.AttackTarget, AttackTarget);

        _attackActions.Add(EUnitAttackState.LookIntoSunset, LookForward);
        _attackActions.Add(EUnitAttackState.WalkIntoSunset, MoveForward);
    }

    public void Update()
    {
        if (_state != EUnitAttackState.None && _state != EUnitAttackState.NoTarget)
        {
            if (_attackActions.ContainsKey(_state))
                _attackActions[_state]();
        }
    }

    #region Movement

    public void LookIntoSunset()
    {
        State = EUnitAttackState.LookIntoSunset;
        _lastAttackUnit = null;
    }

    public void WalkIntoSunset()
    {
        State = EUnitAttackState.WalkIntoSunset;
        _model.PlayRunAnimation();
    }

    public void FindTarget(BaseUnitBehaviour unit, BaseUnitBehaviour currentTarget, ArrayRO<BaseUnitBehaviour> possibleUnits,
        Action<BaseUnitBehaviour> onTargetFound, Action onTargetAttacked)
    {
        Reset(false);
        //UnitSet.Instance.SetUnitPositions();

        _onTargetFound = onTargetFound;
        _onTargetAttacked = onTargetAttacked;
        _target = UnitSet.Instance.GetTarget(unit, currentTarget, possibleUnits);
        if (_target != null)
        {
            _targetTransform = _target.transform;
        }
        if (_targetTransform == null)
        {
            Reset(false);
            State = EUnitAttackState.NoTarget;
            return;
        }
        onTargetFound(_targetTransform.gameObject.GetComponent<BaseUnitBehaviour>());
        if (State == EUnitAttackState.None || State == EUnitAttackState.NoAttack || State == EUnitAttackState.WatchTarget)
        {
            StartTargetAttack(_target);
        }
    }

    void StartTargetAttack(BaseUnitBehaviour target)
    {
        _model.StopCurrentAnimation();
        Action<BaseUnitBehaviour> onTargetFound = _onTargetFound;
        _onTargetFound = null;
        StopAllCoroutines();

        State = EUnitAttackState.WatchTarget;
        onTargetFound(target);
    }

    private void WatchTarget()
    {
        _modelTransform.localRotation = Quaternion.Lerp(_modelTransform.localRotation,
            Quaternion.LookRotation(_targetTransform.position - _transform.position), _rotationSpeed * Time.deltaTime);

        BaseUnitBehaviour unit = gameObject.GetComponent<BaseUnitBehaviour>();
        BaseUnitBehaviour attackUnit = UnitSet.Instance.GetNextAttackUnit(_lastAttackUnit);
        if (attackUnit != null && attackUnit.Equals(unit))
        {
            if (_lastAttackUnit == null || _lastAttackUnit != null 
                && _lastAttackUnit.UnitAttack.State != EUnitAttackState.AttackTarget)
            {
                State = EUnitAttackState.AttackTarget;
            }
        }
    }

    private void AttackTarget()
    {
        _modelTransform.localRotation = Quaternion.Lerp(_modelTransform.localRotation,
            Quaternion.LookRotation(_targetTransform.position - _transform.position), _rotationSpeed * Time.deltaTime);

        if (_onTargetAttacked != null)
        {
            _onTargetAttacked();
        }
    }

    public void ToNextAttackUnit(BaseUnitBehaviour currentUnit)
    {
        if (currentUnit.UnitAttack.State == EUnitAttackState.LookIntoSunset)
            return;
        _lastAttackUnit = currentUnit;
        if (currentUnit != null)
        {
            BaseUnitBehaviour nextAttackUnit = UnitSet.Instance.GetNextAttackUnit(_lastAttackUnit);
            if (nextAttackUnit != null && nextAttackUnit.UnitAttack.State != EUnitAttackState.LookIntoSunset)
            {
                nextAttackUnit.FindTarget();
            }
        }
    }

    private void LookForward()
    {
        _modelTransform.localRotation = Quaternion.Lerp(_modelTransform.localRotation, 
            Quaternion.LookRotation(new Vector3(1f, 0f, 0f)), _rotationSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        Vector3 destination = _modelTransform.position + new Vector3(1f, 0f, 0f);
        _modelTransform.localRotation = Quaternion.Lerp(_modelTransform.localRotation,
            Quaternion.LookRotation(new Vector3(1f, 0f, 0f)), _rotationSpeed * Time.deltaTime);
        //_modelTransform.LookAt(destination);
        _transform.position = Vector3.MoveTowards(_transform.position, destination, Time.deltaTime * _speed);
    }

    public void SetPosition(BaseUnitBehaviour unit, Vector3 position)
    {
        if (_destinationPosition == Vector3.zero)
            _destinationPosition = unit.transform.position = position;
        else
        {
            _destinationPosition = position;
            if (_destinationPosition != _modelTransform.position)
            {
                if (State != EUnitAttackState.AttackTarget)
                    State = EUnitAttackState.NoAttack;
                //_model.PlayRunAnimation();
            }
        }
    }

    public void MoveToPosition()
    {
        float minDistance = 0.1f;
        _transform.position = Vector3.MoveTowards(_modelTransform.position, _destinationPosition, Time.deltaTime * _speed * 3);
        if (Vector3.Distance(_transform.position, _destinationPosition) < minDistance)
        {
            //_model.StopCurrentAnimation();
            _destinationPosition = _transform.position;
            State = EUnitAttackState.None;
        }
    }

    #endregion

    public void Reset(bool full)
    {
        _target = null;
        _targetTransform = null;

        _onTargetFound = null;
        _onTargetAttacked = null;
		StopAllCoroutines();
        if (full)
        {
            State = EUnitAttackState.None;
            //Debug.Log("None " + gameObject.GetComponent<BaseUnitBehaviour>().name);
        }
    }
    //string GetUnitName(BaseUnitBehaviour unit)
    //{
    //    return unit != null ? unit.name : string.Empty;
    //}

    //string GetPlaceName(UnitPlace place)
    //{
    //    return place.Range + " " + place.Position;
    //}
}


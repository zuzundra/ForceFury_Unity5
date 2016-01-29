using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSet
{
    #region instance

    private static UnitSet _instance = null;
    public static UnitSet Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UnitSet();
            return _instance;
        }
    }

    #endregion

    public const int FirstZoneIndex = 0;
    public const int SecondZoneIndex = 1;
    public const int ThirdZoneIndex = 2;

    #region Первоначальная расстановка юнитов по полю

    public void SetUnitPositions()
    {
        _rangeAllyUnits = null;
        SetUnitPositions(RangeAllyUnits, true);

        _rangeEnemyUnits = null;
        SetUnitPositions(RangeEnemyUnits, false);
    }

    List<BaseUnitBehaviour>[] _rangeAllyUnits = null;
    public List<BaseUnitBehaviour>[] RangeAllyUnits
    {
        get
        {
            if (_rangeAllyUnits == null)
            {
                _rangeAllyUnits = GetRangeUnits(FightManager.SceneInstance.AllyUnits);
            }
            return _rangeAllyUnits;
        }
    }

    List<BaseUnitBehaviour>[] _rangeEnemyUnits = null;
    public List<BaseUnitBehaviour>[] RangeEnemyUnits
    {
        get
        {
            if (_rangeEnemyUnits == null)
            {
                _rangeEnemyUnits = GetRangeUnits(FightManager.SceneInstance.EnemyUnits);
            }
            return _rangeEnemyUnits;
        }
    }

    List<BaseUnitBehaviour>[] GetRangeUnits(ArrayRO<BaseUnitBehaviour> units)
    {
        List<BaseUnitBehaviour>[] rangeUnits = new List<BaseUnitBehaviour>[3] { new List<BaseUnitBehaviour>(),
            new List<BaseUnitBehaviour>(), new List<BaseUnitBehaviour>() };
        List<BaseUnitBehaviour> heroes = rangeUnits[FirstZoneIndex];
        List<BaseUnitBehaviour> remoteUnits = rangeUnits[SecondZoneIndex];
        List<BaseUnitBehaviour> nearUnits = rangeUnits[ThirdZoneIndex];
        for (int i = 0; i < units.Length; i++)
        {
            BaseUnitBehaviour unit = units[i];
            if (unit != null && unit.UnitData != null)
            {
                EUnitPosition position = unit.Place.Position;
                if (UnitsConfig.Instance.IsHero(unit.UnitData.Data.Key))
                {
                    if (heroes.Count < 3)
                        heroes.Add(unit);
                    else if (remoteUnits.Count < 3)
                    {
                        unit.Place = new UnitPlace() { Range = EUnitRange.Ranged, Position = position };
                        remoteUnits.Add(unit);
                    }
                    else if (nearUnits.Count < 3)
                    {
                        unit.Place = new UnitPlace() { Range = EUnitRange.Melee, Position = position };
                        nearUnits.Add(unit);
                    }
                }
                else
                {
                    EUnitRange range = unit.UnitData.TemplatePlace.Range != EUnitRange.None
                        ? unit.UnitData.TemplatePlace.Range : unit.UnitData.Data.BaseRange;
                    if (range == EUnitRange.Ranged)
                    {
                        if (remoteUnits.Count < 3)
                        {
                            unit.Place = new UnitPlace() { Range = EUnitRange.Ranged, Position = position };
                            remoteUnits.Add(unit);
                        }
                        else if (nearUnits.Count < 3)
                        {
                            unit.Place = new UnitPlace() { Range = EUnitRange.Melee, Position = position };
                            nearUnits.Add(unit);
                        }
                    }
                    else if (range == EUnitRange.Melee)
                    {
                        if (nearUnits.Count < 3)
                        {
                            unit.Place = new UnitPlace() { Range = EUnitRange.Melee, Position = position };
                            nearUnits.Add(unit);
                        }
                        else if (remoteUnits.Count < 3)
                        {
                            unit.Place = new UnitPlace() { Range = EUnitRange.Ranged, Position = position };
                            remoteUnits.Add(unit);
                        }
                    }
                }
            }
        }
        return rangeUnits;
    }

    void SetUnitPositions(List<BaseUnitBehaviour>[] rangeUnits, bool isAlly)
    {
        Canvas canvas = FightManager.SceneInstance.UI.CanvasBG;
        float width = GameConstants.DEFAULT_RESOLUTION_WIDTH * canvas.transform.localScale.x;
        float height = GameConstants.DEFAULT_RESOLUTION_HEIGHT * canvas.transform.localScale.y;
        float xMin = -width / 2;
        float xMax = width / 2;
        float y = canvas.transform.position.y + 1;
        float zMin = -height / 2;
        float zMax = height / 2;
        float delta = (xMax - xMin) / 12;

        //Debug.Log("W" + GameConstants.DEFAULT_RESOLUTION_WIDTH);
        //Debug.Log("H" + GameConstants.DEFAULT_RESOLUTION_HEIGHT);
        //Debug.Log("w" + width);
        //Debug.Log("h" + height);
        //Debug.Log("xmin" + xMin);
        //Debug.Log("xmax" + xMax);
        //Debug.Log("ymin" + yMin);
        //Debug.Log("ymax" + yMax);
        //Debug.Log("d" + delta);

        List<BaseUnitBehaviour> heroes = rangeUnits[FirstZoneIndex];
        SetZonePositions(heroes, isAlly ? xMin + delta : xMax - delta, y, zMin, zMax);

        List<BaseUnitBehaviour> remoteUnits = rangeUnits[SecondZoneIndex];
        SetZonePositions(remoteUnits, isAlly ? xMin + delta * 3 : xMax - delta * 3, y, zMin, zMax);

        List<BaseUnitBehaviour> nearUnits = rangeUnits[ThirdZoneIndex];
        SetZonePositions(nearUnits, isAlly ? xMin + delta * 5 : xMax - delta * 5, y, zMin, zMax);
    }

    void SetZonePositions(List<BaseUnitBehaviour> units, float x, float y, float minZ, float maxZ)
    {
        if (units.Count == 0)
            return;
        units.Sort();
        if (units.Count > 3)
        {
            units = units.GetRange(0, 3);
        }
        BaseUnitBehaviour firstUnit = units[0];
        firstUnit.SetPlace(firstUnit.Place.Range, EUnitPosition.Middle);        
        if (units.Count > 1)
        {
            BaseUnitBehaviour secondUnit = units[1];            
            secondUnit.SetPlace(secondUnit.Place.Range, EUnitPosition.Top);
            if (units.Count > 2)
            {
                BaseUnitBehaviour thirdUnit = units[2];                
                thirdUnit.SetPlace(thirdUnit.Place.Range, EUnitPosition.Bottom);
            }
        }
        foreach (BaseUnitBehaviour unit in units)
        {
            switch (unit.Place.Position)
            {
                case EUnitPosition.Middle: unit.SetPosition(new Vector3(x, y, (maxZ + minZ) / 2)); break;
                case EUnitPosition.Top: unit.SetPosition(new Vector3(x, y, maxZ - (maxZ - minZ) / 6)); break;
                case EUnitPosition.Bottom: unit.SetPosition(new Vector3(x, y, minZ + (maxZ - minZ) / 6)); break;                 
            }
        }
    }

    #endregion

    #region Поочерёдная атака

    public BaseUnitBehaviour GetNextAttackUnit(BaseUnitBehaviour attackUnit)
    {
        bool currentIsAllyAttack =  attackUnit != null && attackUnit.IsAlly;
        UnitPlace currentAttackPlace = attackUnit != null ? attackUnit.Place
            : new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };

        bool nextIsAllyAttack = !currentIsAllyAttack;
        UnitPlace nextAttackPlace = nextIsAllyAttack && attackUnit != null
            ? GetNextAttackPlace(currentAttackPlace) : currentAttackPlace;

        while (!(nextIsAllyAttack == currentIsAllyAttack && nextAttackPlace.Equals(currentAttackPlace)))
        {
            if (nextAttackPlace.Range != EUnitRange.None || nextAttackPlace.Position != EUnitPosition.None)
            {
                BaseUnitBehaviour nextUnit = GetPlaceBaseUnitBehaviour(nextIsAllyAttack
                    ? FightManager.SceneInstance.AllyUnits : FightManager.SceneInstance.EnemyUnits, nextAttackPlace);
                if (nextUnit != null && !nextUnit.UnitData.IsDead)
                    return nextUnit;
            }
            nextIsAllyAttack = attackUnit != null ? !nextIsAllyAttack : nextIsAllyAttack;
            nextAttackPlace = nextIsAllyAttack ? GetNextAttackPlace(nextAttackPlace) : nextAttackPlace;
            if (attackUnit == null && nextIsAllyAttack.Equals(currentAttackPlace))
                nextIsAllyAttack = false;
        }
        return null;
    }

    UnitPlace GetNextAttackPlace(UnitPlace attackPlace)
    {
        switch (attackPlace.Range)
        {
            case EUnitRange.Melee:
                return new UnitPlace() { Range = EUnitRange.Ranged, Position = attackPlace.Position };

            case EUnitRange.Ranged:
                if (attackPlace.Position == EUnitPosition.Middle)
                    return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Top };
                else if (attackPlace.Position == EUnitPosition.Top)
                    return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Bottom };
                else if (attackPlace.Position == EUnitPosition.Bottom)
                    return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
                break;

            case EUnitRange.None:
                if (attackPlace.Position == EUnitPosition.Middle)
                    return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };
                break;
        }
        return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.None };
    }

    BaseUnitBehaviour GetPlaceBaseUnitBehaviour(ArrayRO<BaseUnitBehaviour> units, UnitPlace place)
    {
        for (int i = 0; i < units.Length; i++)
        {
            BaseUnitBehaviour possibleUnit = units[i];
            if (possibleUnit.Place.Range == place.Range && possibleUnit.Place.Position == place.Position)
                return possibleUnit;
        }
        return null;
    }

    #endregion

    #region Поиск цели

    public BaseUnitBehaviour GetTarget(BaseUnitBehaviour unit, BaseUnitBehaviour currentTarget, 
        ArrayRO<BaseUnitBehaviour> possibleUnits)
    {
        UnitPlace currentTargetPlace = currentTarget != null ? currentTarget.Place
            : new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
        UnitPlace nextTargetPlace = GetNextTargetPlace(unit.Place, currentTargetPlace);
        while (!nextTargetPlace.Equals(currentTargetPlace))
        {
            if (nextTargetPlace.Range != EUnitRange.None || nextTargetPlace.Position != EUnitPosition.None)
            {
                BaseUnitBehaviour nextUnit = GetPlaceBaseUnitBehaviour(possibleUnits, nextTargetPlace);
                if (nextUnit != null && !nextUnit.UnitData.IsDead)
                    return nextUnit;
            }
            nextTargetPlace = GetNextTargetPlace(unit.Place, nextTargetPlace);
        }
        if (currentTarget != null)
            return currentTarget;
        else
        {
            BaseUnitBehaviour nextUnit = GetPlaceBaseUnitBehaviour(possibleUnits, currentTargetPlace);
            return nextUnit != null && !nextUnit.UnitData.IsDead ? nextUnit : null;
        }
    }

    UnitPlace GetNextTargetPlace(UnitPlace attackPlace, UnitPlace currentPlace)
    {
        switch (attackPlace.Position)
        {
            case EUnitPosition.Middle:
                if (attackPlace.Range == EUnitRange.Ranged || attackPlace.Range == EUnitRange.Melee)
                {
                    if (currentPlace.Range == EUnitRange.Melee)
                        return new UnitPlace() { Range = EUnitRange.Ranged, Position = currentPlace.Position };
                    else if (currentPlace.Range == EUnitRange.Ranged)
                    {
                        if (currentPlace.Position == EUnitPosition.Middle)
                            return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Top };
                        else if (currentPlace.Position == EUnitPosition.Top)
                            return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Bottom };
                        else if (currentPlace.Position == EUnitPosition.Bottom)
                            return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
                    }
                    else
                        return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };
                }
                else
                {
                    if (currentPlace.Position == EUnitPosition.Middle)
                    {
                        if (currentPlace.Range == EUnitRange.Ranged || currentPlace.Range == EUnitRange.Melee)
                            return new UnitPlace() { Range = currentPlace.Range, Position = EUnitPosition.Top };
                        else
                            return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };
                    }
                    else if (currentPlace.Position == EUnitPosition.Top)
                        return new UnitPlace() { Range = currentPlace.Range, Position = EUnitPosition.Bottom };
                    else if (currentPlace.Position == EUnitPosition.Bottom)
                    {
                        if (currentPlace.Range == EUnitRange.Melee)
                            return new UnitPlace() { Range = EUnitRange.Ranged, Position = EUnitPosition.Middle };
                        else if (currentPlace.Range == EUnitRange.Ranged)
                            return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
                    }
                }
                break;

            case EUnitPosition.Top:
                if (currentPlace.Range == EUnitRange.Melee)
                    return new UnitPlace() { Range = EUnitRange.Ranged, Position = currentPlace.Position };
                else if (currentPlace.Range == EUnitRange.Ranged)
                {
                    if (currentPlace.Position == EUnitPosition.Top)
                        return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };
                    else if (currentPlace.Position == EUnitPosition.Middle)
                        return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Bottom };
                    else if (currentPlace.Position == EUnitPosition.Bottom)
                        return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
                }
                else
                    return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Top };
                break;

            case EUnitPosition.Bottom:
                if (currentPlace.Range == EUnitRange.Melee)
                    return new UnitPlace() { Range = EUnitRange.Ranged, Position = currentPlace.Position };
                else if (currentPlace.Range == EUnitRange.Ranged)
                {
                    if (currentPlace.Position == EUnitPosition.Bottom)
                        return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Middle };
                    else if (currentPlace.Position == EUnitPosition.Middle)
                        return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Top };
                    else if (currentPlace.Position == EUnitPosition.Top)
                        return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.Middle };
                }
                else
                    return new UnitPlace() { Range = EUnitRange.Melee, Position = EUnitPosition.Bottom };
                break;
        }
        return new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.None };
    }

    #endregion

    string GetUnitName(BaseUnitBehaviour unit)
    {
        return unit != null ? unit.name : string.Empty;
    }

    string GetPlaceName(UnitPlace place)
    {
        return place.Range + " " + place.Position;
    }
}
using UnityEngine;

[System.Serializable]
public class UnitSlot
{
    [SerializeField]
    UnitPlace _place = new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.None };
    public UnitPlace Place
    {
        get { return _place; }
    }

    [SerializeField]
    EUnitKey _unit = EUnitKey.Idle;
    public EUnitKey Unit
    {
        get { return _unit; }
    }

    public UnitSlot() { }
    public UnitSlot(UnitPlace place, EUnitKey unit)
    {
        _place = place;
        _unit = unit;
    }
}
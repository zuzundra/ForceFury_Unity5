using UnityEngine;

[System.Serializable]
public class CBBarracksLevel {
	[SerializeField]
	private EUnitKey[] _availableUnits = null;
	private ArrayRO<EUnitKey> _availableUnitsRO = null;
	public ArrayRO<EUnitKey> AvailableUnits {
		get {
			if (_availableUnitsRO == null) {
				_availableUnitsRO = new ArrayRO<EUnitKey>(_availableUnits);
			}
			return _availableUnitsRO;
		}
	}

	[SerializeField]
	private int _unitWeaponUpgradeLevel = 10;
	public int UnitWeaponUpgradeLevel {
		get { return _unitWeaponUpgradeLevel; }
	}

	[SerializeField]
	private int _unitArmorUpgradeLevel = 10;
	public int UnitArmorUpgradeLevel {
		get { return _unitArmorUpgradeLevel; }
	}
}

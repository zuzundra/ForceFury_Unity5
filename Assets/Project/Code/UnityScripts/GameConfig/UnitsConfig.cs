using UnityEngine;

public class UnitsConfig : MonoBehaviourResourceSingleton<UnitsConfig> {
#pragma warning disable 0414
	private static string _path = "Config/UnitsConfig";
#pragma warning restore 0414

	[SerializeField]
	private int _maxUnitsHeroCanHire = 4;
	public int MaxUnitsHeroCanHire {
		get { return _maxUnitsHeroCanHire; }
	}

    //[SerializeField]
    //private float _damageReducePerOneArmor = 0f;
    //public float DamageReducePerOneArmor {
    //    get { return _damageReducePerOneArmor; }
    //}

	[SerializeField]
	private EUnitEqupmentSlot[] _heroEquipmentSlots = new EUnitEqupmentSlot[0];
	private ArrayRO<EUnitEqupmentSlot> _heroEquipmentSlotsRO = null;
	[SerializeField]
	private EUnitEqupmentSlot[] _soldierEquipmentSlots = new EUnitEqupmentSlot[0];
	private ArrayRO<EUnitEqupmentSlot> _soldierEquipmentSlotsRO = null;

	[SerializeField]
	private BaseHeroData[] _heroesData = new BaseHeroData[0];
	[SerializeField]
	private BaseSoldierData[] _soldiersData = new BaseSoldierData[0];

	[SerializeField]
	private SoldierUpgrade[] _soldierUpgrades = null;
	private ArrayRO<SoldierUpgrade> _soldierUpgradesRO = null;
	public ArrayRO<SoldierUpgrade> SoldierUpgrades {
		get {
			if (_soldierUpgradesRO == null) {
				_soldierUpgradesRO = new ArrayRO<SoldierUpgrade>(_soldierUpgrades);
			}
			return _soldierUpgradesRO;
		}
	}

	public ArrayRO<EUnitEqupmentSlot> GetUnitEquipmentSlots(BaseUnit unit) {
		if (_heroEquipmentSlotsRO == null) {
			_heroEquipmentSlotsRO = new ArrayRO<EUnitEqupmentSlot>(_heroEquipmentSlots);
			_heroEquipmentSlots = null;
		}
		if (_soldierEquipmentSlotsRO == null) {
			_soldierEquipmentSlotsRO = new ArrayRO<EUnitEqupmentSlot>(_soldierEquipmentSlots);
			_soldierEquipmentSlots = null;
		}

		if (unit is BaseHero) {
			return _heroEquipmentSlotsRO;
		}
		return _soldierEquipmentSlotsRO;
	}

	public BaseHeroData GetHeroData(EUnitKey unitKey) {
		for (int i = 0; i < _heroesData.Length; i++) {
			if (_heroesData[i].Key == unitKey) {
				return _heroesData[i];
			}
		}
		return null;
	}

	public BaseSoldierData GetSoldierData(EUnitKey unitKey) {
		for (int i = 0; i < _soldiersData.Length; i++) {
			if (_soldiersData[i].Key == unitKey) {
				return _soldiersData[i];
			}
		}
		return null;
	}

	public BaseUnitData GetUnitData(EUnitKey unitKey) {
		BaseUnitData bud = null;

		bud = GetHeroData(unitKey);
		if (bud != null) {
			return bud;
		}

		bud = GetSoldierData(unitKey);
		return bud;
	}

	public bool IsHero(EUnitKey unitKey) {
		return (int)unitKey > 0 && (int)unitKey < 1000000;
	}

	#region upgrades
	public SoldierUpgrade GetSoldierUpgrades(EUnitKey unitKey) {
		for (int i = 0; i < _soldierUpgrades.Length; i++) {
			if (_soldierUpgrades[i].SoldierKey == unitKey) {
				return _soldierUpgrades[i];
			}
		}

		return null;
	}
	#endregion
}

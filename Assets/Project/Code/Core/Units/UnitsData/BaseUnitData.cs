using UnityEngine;

[System.Serializable]
public abstract class BaseUnitData {
	[SerializeField]
	private EUnitKey _key = EUnitKey.Idle;	//unit key
	public EUnitKey Key {
		get { return _key; }
	}

    [SerializeField]
	private int _baseHealthPoints = 0;	//base health amount (without upgrades)
	public int BaseHealthPoints {
		get { return _baseHealthPoints; }
	}

	[SerializeField]
	protected int _baseDamage = 0;	//base damage amount (without upgrades)
	public int BaseDamage {
		get { return _baseDamage; }
	}

    [SerializeField]
    protected EUnitRange _baseRange = EUnitRange.None;	//base attack range
    public EUnitRange BaseRange
    {
        get { return _baseRange; }
    }

    //[SerializeField]
    //protected float _baseAR = 1;	//base attack range (without upgrades)
    //public float BaseAR {
    //    get { return _baseAR; }
    //}

    //[SerializeField]
    //protected float _baseAttackSpeed = 1;	//base attack speed (without upgrades)
    //public float BaseAttackSpeed {
    //    get { return _baseAttackSpeed; }
    //}

    //[SerializeField]
    //protected int _baseArmor = 0;	//base armor amount (without upgrades)
    //public int BaseArmor {
    //    get { return _baseArmor; }
    //}

    //[SerializeField]
    //protected float _agroCrystalsDeath = 0;	//amount of aggro crystals unit gives to enemies after death
    //public float AgroCrystalsDeath {
    //    get { return _agroCrystalsDeath; }
    //}

    //[SerializeField]
    //protected float _aggroCrystalsForDeathToAlly = 0;	//amount of aggro crystals unit gives to allies after death
    //public float AggroCrystalsForDeathToAlly {
    //    get { return _aggroCrystalsForDeathToAlly; }
    //}

    //[SerializeField]
    //protected int _baseCritChance = 0;	//base critical hit chance (without upgrades)
    //public int BaseCritChance {
    //    get { return _baseCritChance; }
    //}

    //[SerializeField]
    //protected float _baseCritDamageMultiplier = 2f;	//base critical damage multiplier (without upgrades)
    //public float BaseCritDamageMultiplier {
    //    get { return _baseCritDamageMultiplier; }
    //}

	[SerializeField]
	protected ItemSlot[] _baseEquipment = null;
	protected ArrayRO<ItemSlot> _baseEquipmentRO = null;
	public ArrayRO<ItemSlot> BaseEquipment {
		get {
			if (_baseEquipmentRO == null) {
				_baseEquipmentRO = new ArrayRO<ItemSlot>(_baseEquipment);
			}
			return _baseEquipmentRO;
		}
	}

	[SerializeField]
	protected string _prefabName = string.Empty;
	public string PrefabName {
		get { return _prefabName; }
	}

	[SerializeField]
	protected string _iconName = string.Empty;
	public string IconName {
		get { return _iconName; }
	}

	[SerializeField]
	protected string _hexIconName = string.Empty;
	public string HexIconName {
		get { return _hexIconName; }
	}

    [SerializeField]
    protected string _battlePrefab = string.Empty;
    public string BattlePrefab
    {
        get { return _battlePrefab; }
    }

    [SerializeField]
    protected string _aboutInfo = string.Empty;
    public string AboutInfo
    {
        get { return _aboutInfo; }
    }

	public EItemKey GetBaseItemInSlot(EUnitEqupmentSlot slotName) {
		for (int i = 0; i < _baseEquipment.Length; i++) {
			if (_baseEquipment[i].SlotName == slotName) {
				return _baseEquipment[i].ItemKey;
			}
		}
		return EItemKey.None;
	}
}

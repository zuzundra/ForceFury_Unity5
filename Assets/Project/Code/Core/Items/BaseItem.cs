using UnityEngine;

[System.Serializable]
public class BaseItem {
	[SerializeField]
	protected EItemKey _key = EItemKey.None;	//item key
	public EItemKey Key {
		get { return _key; }
	}

	[SerializeField]
	protected EItemType _type = EItemType.Idle;	//item type
	public EItemType Type {
		get { return _type; }
	}

	[SerializeField]
	protected EUnitEqupmentSlot[] _possibleSlots = null;	//slot where item should be equipped
	protected ArrayRO<EUnitEqupmentSlot> _possibleSlotsRO = null;
	public ArrayRO<EUnitEqupmentSlot> PossibleSlots {
		get {
			if (_possibleSlotsRO == null) {
				_possibleSlotsRO = new ArrayRO<EUnitEqupmentSlot>(_possibleSlots);
			}
			return _possibleSlotsRO;
		}
	}

	//modifiers
	[SerializeField]
	protected int _modHealth = 0;
	public int ModHealth {
		get { return _modHealth; }
	}

	[SerializeField]
	protected int _modDamage = 0;
	public int ModDamage {
		get { return _modDamage; }
	}

    //[SerializeField]
    //protected float _modDamageRange = 0;
    //public float ModDamageRange {
    //    get { return _modDamageRange; }
    //}

    //[SerializeField]
    //protected float _modDamageSpeed = 0;
    //public float ModDamageSpeed {
    //    get { return _modDamageSpeed; }
    //}

    //[SerializeField]
    //protected int _modArmor = 0;
    //public int ModArmor {
    //    get { return _modArmor; }
    //}   

    [SerializeField]
    protected int _modReward = 0;
    public int ModReward
    {
        get { return _modReward; }
    }

    //[SerializeField]
    //protected int _modCritChance = 0;
    //public int ModCritChance {
    //    get { return _modCritChance; }
    //}

    //[SerializeField]
    //protected float _modCritDamageMultiplier = 0;
    //public float ModCritDamageMultiplier {
    //    get { return _modCritDamageMultiplier; }
    //}

	[SerializeField]
	protected int _levelRequirement = 1;
	public float LevelRequirement {
		get { return _levelRequirement; }
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
	protected ItemPriceData _prices = new ItemPriceData();
	public ItemPriceData Prices {
		get { return _prices; }
	}

	public BaseItem() {	}

	public BaseItem(EItemKey key, EItemType type, EUnitEqupmentSlot[] possibleSlots) {
		_key = key;
		_type = type;
		_possibleSlots = possibleSlots;
	}
}

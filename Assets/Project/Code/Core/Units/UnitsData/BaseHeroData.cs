using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class  BaseHeroData : BaseUnitData {
	[SerializeField]
	protected int _baseLeadership = 0;	//base leadership (without upgrades and level-ups)
	public int BaseLeadership {
		get { return _baseLeadership; }
	}

	[SerializeField]
	protected float _baseAggroCrystalsPerAttack = 0;	//base aggro crystals hero receives per attack (without upgrades and level-ups)
	public float BaseAggroCrystalPerAttack {
		get { return _baseAggroCrystalsPerAttack; }
	}

	[SerializeField]
	protected float _baseAggroCrystalsMaximum = 0;	//base aggro crystals cap (without upgrades and level-ups)
	public float BaseAggroCrystalsMaximum {
		get { return _baseAggroCrystalsMaximum; }
	}

	[SerializeField]
	protected ItemSlotConfig[] _availableItemTypes = new ItemSlotConfig[0];
	protected ArrayRO<ItemSlotConfig> _availableItemTypesRO = null;
	public ArrayRO<ItemSlotConfig> AvailableItemTypes {
		get {
			if (_availableItemTypesRO == null) {
				_availableItemTypesRO = new ArrayRO<ItemSlotConfig>(_availableItemTypes);
				_availableItemTypes = null;
			}
			return _availableItemTypesRO;
		}
	}

    [SerializeField]
    protected UnitSlot[] _slotTemplate = null;
    protected ArrayRO<UnitSlot> _slotTemplateRO = null;
    public ArrayRO<UnitSlot> SlotTemplate
    {
        get
        {
            if (_slotTemplateRO == null)
            {
                _slotTemplateRO = new ArrayRO<UnitSlot>(_slotTemplate);
            }
            return _slotTemplateRO;
        }
        set
        {
            _slotTemplateRO = value;
        }
    }
}

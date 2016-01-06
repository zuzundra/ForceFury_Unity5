using UnityEngine;

[System.Serializable]
public class ItemSlotConfig  {
	[SerializeField]
	private EUnitEqupmentSlot _slotKey = EUnitEqupmentSlot.None;
	public EUnitEqupmentSlot SlotKey {
		get { return _slotKey; }
	}

	[SerializeField]
	private EItemType[] _availableItemTypes = new EItemType[0];
	private ArrayRO<EItemType> _availableItemTypesRO = null;
	public ArrayRO<EItemType> AvailableItemTypes {
		get {
			if (_availableItemTypesRO == null) {
				_availableItemTypesRO = new ArrayRO<EItemType>(_availableItemTypes);
				_availableItemTypes = null;
			}
			return _availableItemTypesRO;
		}
	}

	public ItemSlotConfig() {
		_availableItemTypesRO = new ArrayRO<EItemType>(_availableItemTypes);
		_availableItemTypes = null;
	}
}

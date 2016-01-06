using UnityEngine;

[System.Serializable]
public class ItemSlot {
	[SerializeField]
	private EUnitEqupmentSlot _slotName = EUnitEqupmentSlot.None;
	public EUnitEqupmentSlot SlotName {
		get {
			return _slotName;
		}
	}

	[SerializeField]
	private EItemKey _itemKey = EItemKey.None;
	public EItemKey ItemKey {
		get { return _itemKey; }
		set { _itemKey = value; }
	}

	public ItemSlot(EUnitEqupmentSlot slotName) {
		_slotName = slotName;
		_itemKey = EItemKey.None;
	}

	public ItemSlot(EUnitEqupmentSlot slotName, EItemKey itemKey) {
		_slotName = slotName;
		_itemKey = itemKey;
	}
}
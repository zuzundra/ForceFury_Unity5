public class PlayerItem {
	private BaseItem _itemData = null;
	public BaseItem ItemData {
		get { return _itemData; }
	}

	public EUnitKey ItemCarrier { get; set; }
	public EUnitEqupmentSlot ItemSlot { get; set; }

	public bool IsEquipped {
		get { return ItemCarrier != EUnitKey.Idle; }
	}

	public PlayerItem(BaseItem itemData) {
		_itemData = itemData;
		ItemCarrier = EUnitKey.Idle;
		ItemSlot = EUnitEqupmentSlot.Other;
	}
}

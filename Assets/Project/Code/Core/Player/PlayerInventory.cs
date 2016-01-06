using System.Collections.Generic;

public class PlayerInventory {
	//TODO: items equip state
	private List<PlayerItem> _items = new List<PlayerItem>();
	private ListRO<PlayerItem> _itemsRO = null;
	public ListRO<PlayerItem> Items {
		get { return _itemsRO; }
	}

	public PlayerInventory() {
		_itemsRO = new ListRO<PlayerItem>(_items);

		EventsAggregator.Units.AddListener<BaseUnit, EUnitEqupmentSlot, EItemKey, EItemKey>(EUnitEvent.EquipmentUpdate, OnItemEquip);
	}

	~PlayerInventory() {
		EventsAggregator.Units.RemoveListener<BaseUnit, EUnitEqupmentSlot, EItemKey, EItemKey>(EUnitEvent.EquipmentUpdate, OnItemEquip);
	}

	public void AddItem(BaseItem item) {
		_items.Add(new PlayerItem(item));
	}

	public bool RemoveItem(BaseItem item) {
		for (int i = 0; i < _items.Count; i++) {
			if (_items[i].ItemData == item) {
				//unequip item before removing
				if (_items[i].ItemCarrier != EUnitKey.Idle) {
					Global.Instance.Player.Heroes.GetHero(_items[i].ItemCarrier).Inventory.Unequip(_items[i].ItemSlot);
				}
				_items.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public PlayerItem GetItem(EItemKey itemKey) {
		for (int i = 0; i < _items.Count; i++) {
			if (_items[i].ItemData.Key == itemKey) {
				return _items[i];
			}
		}
		return null;
	}

	public void Equip(BaseHero hero, int slotId, EItemKey itemKey) {
		hero.Inventory.Equip(slotId, itemKey);
	}

	public void Unequip(BaseHero hero, int slotId) {
		hero.Inventory.Equip(slotId, EItemKey.None);
	}

	#region listeners
	private void OnItemEquip(BaseUnit unit, EUnitEqupmentSlot slot, EItemKey oldItemKey, EItemKey newItemKey) {
		if (UnitsConfig.Instance.IsHero(unit.Data.Key) && Global.Instance.Player.Heroes.HaveHero(unit.Data.Key)) {
			PlayerItem oldItem = GetItem(oldItemKey);
			if (oldItem != null) {
				oldItem.ItemCarrier = EUnitKey.Idle;
				oldItem.ItemSlot = EUnitEqupmentSlot.None;
			}

			PlayerItem newItem = GetItem(newItemKey);
			if (newItem != null) {
				newItem.ItemCarrier = unit.Data.Key;
				newItem.ItemSlot = slot;
			}
		}
	}
	#endregion
}

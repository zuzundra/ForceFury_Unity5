using System;
using System.Collections.Generic;

/// <summary>
/// Unit's inventory. Contains equipped items
/// </summary>

public class UnitInventory {
	private Action<EUnitEqupmentSlot, EItemKey, EItemKey> _onEquipmentUpdate = null;	//@params: slot, old item, new item

	private Dictionary<EUnitEqupmentSlot, EItemType[]> _slotsConfig;
	private ItemSlot[] _equipment = null;

	/// <summary>
	/// C'tor
	/// </summary>
	/// <param name="_slotsData">Data about slot: slot key && available item types for the slot</param>
	/// <param name="onEquipmentUpdate"></param>
	public UnitInventory(Dictionary<EUnitEqupmentSlot, EItemType[]> _slotsData, ArrayRO<ItemSlot> _initialItems, Action<EUnitEqupmentSlot, EItemKey, EItemKey> onEquipmentUpdate) {
		_slotsConfig = _slotsData;

		_equipment = new ItemSlot[_slotsConfig.Count];
		int i = 0;
		foreach(KeyValuePair<EUnitEqupmentSlot, EItemType[]> kvp in _slotsConfig) {
			_equipment[i] = new ItemSlot(kvp.Key, EItemKey.None);
			i++;
		}

		for (i = 0; i < _initialItems.Length; i++) {
			Equip(_initialItems[i].SlotName, _initialItems[i].ItemKey);
		}

		_onEquipmentUpdate = onEquipmentUpdate;
	}

	//equip item
	public void Equip(int slotId, EItemKey itemKey) {
		//check slotId
		if (slotId < 0 || _equipment.Length - 1 < slotId) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongSlotId, slotId);
			return;
		}

		if (!CanEquipItem(itemKey, _equipment[slotId].SlotName)) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongItem, slotId);
			return;
		}

		BaseItem item = ItemsConfig.Instance.GetItem(itemKey);

		EItemKey oldItemKey = GetItemInSlot(slotId);
		_equipment[slotId].ItemKey = item.Key;

		if (_onEquipmentUpdate != null) {
			_onEquipmentUpdate(_equipment[slotId].SlotName, oldItemKey, itemKey);
		}
	}

	public void Equip(EUnitEqupmentSlot slotName, EItemKey itemKey) {
		Equip(SlotNameToId(slotName), itemKey);
	}

	//unequip item
	public void Unequip(EUnitEqupmentSlot itemSlot) {
		for (int i = 0; i < _equipment.Length; i++) {
			if (_equipment[i].SlotName == itemSlot) {
				Equip(i, EItemKey.None);
				break;
			}
		}
	}

	//get item key in slot
	public EItemKey GetItemInSlot(int slotId) {
		if (slotId < 0 || _equipment.Length - 1 < slotId) {
			EventsAggregator.Items.Broadcast<int>(EItemEvent.WrongSlotId, slotId);
			return EItemKey.None;
		}

		return _equipment[slotId].ItemKey;
	}

	public EItemKey GetItemInSlot(EUnitEqupmentSlot slotKey) {
		for (int i = 0; i < _equipment.Length; i++) {
			if (_equipment[i].SlotName == slotKey) {
				return _equipment[i].ItemKey;
			}
		}

		return EItemKey.None;
	}

	//check if item can be equipped
	public bool CanEquipItem(EItemKey itemKey, EUnitEqupmentSlot slotKey) {
		BaseItem item = ItemsConfig.Instance.GetItem(itemKey);

		//check item data found
		if (item == null) {
			return false;
		}

		//check slot is correct
		if (!_slotsConfig.ContainsKey(slotKey)) {
			return false;
		}

		//no slot restrictions
		if (_slotsConfig[slotKey].Length == 0) {
			return true;
		}

		//check item fits to slot
		for (int i = 0; i < _slotsConfig[slotKey].Length; i++) {
			if (_slotsConfig[slotKey][i] == item.Type) {
				return true;
			}
		}

		return false;
	}

	//get slot id by slot name
	public int SlotNameToId(EUnitEqupmentSlot slotName) {
		for(int i = 0; i < _equipment.Length; i++) {
			if(_equipment[i].SlotName == slotName) {
				return i;
			}
		}
		return -1;
	}
}

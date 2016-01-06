using UnityEngine;

[System.Serializable]
public class ItemDropChance {
	[SerializeField]
	private EItemKey _itemKey = EItemKey.None;
	public EItemKey ItemKey {
		get { return _itemKey; }
	}

	[SerializeField]
	private int _dropChance = 0;
	public int DropChance {
		get { return _dropChance; }
	}
}

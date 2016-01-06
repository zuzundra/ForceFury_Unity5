using UnityEngine;

/// <summary>
/// Keeps all items data
/// Configured through prefab
/// </summary>
public class ItemsConfig : MonoBehaviourResourceSingleton<ItemsConfig> {	//TODO: create editor
#pragma warning disable 0414
	private static string _path = "Config/ItemsConfig";
#pragma warning restore 0414

	[SerializeField]
	private BaseItem[] _data = new BaseItem[0];

	public BaseItem GetItem(EItemKey itemKey) {
		for (int i = 0; i < _data.Length; i++) {
			if (_data[i].Key == itemKey) {
				return _data[i];
			}
		}

		return null;
	}

	public bool IsWeapon(EItemKey itemKey) {
		return (int)itemKey > 1000000 && (int)itemKey < 2000000;
	}

	public bool IsWeaponRanged(EItemKey itemKey) {
		if (IsWeapon(itemKey)) {
			return itemKey == EItemKey.W_Gun || itemKey == EItemKey.W_Rifle;
		}
		return false;
	}
}

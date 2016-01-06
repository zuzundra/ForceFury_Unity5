using UnityEngine;

[System.Serializable]
public class CBWarehouseData : CBBaseData {
	[SerializeField]
	private CBWarehouseLevel[] _upgrades = null;
	private ArrayRO<CBWarehouseLevel> _upgradesRO = null;
	public ArrayRO<CBWarehouseLevel> Upgrades {
		get {
			if (_upgradesRO == null) {
				_upgradesRO = new ArrayRO<CBWarehouseLevel>(_upgrades);
			}
			return _upgradesRO;
		}
	}
}

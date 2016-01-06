using UnityEngine;

[System.Serializable]
public class CBBarracksData : CBBaseData {
	[SerializeField]
	private CBBarracksLevel[] _upgrades = null;
	private ArrayRO<CBBarracksLevel> _upgradesRO = null;
	public ArrayRO<CBBarracksLevel> Upgrades {
		get {
			if (_upgradesRO == null) {
				_upgradesRO = new ArrayRO<CBBarracksLevel>(_upgrades);
			}
			return _upgradesRO;
		}
	}
}

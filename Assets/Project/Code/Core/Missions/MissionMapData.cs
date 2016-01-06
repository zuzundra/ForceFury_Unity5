using UnityEngine;

[System.Serializable]
public class MissionMapData {
	[SerializeField]
	private EUnitKey[] _units = null;
	private ArrayRO<EUnitKey> _unitsRO = null;
	public ArrayRO<EUnitKey> Units {
		get {
			if (_unitsRO == null) {
				_unitsRO = new ArrayRO<EUnitKey>(_units);
				_units = null;
			}

			return _unitsRO;
		}
	}

	[SerializeField]
	private string _mapBackgroundPath = string.Empty;
	public string MapBackgroundPath {
		get { return _mapBackgroundPath; }
	}

	public MissionMapData() { }

	public MissionMapData(EUnitKey[] units, string mapBackgroundPath) {
		_units = units;
		_mapBackgroundPath = mapBackgroundPath;
	}
}

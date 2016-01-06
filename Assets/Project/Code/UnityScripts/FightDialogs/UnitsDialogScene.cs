using UnityEngine;

[System.Serializable]
public class UnitsDialogScene {
	[SerializeField]
	private EMissionKey _missionKey = EMissionKey.None;
	public EMissionKey MissionKey {
		get { return _missionKey; }
	}

	[SerializeField]
	private int _mapIndex = 0;
	public int MapIndex {
		get { return _mapIndex; }
	}

	[SerializeField]
	private UnitDialodEntity[] _dialogData = null;
	private ArrayRO<UnitDialodEntity> _dialogDataRO = null;
	public ArrayRO<UnitDialodEntity> DialogData {
		get {
			if (_dialogDataRO == null) {
				_dialogDataRO = new ArrayRO<UnitDialodEntity>(_dialogData);
			}
			return _dialogDataRO;
		}
	}
}

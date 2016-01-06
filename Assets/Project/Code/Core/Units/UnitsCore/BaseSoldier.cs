using UnityEngine;

[System.Serializable]
public class BaseSoldier : BaseUnit {
	protected new BaseSoldierData _data;
	public new BaseSoldierData Data {
		get { return _data; }
	}

	public BaseSoldier(BaseSoldierData data, int level) : base(data) {
		_data = data;
		Level = level;
	}
}

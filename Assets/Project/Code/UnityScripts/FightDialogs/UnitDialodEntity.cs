using UnityEngine;

public enum EFightDialogSpeaker {
	PlayerHero = 0,
	EnemyUnit
}

[System.Serializable]
public class UnitDialodEntity {
	[SerializeField]
	private EFightDialogSpeaker _speaker;
	public EFightDialogSpeaker Speaker {
		get { return _speaker; }
	}

	[SerializeField]
	private EUnitKey _unitKey;
	public EUnitKey UnitKey {
		get { return _unitKey; }
	}

	[SerializeField]
	private string _text;
	public string Text {
		get { return _text; }
	}

	[SerializeField]
	private string _prefabPath = "FightDialogs/UnitMonolog";
	public string PrefabPath {
		get { return _prefabPath; }
	}

	[SerializeField]
	private Vector3 _cameraOffset = new Vector3(-0.495f, 1.316f, 1.342f);
	public Vector3 CameraOffset {
		get { return _cameraOffset; }
	}
}

using UnityEngine;

[System.Serializable]
public class PlanetData {
	[SerializeField]
	private EPlanetKey _key = EPlanetKey.None;
	public EPlanetKey Key {
		get { return _key; }
	}

	[SerializeField]
	private int _creditsIncome = 0;
	public int CreditsIncome {
		get { return _creditsIncome; }
	}

	[SerializeField]
	private MissionData[] _missions = new MissionData[0];
	private ArrayRO<MissionData> _missionsRO = null;
	public ArrayRO<MissionData> Missions {
		get {
			if (_missionsRO == null) {
				_missionsRO = new ArrayRO<MissionData>(_missions);
			}
			return _missionsRO;
		}
	}

	[SerializeField]
	private string _sceneName = string.Empty;
	public string SceneName {
		get { return _sceneName; }
	}

	public MissionData GetMission(EMissionKey missionKey) {
		for (int i = 0; i < _missions.Length; i++) {
			if (_missions[i].Key == missionKey) {
				return _missions[i];
			}
		}
		return null;
	}
}
using System.Collections.Generic;

/// <summary>
/// Information about completed levels, missions, etc
/// </summary>
public class PlayerStoryProgress {
	private Dictionary<EPlanetKey, List<EMissionKey>> _progress = new Dictionary<EPlanetKey, List<EMissionKey>>();
	private Dictionary<EPlanetKey, Dictionary<EMissionKey, int>> _dailyMissionAttempts = new Dictionary<EPlanetKey, Dictionary<EMissionKey, int>>();

	public void SaveProgress(EPlanetKey planetKey, EMissionKey missionKey) {
		if (!IsMissionCompleted(planetKey, missionKey)) {
			if (!_progress.ContainsKey(planetKey)) {
				_progress.Add(planetKey, new List<EMissionKey>());
			}
			_progress[planetKey].Add(missionKey);
		}
	}

	#region missions completion and availability
	public bool IsMissionCompleted(EPlanetKey planetKey, EMissionKey missionKey) {
		if (!_progress.ContainsKey(planetKey)) {
			return false;
		}
		return _progress[planetKey].IndexOf(missionKey) != -1;
	}

	public bool IsMissioAvailable(EPlanetKey planetKey, EMissionKey missionKey) {
		PlanetData pd = MissionsConfig.Instance.GetPlanet(planetKey);
		if(pd != null) {
			for (int i = 0; i < pd.Missions.Length; i++) {
				if (pd.Missions[i].Key == missionKey) {
					//first mission
					if (i == 0) {
						if (MissionsConfig.Instance.Planets[i].Key == pd.Key) {
							//first planet
							return true;
						} else {
							//check prev planet
							return IsPlanetCompleted(MissionsConfig.Instance.GetPreviuosPlanet(planetKey).Key);
						}
					} else {
						//check prev mission
						return IsMissionCompleted(planetKey, pd.Missions[i - 1].Key);
					}
				}
			}
		}

		return false;
	}

	public bool IsPlanetCompleted(EPlanetKey planetKey) {
		if (!_progress.ContainsKey(planetKey)) {
			return false;
		}

		PlanetData pd = MissionsConfig.Instance.GetPlanet(planetKey);
		for (int i = 0; i < pd.Missions.Length; i++) {
			if (!IsMissionCompleted(planetKey, pd.Missions[i].Key)) {
				return false;
			}
		}

		return true;
	}
	#endregion

	#region attempts
	//TODO: reset attempts after midnight

	public int GetMissionAttemptsUsed(EPlanetKey planetKey, EMissionKey missionKey) {
		if (_dailyMissionAttempts.ContainsKey(planetKey) && _dailyMissionAttempts[planetKey].ContainsKey(missionKey)) {
			return _dailyMissionAttempts[planetKey][missionKey];
		}

		return 0;
	}

	public void RegisterAttemptUsage(EPlanetKey planetKey, EMissionKey missionKey) {
		if (!_dailyMissionAttempts.ContainsKey(planetKey)) {
			_dailyMissionAttempts.Add(planetKey, new Dictionary<EMissionKey,int>());
		}
		if(!_dailyMissionAttempts[planetKey].ContainsKey(missionKey)) {
			_dailyMissionAttempts[planetKey].Add(missionKey, 1);
		} else {
			_dailyMissionAttempts[planetKey][missionKey]++;
		}
	}
	#endregion
}

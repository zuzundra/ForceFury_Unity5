using UnityEngine;

/// <summary>
/// Data that stored in player profile
/// </summary>
public class CityBuildingInfo {
	public ECityBuildingKey Key { get; private set; }

	public int Level { get; set; }

	public int ConstructionCompletionTimestamp { get; set; }
	public bool IsUnderCoustruction {
		get { return ConstructionCompletionTimestamp > -1; }
	}

	public bool IsLevelMaxed {
		get {
			return Level >= CityConfig.Instance.GetBuildingData(Key).MaxLevel;
		}
	}

	public CityBuildingInfo(ECityBuildingKey key, int level) {
		ConstructionCompletionTimestamp = -1;

		Key = key;
		Level = level;
	}
}

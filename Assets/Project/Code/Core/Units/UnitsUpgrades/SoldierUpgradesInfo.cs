using UnityEngine;

/// <summary>
/// Data that will store player upgrades progress
/// </summary>
public class SoldierUpgradesInfo {
	public EUnitKey UnitKey { get; private set; }
	public int Level { get; private set; }	//TODO: dispatch level up event

	public SoldierUpgradesInfo(EUnitKey unitKey) {
		UnitKey = unitKey;
		Level = 1;
	}

	public SoldierUpgradesInfo(EUnitKey unitKey, int currentLevel) {
		if (currentLevel < 1 || currentLevel > GameConstants.City.MAX_UNIT_UPGRADE_LEVEL) {
			Debug.LogError("Wrong unit level: " + unitKey + " - " + currentLevel);
			currentLevel = 1;
		}

		UnitKey = unitKey;
		Level = currentLevel;
	}

	public void LevelUp() {
		if (Level < GameConstants.City.MAX_UNIT_UPGRADE_LEVEL) {
			Level++;
		} else {
			Debug.LogWarning("Max unit level reached: " + UnitKey);
		}
	}
}

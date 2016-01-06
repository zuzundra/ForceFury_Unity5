using UnityEngine;

[System.Serializable]
public class ConstructionBuildingRequirement {
	[SerializeField]
	private ECityBuildingKey _key;
	public ECityBuildingKey Key {
		get { return _key; }
	}

	[SerializeField]
	private int _level;
	public int Level {
		get { return _level; }
	}
}

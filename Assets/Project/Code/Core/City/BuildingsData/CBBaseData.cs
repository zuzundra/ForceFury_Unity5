using UnityEngine;
using System.Collections;

[System.Serializable]
public class CBBaseData {
	[SerializeField]
	protected CBConstructionRequirement[] _levelsRequirements = null;
	private ArrayRO<CBConstructionRequirement> _levelsRequirementsRO = null;
	public ArrayRO<CBConstructionRequirement> LevelsRequirements {
		get {
			if (_levelsRequirementsRO == null) {
				_levelsRequirementsRO = new ArrayRO<CBConstructionRequirement>(_levelsRequirements);
			}
			return _levelsRequirementsRO;
		}
	}

	public int MaxLevel {
		get { return _levelsRequirements.Length; }
	}

	public CBConstructionRequirement GetConstructionRequirements(int buildingLevel) {
		if (buildingLevel > 0 && buildingLevel <= _levelsRequirements.Length) {
			return _levelsRequirements[buildingLevel - 1];
		}
		return null;
	}
}

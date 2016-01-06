using UnityEngine;

[System.Serializable]
public class CBConstructionRequirement {
	[SerializeField]
	private string _iconPath;	//path to icon
	public string IconPath {
		get { return _iconPath; }
	}

	[SerializeField]
	private int _buildTime = 0;	//construction time (in seconds)
	public int BuildTime {
		get { return _buildTime; }
	}

	[SerializeField]
	private int _costCredits = 0;	//construction cost in credits
	public int CostCredits {
		get { return _costCredits; }
	}

	[SerializeField]
	private int _costFuel = 0;	//Construction cost in fuel
	public int CostFuel {
		get { return _costFuel; }
	}

	[SerializeField]
	private int _costMinerals = 0;	//construction cost in minerals
	public int CostMinerals {
		get { return _costMinerals; }
	}

	[SerializeField]
	private ConstructionBuildingRequirement[] _buildingRequirements = null;
	private ArrayRO<ConstructionBuildingRequirement> _buildingRequirementsRO = null;
	public ArrayRO<ConstructionBuildingRequirement> BuildingRequirements {
		get {
			if (_buildingRequirementsRO == null) {
				_buildingRequirementsRO = new ArrayRO<ConstructionBuildingRequirement>(_buildingRequirements);
			}
			return _buildingRequirementsRO;
		}
	}

}

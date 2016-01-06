using UnityEngine;

[System.Serializable]
public class MissionData {
	[SerializeField]
	private EMissionKey _key = EMissionKey.None;
	public EMissionKey Key {
		get { return _key; }
	}

	[SerializeField]
	private int _fuelWinCost = 0;
	public int FuelWinCost {
		get { return _fuelWinCost; }
	}

	[SerializeField]
	private int _fuelLoseCost = 0;
	public int FuelLoseCost {
		get { return _fuelLoseCost; }
	}

	[SerializeField]
	private int _creditsWinCost = 0;
	public int CreditsWinCost {
		get { return _creditsWinCost; }
	}

	[SerializeField]
	private int _creditsLoseCost = 0;
	public int CreditsLoseCost {
		get { return _creditsLoseCost; }
	}

	[SerializeField]
	private int _mineralsWinCost = 0;
	public int MineralsWinCost {
		get { return _mineralsWinCost; }
	}

	[SerializeField]
	private int _mineralsLoseCost = 0;
	public int MineralsLoseCost {
		get { return _mineralsLoseCost; }
	}

	[SerializeField]
	private int _rewardExperienceWin = 0;
	public int RewardExperienceWin {
		get { return _rewardExperienceWin; }
	}

	[SerializeField]
	private int _rewardExperienceLose = 0;
	public int RewardExperienceLose {
		get { return _rewardExperienceLose; }
	}

	[SerializeField]
	private int _rewardFuel = 0;
	public int RewardFuel {
		get { return _rewardFuel; }
	}

	[SerializeField]
	private int _rewardCredits = 0;
	public int RewardCredits {
		get { return _rewardCredits; }
	}

	[SerializeField]
	private int _rewardMinerals = 0;
	public int RewardMinerals {
		get { return _rewardMinerals; }
	}

	[SerializeField]
	private ItemDropChance[] _rewardItems;
	public ArrayRO<ItemDropChance> _rewardItemsRO;
	public ArrayRO<ItemDropChance> RewardItems {
		get {
			if (_rewardItemsRO == null) {
				_rewardItemsRO = new ArrayRO<ItemDropChance>(_rewardItems);
			}
			return _rewardItemsRO;
		}
	}

	[SerializeField]
	private int _attemptsDaily = 5;
	public int AttemptsDaily {
		get { return _attemptsDaily; }
	}

	[SerializeField]
	private int _mineIncome = 0;
	public int MineIncome {
		get { return _mineIncome; }
	}
	public bool HasMine {
		get { return _mineIncome > 0; }
	}

	[SerializeField]
	private MissionMapData[] _maps = new MissionMapData[0];
	public int MapsCount {
		get { return _maps.Length; }
	}

	public MissionData() { }

	public MissionData(EMissionKey key, int fuelWinCost, int fuelLoseCost, int creditsWinCost, int creditsLoseCost, int mineralsWinCost, int mineralsLoseCost, int rewardExperienceWin, int rewardExperienceLose, int rewardFuel, int rewardCredits, int rewardMinerals, MissionMapData[] maps) {
		_key = key;
		_fuelWinCost = fuelWinCost;
		_fuelLoseCost = fuelLoseCost;
		_creditsWinCost = creditsWinCost;
		_creditsLoseCost = creditsLoseCost;
		_mineralsWinCost = mineralsWinCost;
		_mineralsLoseCost = mineralsLoseCost;
		_rewardExperienceWin = rewardExperienceWin;
		_rewardExperienceLose = rewardExperienceLose;
		_rewardFuel = rewardFuel;
		_rewardCredits = rewardCredits;
		_rewardMinerals = rewardMinerals;
		_maps = maps;
	}

	public MissionMapData GetMap(int index) {
		return index >= 0 || index < _maps.Length ? _maps[index] : null;
	}
}

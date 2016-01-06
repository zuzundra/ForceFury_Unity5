using UnityEngine;

[System.Serializable]
public class CBWarehouseLevel {
	[SerializeField]
	private int _limitCredits = 0;
	public int LimitCredits {
		get { return _limitCredits; }
	}

	[SerializeField]
	private int _limitMinerals = 0;
	public int LimitMinerals {
		get { return _limitMinerals; }
	}

	[SerializeField]
	private int _limitFuel = 0;
	public int LimitFuel {
		get { return _limitFuel; }
	}

	[SerializeField]
	private int _bonusCreditsProduction = 0;
	public int BonusCreditsProduction {
		get { return _bonusCreditsProduction; }
	}

	[SerializeField]
	private int _bonusMineralsProduction = 0;
	public int BonusMineralsProduction {
		get { return _bonusMineralsProduction; }
	}

	[SerializeField]
	private int _bonusFuelProduction = 0;
	public int BonusFuelProduction {
		get { return _bonusFuelProduction; }
	}
}

using UnityEngine;
using System.Collections.Generic;

public class CityConfig : MonoBehaviourResourceSingleton<CityConfig> {
#pragma warning disable 0414
	private static string _path = "Config/CityConfig";
#pragma warning restore 0414

	[SerializeField]
	private CBTownHallData _townHall = null;
	public CBTownHallData TownHall {
		get { return _townHall; }
	}

	[SerializeField]
	private CBBarracksData _barracks = null;
	public CBBarracksData Barracks {
		get { return _barracks; }
	}

	[SerializeField]
	private CBWarehouseData _warehouse = null;
	public CBWarehouseData Warehouse {
		get { return _warehouse; }
	}

	[SerializeField]
	private CBMarketData _market = null;
	public CBMarketData Market {
		get { return _market; }
	}

	[SerializeField]
	private CBBaseData _fort = null;
	public CBBaseData Fort {
		get { return _fort; }
	}

	[SerializeField]
	private CBBaseData _heroesHall = null;
	public CBBaseData HeroesHall {
		get { return _heroesHall; }
	}

	public CBBaseData GetBuildingData(ECityBuildingKey buildingKey) {
		switch (buildingKey) {
			case ECityBuildingKey.TownHall:
				return _townHall;
			case ECityBuildingKey.Barracks:
				return _barracks;
			case ECityBuildingKey.Market:
				return _market;
			case ECityBuildingKey.Warehouse:
				return _warehouse;
			case ECityBuildingKey.Fort:
				return _fort;
			case ECityBuildingKey.HeroesHall:
				return _heroesHall;
		}

		return null;
	}

	#region barracks
	public List<EUnitKey> GetUnitsList(int barracksLevel) {
		barracksLevel = Mathf.Min(barracksLevel, _barracks.Upgrades.Length);
		barracksLevel = Mathf.Max(barracksLevel, 0);

		List<EUnitKey> result = new List<EUnitKey>();
		for (int i = 0; i < barracksLevel; i++) {
			for (int j = 0; j < _barracks.Upgrades[i].AvailableUnits.Length; j++) {
				result.Add(_barracks.Upgrades[i].AvailableUnits[j]);
			}
		}

		return result;
	}
	#endregion

	#region warehouse
	public int GetWarehouseCreditsLimit(int warehouseLevel) {
		warehouseLevel = Mathf.Min(warehouseLevel, _warehouse.Upgrades.Length);
		warehouseLevel = Mathf.Max(warehouseLevel, 0);

		return _warehouse.Upgrades[warehouseLevel - 1].LimitCredits;
	}

	public int GetWarehouseMineralsLimit(int warehouseLevel) {
		warehouseLevel = Mathf.Min(warehouseLevel, _warehouse.Upgrades.Length);
		warehouseLevel = Mathf.Max(warehouseLevel, 0);

		return _warehouse.Upgrades[warehouseLevel - 1].LimitMinerals;
	}

	public int GetWarehouseFuelLimit(int warehouseLevel) {
		warehouseLevel = Mathf.Min(warehouseLevel, _warehouse.Upgrades.Length);
		warehouseLevel = Mathf.Max(warehouseLevel, 0);

		return _warehouse.Upgrades[warehouseLevel - 1].LimitFuel;
	}
	#endregion
}

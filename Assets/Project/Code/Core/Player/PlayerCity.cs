using System;
using System.Collections.Generic;

/// <summary>
/// Information about player city (buildings, unit upgrades, etc.)
/// </summary>
public class PlayerCity {
	#region city data
	private CityBuildingInfo[] _buildings = null;
	private Dictionary<EUnitKey, SoldierUpgradesInfo> _unitUpgrades = new Dictionary<EUnitKey, SoldierUpgradesInfo>();
	public int WarehouseCollectTime { get; private set; }
	#endregion

#pragma warning disable 0414
	private PlayerFuelRefresher _fuelRefresher = null;
#pragma warning restore 0414
	
	public PlayerCity() {
		ECityBuildingKey[] buildingKeys = Enum.GetValues(typeof(ECityBuildingKey)) as ECityBuildingKey[];
		_buildings = new CityBuildingInfo[buildingKeys.Length - 1];
		for (int i = 1; i < buildingKeys.Length; i++) {
			_buildings[i - 1] = new CityBuildingInfo(buildingKeys[i], 1);
		}

		//TODO: setup correct last refresh time
		_fuelRefresher = new PlayerFuelRefresher(Utils.UnixTimestamp);
		//TODO: start ongoing constructions

		//TODO: setup correct buildings info
		//TODO: setup correct upgrades info
		//TOOD: setup warehouse collect time
		if (IsWarehouseFilled) {
			OnWarehouseFilled();
		} else {
			GameTimer.Instance.AddListener(WarehouseCollectTime, OnWarehouseFilled);
		}
	}

	public CityBuildingInfo GetBuilding(ECityBuildingKey buildingKey) {
		for (int i = 0; i < _buildings.Length; i++) {
			if (_buildings[i].Key == buildingKey) {
				return _buildings[i];
			}
		}
		return null;
	}

	#region construction
	public void StartConstruction(ECityBuildingKey buildingKey) {
		CityBuildingInfo bInfo = GetBuilding(buildingKey);
		
		//check level
		if (bInfo.IsLevelMaxed) {
			return;
		}
		//check current building is not under construction
		if (bInfo.IsUnderCoustruction) {
			return;
		}

		CBConstructionRequirement requirements = CityConfig.Instance.GetBuildingData(buildingKey).GetConstructionRequirements(bInfo.Level + 1);

		//check resources requirements
		if (Global.Instance.Player.Resources.Credits < requirements.CostCredits ||
			Global.Instance.Player.Resources.Minerals < requirements.CostMinerals ||
			Global.Instance.Player.Resources.Fuel < requirements.CostFuel) {
				return;
		}

		//check necessary buildings requirements
		for (int i = 0; i < requirements.BuildingRequirements.Length; i++) {
			if (GetBuilding(requirements.BuildingRequirements[i].Key).Level < requirements.BuildingRequirements[i].Level) {
				return;
			}
		}

		//all conditions match: remove resources and start construction
		Global.Instance.Player.Resources.Credits -= requirements.CostCredits;
		Global.Instance.Player.Resources.Minerals -= requirements.CostMinerals;
		Global.Instance.Player.Resources.Fuel -= requirements.CostFuel;
		bInfo.ConstructionCompletionTimestamp = Utils.UnixTimestamp + requirements.BuildTime;

		StartConstructionInternal(buildingKey, bInfo.ConstructionCompletionTimestamp);
	}

	private void StartConstructionInternal(ECityBuildingKey buildingKey, int constructionCompleteTimestamp) {
		EventsAggregator.City.Broadcast<ECityBuildingKey>(ECityEvent.ConstructionStart, buildingKey);
		if (constructionCompleteTimestamp - Utils.UnixTimestamp > 0) {
			GameTimer.Instance.AddListener(constructionCompleteTimestamp - Utils.UnixTimestamp, delegate() { OnConstructionComplete(buildingKey); });
		} else {
			OnConstructionComplete(buildingKey);
		}
	}

	private void OnConstructionComplete(ECityBuildingKey buildingKey) {
		CityBuildingInfo bInfo = GetBuilding(buildingKey);
		bInfo.Level++;
		bInfo.ConstructionCompletionTimestamp = -1;

		EventsAggregator.City.Broadcast<ECityBuildingKey>(ECityEvent.ConstructionEnd, buildingKey);
	}
	#endregion

	#region barracks
	public List<EUnitKey> AvailableUnits {
		get { return CityConfig.Instance.GetUnitsList(GetBuilding(ECityBuildingKey.Barracks).Level); }
	}

	public SoldierUpgradesInfo GetSoldierUpgradesInfo(EUnitKey unitKey) {
		if (!_unitUpgrades.ContainsKey(unitKey)) {
			_unitUpgrades.Add(unitKey, new SoldierUpgradesInfo(unitKey));
		}
		return _unitUpgrades[unitKey];
	}

	public void SoldierLevelUp(EUnitKey unitKey) {
		GetSoldierUpgradesInfo(unitKey).LevelUp();
	}

	public void SetSoldierLevel(EUnitKey unitKey, int level) {
		for (int i = GetSoldierUpgradesInfo(unitKey).Level; i < level; i++) {
			GetSoldierUpgradesInfo(unitKey).LevelUp();
		}
	}
	#endregion

	#region warehouse
	public bool IsWarehouseFilled {
		get { return Utils.UnixTimestamp >= WarehouseCollectTime; }
	}

	public int WarehouseCreditsLimit {
		get { return CityConfig.Instance.GetWarehouseCreditsLimit(GetBuilding(ECityBuildingKey.Warehouse).Level); }
	}

	public int WarehouseMineralsLimit {
		get { return CityConfig.Instance.GetWarehouseMineralsLimit(GetBuilding(ECityBuildingKey.Warehouse).Level); }
	}

	public int WarehouseFuelLimit {
		get { return CityConfig.Instance.GetWarehouseFuelLimit(GetBuilding(ECityBuildingKey.Warehouse).Level); }
	}

	public void CollectResourcesFromWarehouse() {
		if (!IsWarehouseFilled) {
			return;
		}

		int warehouseCreditsLimit = WarehouseCreditsLimit;
		int warehouseMineralsLimit = WarehouseMineralsLimit;
		//int warehouseFuelLimit = WarehouseFuelLimit;

		int creditsToCollect = 0;
		int mineralsToCollect = 0;
		//int fuelToCollect = 0;

		bool checkEnd = false;
		for (int i = 0; i < MissionsConfig.Instance.Planets.Length; i++) {
			if (checkEnd) {
				break;
			}

			if (Global.Instance.Player.StoryProgress.IsPlanetCompleted(MissionsConfig.Instance.Planets[i].Key)) {
				creditsToCollect += MissionsConfig.Instance.Planets[i].CreditsIncome;

				for (int j = 0; j < MissionsConfig.Instance.Planets[i].Missions.Length; j++) {
					if (MissionsConfig.Instance.Planets[i].Missions[j].HasMine) {
						mineralsToCollect += MissionsConfig.Instance.Planets[i].Missions[j].MineIncome;
					}
				}
			} else {
				for (int j = 0; j < MissionsConfig.Instance.Planets[i].Missions.Length; j++) {
					if (Global.Instance.Player.StoryProgress.IsMissionCompleted(MissionsConfig.Instance.Planets[i].Key, MissionsConfig.Instance.Planets[i].Missions[j].Key)) {
						if (MissionsConfig.Instance.Planets[i].Missions[j].HasMine) {
							mineralsToCollect += MissionsConfig.Instance.Planets[i].Missions[j].MineIncome;
						}
					} else {
						checkEnd = true;
						break;
					}
					
				}
			}
		}

		creditsToCollect = Math.Min(creditsToCollect, warehouseCreditsLimit);
		mineralsToCollect = Math.Min(mineralsToCollect, warehouseMineralsLimit);
		//fuelToCollect = Math.Min(fuelToCollect, warehouseFuelLimit);

		Global.Instance.Player.Resources.Credits += creditsToCollect;
		Global.Instance.Player.Resources.Minerals += mineralsToCollect;
		//Global.Instance.Player.Resources.Fuel += fuelToCollect;

		WarehouseCollectTime = Utils.UnixTimestamp + GameConstants.City.WAREHOUSE_FILLING_TIME;
	}

	private void OnWarehouseFilled() {
		EventsAggregator.City.Broadcast(ECityEvent.WarehouseFilled);
	}
	#endregion
}

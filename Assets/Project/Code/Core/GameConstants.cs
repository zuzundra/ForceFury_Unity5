using UnityEngine;

public static class GameConstants {
	public const int DEFAULT_RESOLUTION_WIDTH = 2048;
	public const int DEFAULT_RESOLUTION_HEIGHT = 1152;
	public static Vector3 CAMERA_ROTATION { get { return new Vector3(0f, 15f, 0f); } }

	public class Scenes {
		public const string CITY = "City";
		public const string FIGHT = "Fight";
	}

	public class Paths {
		public const string ITEM_RESOURCES = "Items";
		public const string UNIT_RESOURCES = "Units";
        public const string UNIT_PREFAB_RESOURCES = "Units/Prefabs";
		public const string UI_RESOURCES = "UI";

		public const string UI_CITY_RESOURCES = "UI/City";
		public const string UI_CITY_BUILDINGS_RESOURCES = "UI/City/Buildings";

		public const string UI_WINDOWS_RESOURCES = "UI/Windows";
        public const string UI_WINDOWS_PREFAB_RESOURCES = "UI/Windows/Units_prefabs";

		public static string UI_UNIT_ICONS_RESOURCES { get { return string.Format("{0}/Icons/Units", UI_RESOURCES); } }
		public static string UI_ITEM_ICONS_RESOURCES { get { return string.Format("{0}/Icons/Items", UI_RESOURCES); } }
		public static string UI_ABILITY_ICONS_RESOURCES { get { return string.Format("{0}/Icons/Skills", UI_RESOURCES); } }

		public static string UI_MAP_BACKGROUND_RESOURCES = "Maps/Backgrounds";

		public class Prefabs {
            public static string UI_UNIT { get { return string.Format("{0}/UnitUI", UI_RESOURCES); } }

			public static string UI_WIN_BATTLE_PREVIEW { get { return string.Format("{0}/Missions/WndBattlePreview", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_SETUP { get { return string.Format("{0}/Missions/WndBattleSetup", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_VICTORY { get { return string.Format("{0}/Missions/WndBattleVictory", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_BATTLE_DEFEAT { get { return string.Format("{0}/Missions/WndBattleDefeat", UI_WINDOWS_RESOURCES); } }

			public static string UI_WIN_CITY_BUILDING_UPGRADE { get { return string.Format("{0}/City/WndBuildingUpgrade", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_CITY_BARRACKS { get { return string.Format("{0}/City/WndCityBarracks", UI_WINDOWS_RESOURCES); } }

			public static string UI_WIN_HEROES_LIST { get { return string.Format("{0}/City/WndHeroesList", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_HERO_INFO { get { return string.Format("{0}/City/WndHeroInfo", UI_WINDOWS_RESOURCES); } }

			public static string UI_WIN_PVP_MODE_SELECT { get { return string.Format("{0}/PvP/WndPvPModeSelect", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_PVP_BATTLE_SETUP { get { return string.Format("{0}/PvP/WndPvPBattleSetup", UI_WINDOWS_RESOURCES); } }

			public static string UI_WIN_SETS { get { return string.Format("{0}/City/WndSets", UI_WINDOWS_RESOURCES); } }
			public static string UI_WIN_SHOP { get { return string.Format("{0}/City/WndShop", UI_WINDOWS_RESOURCES); } }
            public static string UI_WIN_UPGRADES { get { return string.Format("{0}/City/WndUpgrades", UI_WINDOWS_RESOURCES); } }

            public static string UI_WIN_UNIT_SELECT { get { return string.Format("{0}/Missions/WndUnitSelect", UI_WINDOWS_RESOURCES); } }
            public static string UI_WIN_UNIT_CONFIRM { get { return string.Format("{0}/Missions/WndUnitConfirm", UI_WINDOWS_RESOURCES); } }

			public static string UI_WIN_PLANET_OVERAY { get { return string.Format("{0}/Missions/WndPlanetOverlay", UI_WINDOWS_RESOURCES); } }
		}

		#region auxiliary
		public static string GetUnitIconResourcePath(EUnitKey unitKey) {
			return GetUnitIconResourcePath(UnitsConfig.Instance.GetSoldierData(unitKey).IconName);
		}

		public static string GetUnitIconResourcePath(string iconName) {
			return string.Format("{0}/{1}", UI_UNIT_ICONS_RESOURCES, iconName);
		}

		public static string GetUnitBGIconResourcePath(EUnitKey unitKey) {
			return GetUnitBGIconResourcePath(UnitsConfig.Instance.GetSoldierData(unitKey).IconName);
		}

		public static string GetUnitBGIconResourcePath(string iconName) {
			return string.Format("{0}/Borders/EnemyBorder_bg", UI_UNIT_ICONS_RESOURCES, iconName);

		//public static string GetUnitBGIconResourcePath(string iconName) {
			//return string.Format("{0}/{1}_bg", UI_UNIT_ICONS_RESOURCES, iconName); старый код для бэка
		}

		public static string GetLootIconResourcePath(EItemKey itemKey) {
			return GetLootIconResourcePath(ItemsConfig.Instance.GetItem(itemKey).IconName);
		}

		public static string GetLootIconResourcePath(string iconName) {
			return string.Format("{0}/{1}", UI_ITEM_ICONS_RESOURCES, iconName);
		}
		#endregion
	}

	public class Tags {
		public const string UNIT_ALLY = "UnitAlly";
		public const string UNIT_ENEMY = "UnitEnemy";
	}

	public class City {
		public const int MAX_BUILDING_LEVEL = 10;
		public const int MAX_UNIT_UPGRADE_LEVEL = 10;
		public const int WAREHOUSE_FILLING_TIME = 21600;	//6 hours

		public const int FUEL_REFRESH_TIME = 6000;
		public const int FUEL_REFRESH_AMOUNT = 1;
	}
}

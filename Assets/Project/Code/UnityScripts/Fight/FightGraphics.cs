using System.Collections.Generic;
using UnityEngine;

public class FightGraphics {
	private Dictionary<EUnitKey, BaseUnitBehaviour> _allyUnitsGraphicsResources = new Dictionary<EUnitKey, BaseUnitBehaviour>();
	private Dictionary<EUnitKey, BaseUnitBehaviour> _enemyUnitsGraphicsResources = new Dictionary<EUnitKey, BaseUnitBehaviour>();
	private Sprite _backgroundResource = null;

	private Dictionary<EItemKey, GameObject[]> _allyItemsGraphicsResources = new Dictionary<EItemKey, GameObject[]>();
	private Dictionary<EItemKey, GameObject[]> _enemyItemsGraphicsResources = new Dictionary<EItemKey, GameObject[]>();

	private GameObject _unitUIResource;
	public GameObject UnitUIResource {
		get { return _unitUIResource; }
	}

	private ArrayRO<BaseUnitBehaviour> _allyUnits = null;
	public ArrayRO<BaseUnitBehaviour> AllyUnits {
		get { return _allyUnits; }
	}

	private ArrayRO<BaseUnitBehaviour> _enemyUnits = null;
	public ArrayRO<BaseUnitBehaviour> EnemyUnits {
		get { return _enemyUnits; }
	}

	public void Load(MissionMapData mapData) {
		LoadResources(mapData);
		InstantiateGraphics(mapData);

		LoadingScreen.Instance.SetProgress(0.25f);
	}

	public void Unload(bool fullUnload) {
		DestroyInstances(fullUnload);
		UnloadResources(fullUnload);
	}

	#region instances management
	private void InstantiateGraphics(MissionMapData mapData) {
		InstantiateBackground(mapData);
		InstantiateAllyUnits();
		InstantiateEnemyUnits(mapData);
	}

	private void InstantiateBackground(MissionMapData mapData) {
		if (_backgroundResource != null) {
			//FightManager.SceneInstance.UI.ImgMapBackground.sprite = _backgroundResource;
		}
	}

	private void InstantiateAllyUnits() {
		//check if this is first map
		if (_allyUnits != null) {
			return;
		}

		//TODO: setup inventory

		ArrayRO<BaseSoldier> playerSoldiersList = Global.Instance.CurrentMission.SelectedSoldiers;
		BaseUnitBehaviour[] unitsList = new BaseUnitBehaviour[playerSoldiersList.Length + 1];
		BaseUnitBehaviour bub = null;

		//instantiate soldiers
		for (int i = 0; i < playerSoldiersList.Length; i++) {
			if (!playerSoldiersList[i].IsDead) {
				bub = (GameObject.Instantiate(_allyUnitsGraphicsResources[playerSoldiersList[i].Data.Key].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
				bub.gameObject.name = "Ally_" + playerSoldiersList[i] + " (" + i + ")";
				unitsList[i] = bub;

				LoadItemsResources(_allyItemsGraphicsResources, playerSoldiersList[i].Data.Key, bub.ModelView, playerSoldiersList[i].Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_RHand), playerSoldiersList[i].Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_LHand), playerSoldiersList[i].Inventory.GetItemInSlot(EUnitEqupmentSlot.Armor));
			}
		}

		//instantiate player
		BaseHero playerHero = Global.Instance.Player.Heroes.Current;
		if (!playerHero.IsDead) {
			bub = (GameObject.Instantiate(_allyUnitsGraphicsResources[playerHero.Data.Key].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
			bub.gameObject.name = "Ally_" + playerHero.Data.Key;
			unitsList[unitsList.Length - 1] = bub;

			LoadItemsResources(_allyItemsGraphicsResources, playerHero.Data.Key, bub.ModelView, playerHero.Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_RHand), playerHero.Inventory.GetItemInSlot(EUnitEqupmentSlot.Weapon_LHand), playerHero.Inventory.GetItemInSlot(EUnitEqupmentSlot.Armor));
		}

		//save
		_allyUnits = new ArrayRO<BaseUnitBehaviour>(unitsList);
	}

	private void InstantiateEnemyUnits(MissionMapData mapData) {
		//TODO: setup inventory

		BaseUnitBehaviour[] unitsList = new BaseUnitBehaviour[mapData.Units.Length];

		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//instantiate soldiers
		for (int i = 0; i < mapData.Units.Length; i++) {
			bub = (GameObject.Instantiate(_enemyUnitsGraphicsResources[mapData.Units[i]].gameObject) as GameObject).GetComponent<BaseUnitBehaviour>();
			bub.gameObject.name = "Enemy_" + mapData.Units[i] + " (" + i + ")";
			unitsList[i] = bub;

			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			LoadItemsResources(_enemyItemsGraphicsResources, mapData.Units[i], bub.ModelView, bud.GetBaseItemInSlot(EUnitEqupmentSlot.Weapon_RHand), bud.GetBaseItemInSlot(EUnitEqupmentSlot.Weapon_LHand), bud.GetBaseItemInSlot(EUnitEqupmentSlot.Armor));
		}

		//save
		_enemyUnits = new ArrayRO<BaseUnitBehaviour>(unitsList);
	}
	#endregion

	#region resources management
	private void LoadResources(MissionMapData mapData) {
		LoadBackgroundResources(mapData);
		LoadAllyUnitsResources();
		LoadEnemyUnitsResources(mapData);

		_unitUIResource = Resources.Load(GameConstants.Paths.Prefabs.UI_UNIT) as GameObject;
	}

	private void LoadBackgroundResources(MissionMapData mapData) {
		if (!mapData.MapBackgroundPath.Equals(string.Empty)) {
			_backgroundResource = UIResourcesManager.Instance.GetResource<Sprite>(string.Format("{0}/{1}", GameConstants.Paths.UI_MAP_BACKGROUND_RESOURCES, mapData.MapBackgroundPath));
		}
	}

	private void LoadAllyUnitsResources() {
		//check if this is first map
		if (_allyUnitsGraphicsResources.Count > 0) {
			return;
		}

		ArrayRO<BaseSoldier> playerSoldiersList = Global.Instance.CurrentMission.SelectedSoldiers;

		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//load unique soldiers
		for (int i = 0; i < playerSoldiersList.Length; i++) {
			if (_allyUnitsGraphicsResources.ContainsKey(playerSoldiersList[i].Data.Key)) {
				continue;
			}

			bud = UnitsConfig.Instance.GetSoldierData(playerSoldiersList[i].Data.Key);
			if (bud != null && !bud.PrefabName.Equals(string.Empty)) {
				bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNIT_RESOURCES, bud.PrefabName));
				_allyUnitsGraphicsResources.Add(playerSoldiersList[i].Data.Key, bub);
			} else {
				Debug.LogError("Can't load unit graphics: " + playerSoldiersList[i].Data.Key);
			}
		}

		//load hero
		BaseHero playerHero = Global.Instance.Player.Heroes.Current;
		if (playerHero == null || playerHero.Data.PrefabName.Equals(string.Empty)) {
			Debug.LogError("Can't load unit graphics for player hero");
		}

		bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNIT_RESOURCES, playerHero.Data.PrefabName));
		_allyUnitsGraphicsResources.Add(playerHero.Data.Key, bub);
	}

	private void LoadEnemyUnitsResources(MissionMapData mapData) {
		BaseUnitData bud = null;
		BaseUnitBehaviour bub = null;

		//load unique units
		for (int i = 0; i < mapData.Units.Length; i++) {
			if (_enemyUnitsGraphicsResources.ContainsKey(mapData.Units[i])) {
				continue;
			}

			bud = UnitsConfig.Instance.GetUnitData(mapData.Units[i]);
			if (bud != null && !bud.PrefabName.Equals(string.Empty)) {
				bub = LoadUnitResource<BaseUnitBehaviour>(string.Format("{0}/{1}", GameConstants.Paths.UNIT_RESOURCES, bud.PrefabName));
				_enemyUnitsGraphicsResources.Add(mapData.Units[i], bub);
			} else {
				Debug.LogError("Can't load unit graphics: " + mapData.Units[i]);
			}
		}
	}

	private void LoadItemsResources(Dictionary<EItemKey, GameObject[]> resourcesDic, EUnitKey unitKey, UnitModelView unitModelView, EItemKey weaponRKey, EItemKey weaponLKey, EItemKey armorKey) {
		if (unitModelView != null) {
			GameObject rhWeaponResource = null;
			GameObject lhWeaponResource = null;
			GameObject headArmorResource = null;
			GameObject bodyArmorResource = null;

			unitModelView.SetWeaponType(weaponRKey, weaponLKey);

			//right hand weapon
			if (!resourcesDic.ContainsKey(weaponRKey)) {
				GameObject[] weaponResources = new GameObject[1];
				weaponResources[0] = Resources.Load(string.Format("{0}/{1}", GameConstants.Paths.ITEM_RESOURCES, GetItemResourcePath(weaponRKey))) as GameObject;
				resourcesDic.Add(weaponRKey, weaponResources);

				rhWeaponResource = weaponResources[0];
			} else if(weaponRKey != EItemKey.None) {
				rhWeaponResource = resourcesDic[weaponRKey][0];
			}

			//left hand weapon
			if (!resourcesDic.ContainsKey(weaponLKey)) {
				GameObject[] weaponResources = new GameObject[1];
				weaponResources[0] = Resources.Load(string.Format("{0}/{1}", GameConstants.Paths.ITEM_RESOURCES, GetItemResourcePath(weaponLKey))) as GameObject;
				resourcesDic.Add(weaponLKey, weaponResources);

				lhWeaponResource = weaponResources[0];
			} else if (weaponLKey != EItemKey.None) {
				lhWeaponResource = resourcesDic[weaponLKey][0];
			}

			//armor
			if (armorKey == EItemKey.None) {
				//Debug.LogWarning(string.Format("No armor set for {0} unit", unitKey));
			} else if (!resourcesDic.ContainsKey(armorKey)) {
				string armorResourcePath = GetItemResourcePath(armorKey);

				GameObject[] armorResources = new GameObject[2];
				armorResources[0] = Resources.Load(string.Format("{0}/{1}_head", GameConstants.Paths.ITEM_RESOURCES, armorResourcePath)) as GameObject;
				armorResources[1] = Resources.Load(string.Format("{0}/{1}_body", GameConstants.Paths.ITEM_RESOURCES, armorResourcePath)) as GameObject;
				resourcesDic.Add(armorKey, armorResources);

				headArmorResource = armorResources[0];
				bodyArmorResource = armorResources[1];
			} else {
				headArmorResource = resourcesDic[armorKey][0];
				bodyArmorResource = resourcesDic[armorKey][1];
			}

			unitModelView.SetupGraphics(rhWeaponResource, lhWeaponResource, headArmorResource, bodyArmorResource);
		}
	}

	private void DestroyInstances(bool fullUnload) {
		//destroy ally units
		if (fullUnload && _allyUnits != null) {
			for (int i = 0; i < _allyUnits.Length; i++) {
				if (_allyUnits[i] != null) {
					GameObject.Destroy(_allyUnits[i].gameObject);
				}
			}
			_allyUnits = null;
		}

		//destroy enemy units
		if (_enemyUnits != null) {
			for (int i = 0; i < _enemyUnits.Length; i++) {
				if (_enemyUnits[i] != null) {
					GameObject.Destroy(_enemyUnits[i].gameObject);
				}
			}
			_enemyUnits = null;
		}

		if (_backgroundResource != null) {
			if (FightManager.SceneInstance != null && FightManager.SceneInstance.UI != null && FightManager.SceneInstance.UI.ImgMapBackground != null) {
				FightManager.SceneInstance.UI.ImgMapBackground.sprite = null;
			}
		}
	}

	private void UnloadResources(bool fullUnload) {
		if (fullUnload) {
			_allyUnitsGraphicsResources.Clear();
			_allyItemsGraphicsResources.Clear();
			_unitUIResource = null;
		}
		_enemyUnitsGraphicsResources.Clear();
		_enemyItemsGraphicsResources.Clear();

		if (_backgroundResource != null) {
			UIResourcesManager.Instance.FreeResource(_backgroundResource);
			_backgroundResource = null;
		}

		Resources.UnloadUnusedAssets();
	}

	private T LoadUnitResource<T>(string path) where T : MonoBehaviour {
		GameObject go = Resources.Load(path) as GameObject;
		return go.GetComponent<T>();
	}
	#endregion

	#region auxiliary
	private string GetItemResourcePath(EItemKey itemKey) {
		BaseItem itemData = ItemsConfig.Instance.GetItem(itemKey);
		if (itemData != null && !itemData.PrefabName.Equals(string.Empty)) {
			return itemData.PrefabName;
		}

		return itemKey.ToString();
	}
	#endregion
}

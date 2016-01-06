using System;
using UnityEngine;
using UnityEngine.UI;

public class UICity : MonoBehaviour {
	#region local scene instance
	private static UICity _sceneInstance = null;
	public static UICity SceneInstance {
		get { return _sceneInstance; }
	}
	#endregion

	[SerializeField]
	private UICityBuildingIcon _cbiTownHall;
	[SerializeField]
	private UICityBuildingIcon _cbiFort;
	[SerializeField]
	private UICityBuildingIcon _cbiBarracks;
	[SerializeField]
	private UICityBuildingIcon _cbiMarket;
	[SerializeField]
	private UICityBuildingIcon _cbiWarehouse;
	[SerializeField]
	private UICityBuildingIcon _cbiHeroesHall;

	[SerializeField]
	private Button _btnBMMissions;
	[SerializeField]
	private Button _btnBMCrusade;
	[SerializeField]
	private Button _btnBMRaids;

	[SerializeField]
	private GameObject _goMenuRight;
	[SerializeField]
	private Button _btnRightMenuToggle;
	[SerializeField]
	private Button _btnRMHeroesHall;
	[SerializeField]
	private Button _btnRMBarracks;
	[SerializeField]
	private Button _btnRMInventory;
	[SerializeField]
	private Button _btnRMQuests;

	[SerializeField]
	private UICityBuildingPopup _buildingPopup;
	public UICityBuildingPopup BuildingPopup {
		get { return _buildingPopup; }
	}

	public void Awake() {
		_sceneInstance = this;

		if (!Global.IsInitialized) {
			Global.Instance.Initialize();
		}

		EventsAggregator.City.AddListener<ECityBuildingKey>(ECityEvent.ConstructionEnd, OnBuildingConstructionComplete);
	}

	public void Start() {
		//TODO: UI scale

		_btnBMMissions.onClick.AddListener(OnMissionsClick);
		_btnBMCrusade.onClick.AddListener(OnCrusadeClick);
		_btnBMRaids.onClick.AddListener(OnRaidsClick);

		_btnRightMenuToggle.onClick.AddListener(ToggleRightMenu);
		_btnRMHeroesHall.onClick.AddListener(OnRMHeroesHallClick);
		_btnRMBarracks.onClick.AddListener(OnRMBarracksClick);
		_btnRMInventory.onClick.AddListener(OnInventoryClick);
		_btnRMQuests.onClick.AddListener(OnQuestsClick);

		_buildingPopup.Hide();

		ECityBuildingKey[] buildingKeys = Enum.GetValues(typeof(ECityBuildingKey)) as ECityBuildingKey[];
		for (int i = 0; i < buildingKeys.Length; i++) {
			UpdateBuildingImage(buildingKeys[i], true);
		}
	}

	public void OnDestroy() {
		_sceneInstance = null;

		EventsAggregator.City.RemoveListener<ECityBuildingKey>(ECityEvent.ConstructionEnd, OnBuildingConstructionComplete);
	}

	private void UpdateBuildingImage(ECityBuildingKey buildingKey, bool isFirstLoad) {
		if (buildingKey == ECityBuildingKey.Idle) {
			return;
		}

		CBConstructionRequirement cr = CityConfig.Instance.GetBuildingData(buildingKey).GetConstructionRequirements(Global.Instance.Player.City.GetBuilding(buildingKey).Level);

		if (cr != null) {
			Sprite spriteResource = UIResourcesManager.Instance.GetResource<Sprite>(string.Format("{0}/{1}", GameConstants.Paths.UI_CITY_BUILDINGS_RESOURCES, cr.IconPath));
			UICityBuildingIcon buildingIcon = null;

			if (spriteResource != null) {
				switch (buildingKey) {
					case ECityBuildingKey.TownHall:
						buildingIcon = _cbiTownHall;
						break;
					case ECityBuildingKey.Barracks:
						buildingIcon = _cbiBarracks;
						break;
					case ECityBuildingKey.Fort:
						buildingIcon = _cbiFort;
						break;
					case ECityBuildingKey.HeroesHall:
						buildingIcon = _cbiHeroesHall;
						break;
					case ECityBuildingKey.Market:
						buildingIcon = _cbiMarket;
						break;
					case ECityBuildingKey.Warehouse:
						buildingIcon = _cbiWarehouse;
						break;
				}

				if (buildingIcon != null) {
					if (!isFirstLoad && buildingIcon.ImgBuilding.sprite != null) {
						Sprite s = buildingIcon.ImgBuilding.sprite;
						buildingIcon.ImgBuilding.sprite = null;
						UIResourcesManager.Instance.FreeResource(s);
					}

					buildingIcon.ImgBuilding.sprite = spriteResource;
				}
			}
		}
	}

	#region button listeners
	//bottom menu
	private void OnMissionsClick() {
		//TODO: load last planet's scene
		Application.LoadLevel("Planet1");
	}

	private void OnCrusadeClick() {
		//TODO: show crusade
	}

	private void OnRaidsClick() {
		//TODO: show raids
	}

	//right menu
	private void ToggleRightMenu() {
		_goMenuRight.SetActive(!_goMenuRight.activeInHierarchy);
	}

	private void OnInventoryClick() {
		//TODO: show player inventory
	}

	private void OnQuestsClick() {
		//TODO: show quests
	}

	private void OnRMHeroesHallClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowHeroesList>(EUIWindowKey.HeroesList).Show();
	}

	private void OnRMBarracksClick() {
		//TODO: show barracks window
	}
	#endregion

	#region listeners
	private void OnBuildingConstructionComplete(ECityBuildingKey buildingKey) {
		UpdateBuildingImage(buildingKey, false);
	}
	#endregion
}

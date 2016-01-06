using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowBuildingUpgrade : UIWindow {
	[SerializeField]
	private Text _lblCaption;
	[SerializeField]
	private Text _lblUpgrade;

	[SerializeField]
	private Text _lblFuelCost;
	[SerializeField]
	private Text _lblMineralsCost;
	[SerializeField]
	private Text _lblCreditsCost;
	[SerializeField]
	private Text _lblTimeCost;

	[SerializeField]
	private Button _btnUpgrade;
	[SerializeField]
	private Button _btnBack;

	[SerializeField]
	private Image _imgBuilding;

	private WaitForSeconds _wfs = null;
	private ECityBuildingKey _buildingKey = ECityBuildingKey.Idle;
	private int _buildngLevel = 1;

	public void Awake() {
		EventsAggregator.City.AddListener<ECityBuildingKey>(ECityEvent.ConstructionEnd, ConstructionComplete);
	}

	public void Start() {
		//TODO: setup language labels

		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnUpgrade.onClick.AddListener(OnBtnUpgradeClick);
	}

	public void OnDestroy() {
		StopAllCoroutines();

		_imgBuilding.sprite = null;

		if (CityConfig.Instance != null) {
			UIResourcesManager.Instance.FreeResource(string.Format("{0}/{1}", GameConstants.Paths.UI_CITY_BUILDINGS_RESOURCES, CityConfig.Instance.GetBuildingData(_buildingKey).GetConstructionRequirements(_buildngLevel).IconPath));
		}
		
		_buildingKey = ECityBuildingKey.Idle;

		EventsAggregator.City.RemoveListener<ECityBuildingKey>(ECityEvent.ConstructionEnd, ConstructionComplete);
	}

	public void Show(ECityBuildingKey buildingKey) {
		Show();
		Setup(buildingKey);
	}

	public void Setup(ECityBuildingKey buildingKey) {
		StopAllCoroutines();

		if (_imgBuilding.sprite != null) {
			_imgBuilding.sprite = null;
			UIResourcesManager.Instance.FreeResource(string.Format("{0}/{1}", GameConstants.Paths.UI_CITY_BUILDINGS_RESOURCES, CityConfig.Instance.GetBuildingData(_buildingKey).GetConstructionRequirements(_buildngLevel).IconPath));
		}

		_imgBuilding.enabled = false;
		_buildingKey = buildingKey;
		
		bool canUpgrade = false;
		CBBaseData buildingData = CityConfig.Instance.GetBuildingData(_buildingKey);
		if (buildingData != null) {
			CityBuildingInfo playerBuildingInfo = Global.Instance.Player.City.GetBuilding(_buildingKey);
			if (playerBuildingInfo != null) {
				_buildngLevel = playerBuildingInfo.Level;

				//setup image
				Sprite sprResource = UIResourcesManager.Instance.GetResource<Sprite>(string.Format("{0}/{1}", GameConstants.Paths.UI_CITY_BUILDINGS_RESOURCES, buildingData.GetConstructionRequirements(_buildngLevel).IconPath));
				if (sprResource != null) {
					_imgBuilding.sprite = sprResource;
					_imgBuilding.enabled = true;
				}

				if (playerBuildingInfo.IsLevelMaxed) {
					//TODO: set correct text
					_lblCaption.text = "Building has maximum level";

					_lblFuelCost.text = "-";
					_lblMineralsCost.text = "-";
					_lblCreditsCost.text = "-";
					_lblTimeCost.text = "-";
				} else {
					if (playerBuildingInfo.IsUnderCoustruction) {
						StartCoroutine(UpdateConstriuctionTime(playerBuildingInfo.ConstructionCompletionTimestamp - Utils.UnixTimestamp));

						_lblFuelCost.text = "-";
						_lblMineralsCost.text = "-";
						_lblCreditsCost.text = "-";
						_lblTimeCost.text = "-";
					} else {
						_lblCaption.text = string.Format("Ready to upgrade {0} to {1} lvl?", GetBuildingName(), (_buildngLevel + 1));

						CBConstructionRequirement nextLevelRequirenemts = buildingData.GetConstructionRequirements(_buildngLevel + 1);
						_lblFuelCost.text = nextLevelRequirenemts.CostFuel.ToString();
						_lblMineralsCost.text = nextLevelRequirenemts.CostMinerals.ToString();
						_lblCreditsCost.text = nextLevelRequirenemts.CostCredits.ToString();
						_lblTimeCost.text = nextLevelRequirenemts.BuildTime == 0 ? "instant" : Utils.FormatTime(nextLevelRequirenemts.BuildTime);	//TODO: setup correct "instant" text

						canUpgrade = true;

						if (Global.Instance.Player.Resources.Fuel < nextLevelRequirenemts.CostFuel) {
							_lblFuelCost.color = Color.red;
							canUpgrade = false;
						} else {
							_lblFuelCost.color = Color.white;
						}
						if (Global.Instance.Player.Resources.Minerals < nextLevelRequirenemts.CostMinerals) {
							_lblMineralsCost.color = Color.red;
							canUpgrade = false;
						} else {
							_lblMineralsCost.color = Color.white;
						}
						if (Global.Instance.Player.Resources.Credits < nextLevelRequirenemts.CostCredits) {
							_lblCreditsCost.color = Color.red;
							canUpgrade = false;
						} else {
							_lblCreditsCost.color = Color.white;
						}
					}
				}
			}
		}
		_btnUpgrade.interactable = canUpgrade;
	}

	private IEnumerator UpdateConstriuctionTime(int timeLeft) {
		_wfs = new WaitForSeconds(1f);

		while(timeLeft >= 0) {
			_lblCaption.text = string.Format("Upgrading: {0}", Utils.FormatTime(timeLeft));
			yield return _wfs;
			timeLeft--;
		}

		_wfs = null;
	}

	#region listeners
	private void OnBtnUpgradeClick() {
		Global.Instance.Player.City.StartConstruction(_buildingKey);
		Setup(_buildingKey);
	}

	private void OnBtnBackClick() {
		Hide();
	}

	private void ConstructionComplete(ECityBuildingKey buildingKey) {
		if (buildingKey == _buildingKey) {
			Setup(buildingKey);
		}
	}
	#endregion

	//WARNING! temp!
	private string GetBuildingName() {
		switch (_buildingKey) {
			case ECityBuildingKey.TownHall:
				return "Main";
			case ECityBuildingKey.Barracks:
				return "Barracks";
			case ECityBuildingKey.Fort:
				return "Fort";
			case ECityBuildingKey.HeroesHall:
				return "Heroes Hall";
			case ECityBuildingKey.Market:
				return "Merchant";
			case ECityBuildingKey.Warehouse:
				return "Warehouse";
		}

		return string.Empty;
	}
}

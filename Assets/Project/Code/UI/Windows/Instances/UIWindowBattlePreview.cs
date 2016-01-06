using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIWindowBattlePreview : UIWindow {
	[SerializeField]
	private Text _txtTitleCaption;
	//название босса

	[SerializeField]
	private Text _txtAttemptsCaption;
	[SerializeField]
	private Text _txtAttemptsAmount;
	//количество оставшихся попыток

	[SerializeField]
	private Text _txtFuelCaption;
	[SerializeField]
	private Text _txtFuelAmount;
	//необходимое количество топлива

	[SerializeField]
	private Text _txtEnemiesCaption;
	[SerializeField]
	private Text _txtLootCaption;
	//Лут

	[SerializeField]
	private UIBattlePreviewUnitInfo _iconEnemy;
    [SerializeField]
    private Transform _enemyCard; 

	[SerializeField]
	private float _offsetImageEnemy = 40f;

	[SerializeField]
	private Image _imgLoot;
	[SerializeField]
	private float _offsetImageLoot = 20f;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnPlay;

	private EPlanetKey _planetKey = EPlanetKey.None;
	private EMissionKey _missionKey = EMissionKey.None;

	private EUnitKey[] _enemies = null;
	private UIBattlePreviewUnitInfo[] _enemyIcons = null;

	private EItemKey[] _lootItems = null;
	private Image[] _lootItemImages = null;

	public void Awake() {
		AddDisplayAction(EUIWindowDisplayAction.PostHide, OnWindowHide);
	}

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnPlay.onClick.AddListener(OnBtnPlayClick);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	#region setup
	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		_planetKey = planetKey;
		_missionKey = missionKey;

		MissionData md = MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey);
		if (md != null) {
			//TODO: setup title
			SetupAttempts(md);
			SetupFuel(md);
			SetupEnemies(md);
			SetupLoot(md);

		}
	}

	private void SetupAttempts(MissionData md) {
		int attemptsUsed = Global.Instance.Player.StoryProgress.GetMissionAttemptsUsed(_planetKey, _missionKey);
		_txtAttemptsAmount.text = string.Format("{0}/{1}", md.AttemptsDaily - attemptsUsed, md.AttemptsDaily);
		if (attemptsUsed >= md.AttemptsDaily) {
			_btnPlay.interactable = false;
		}
	}

	private void SetupFuel(MissionData md) {
		_txtFuelAmount.text = md.FuelWinCost.ToString();
		if (Global.Instance.Player.Resources.Fuel < md.FuelWinCost) {
			_btnPlay.interactable = false;
			_txtFuelAmount.color = Color.red;
		} else {
			_txtFuelAmount.color = Color.white;
		}
	}

	private void SetupEnemies(MissionData md) {
		float enemyImageWidth = _iconEnemy.UnitBG.transform.GetComponent<RectTransform>().rect.width;

		List<EUnitKey> unitKeys = new List<EUnitKey>();
		MissionMapData mmd = null;
		for (int i = 0; i < md.MapsCount; i++) {
			mmd = md.GetMap(i);
			for (int j = 0; j < mmd.Units.Length; j++) {
				if (unitKeys.IndexOf(mmd.Units[j]) == -1) {
					unitKeys.Add(mmd.Units[j]);
				}
			}
		}
        EUnitKey firstEnemy = unitKeys[0];
        UIHeroCard enemyCard = _enemyCard.GetComponent<UIHeroCard>();
        enemyCard.UnitKey = firstEnemy;

        //_enemies = unitKeys.ToArray();
        //_enemyIcons = new UIBattlePreviewUnitInfo[_enemies.Length];
        //_enemyIcons[0] = _iconEnemy;
        //for (int i = 0; i < _enemies.Length; i++)
        //{
        //    if (i > 0)
        //    {
        //        _enemyIcons[i] = (GameObject.Instantiate(_iconEnemy.gameObject) as GameObject).GetComponent<UIBattlePreviewUnitInfo>();
        //        _enemyIcons[i].transform.SetParent(_iconEnemy.transform.parent, false);
        //        _enemyIcons[i].UnitBG.rectTransform.anchoredPosition = _iconEnemy.UnitBG.rectTransform.anchoredPosition + new Vector2(i * (enemyImageWidth + _offsetImageEnemy), 0f);
        //    }
        //    _enemyIcons[i].Setup(_enemies[i]);
        //}
	}

	private void SetupLoot(MissionData md) {
		float lootImageWidth = _imgLoot.transform.GetChild(0).GetComponent<RectTransform>().rect.width;

		ArrayRO<ItemDropChance> loot = MissionsConfig.Instance.GetPlanet(_planetKey).GetMission(_missionKey).RewardItems;
		if (loot.Length == 0) {
			_imgLoot.gameObject.SetActive(false);
		} else {
			_lootItems = new EItemKey[loot.Length];
			_lootItemImages = new Image[loot.Length];
			_lootItemImages[0] = _imgLoot;

			for (int i = 0; i < loot.Length; i++) {
				_lootItems[i] = loot[i].ItemKey;

				if (i > 0) {
					_lootItemImages[i] = (GameObject.Instantiate(_imgLoot.gameObject) as GameObject).GetComponent<Image>();
					_lootItemImages[i].transform.SetParent(_imgLoot.transform.parent, false);
					_lootItemImages[i].rectTransform.anchoredPosition = _imgLoot.rectTransform.anchoredPosition + new Vector2(i * (lootImageWidth + _offsetImageLoot), 0f);
				}

				Image lootIcon = _lootItemImages[i];
				Sprite lootIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetLootIconResourcePath(loot[i].ItemKey));
				if (lootIconResource != null) {
					lootIcon.sprite = lootIconResource;
				}
			}
		}
	}
	#endregion

	#region listeners
	private void OnBtnPlayClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowBattleSetup>(EUIWindowKey.BattleSetup).Show(_planetKey, _missionKey);
	}

	private void OnBtnBackClick() {
		Hide();
	}

	private void OnWindowHide(UIWindow window) {
		_planetKey = EPlanetKey.None;
		_missionKey = EMissionKey.None;

		//clear enemies
		if (_enemies != null) {
			for (int i = 0; i < _enemies.Length; i++) {
				_enemyIcons[i].Clear();
				if (i > 0) {
					GameObject.Destroy(_enemyIcons[i].gameObject);
				}
			}
		}
		_enemies = null;
		_enemyIcons = null;

		//clear loot
		if (_lootItems != null) {
			for (int i = 0; i < _lootItems.Length; i++) {
				_lootItemImages[i].sprite = null;
				if (i > 0) {
					GameObject.Destroy(_lootItemImages[i].gameObject);
				}
				UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetLootIconResourcePath(_lootItems[i]));
			}
		}
		_lootItems = null;
		_lootItemImages = null;
	}
	#endregion
}

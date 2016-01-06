using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowBattleSetup : UIWindow {
	[SerializeField]
	private Text _txtPlayerLeadershipCaption;
	[SerializeField]
	private Text _txtPlayerLeadershipAmount;

    [SerializeField]
    private Transform _heroCard; 

    [SerializeField]
    private Text _txtHeroName;
    [SerializeField]
    private Text _txtLevel;
    [SerializeField]
    private Text _txtHealth;
    [SerializeField]
    private Text _txtDamage;
    [SerializeField]
    private Text _txtLeadership;
    [SerializeField]
    private Text _txtSkills;

    [SerializeField]
    private MultiImageButton _mbtnCurrrentSoldier;

    BaseSoldierData _currentSoldierData = null;

    //[SerializeField]
    //private float _offsetImageAvailableSoldiersX = 70f;
	//[SerializeField]
	//private float _offsetImageAvailableSoldiersY = 265f;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnPlay;
    [SerializeField]
    private Button _btnChangeCurrentSoldier;
    [SerializeField]
    private Button _btnDeleteCurrentSoldier;

	//private BaseSoldierData[] _availableSoldiers = null;
	//private int[] _hiredSoldiers = null;	//indexes in available units

	//private UIBattleSetupUnitInfo[] _availableSoldiersInfo = null;
	//private Button[] _hiredSoldiersButtons = null;   

	//private int _leadershipSpent = 0;

	private EPlanetKey _planetKey = EPlanetKey.None;
	private EMissionKey _missionKey = EMissionKey.None;

    UIUnitSlotManager _slotManager { get { return GetComponent<UIUnitSlotManager>(); }}

	public void Awake() {
		AddDisplayAction(EUIWindowDisplayAction.PostHide, OnWindowHide);

		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnPlay.onClick.AddListener(OnBtnPlayClick);
        _btnChangeCurrentSoldier.onClick.AddListener(OnBtnChangeCurrentSoldier);
        _btnDeleteCurrentSoldier.onClick.AddListener(OnBtnDeleteCurrentSoldier);

        _slotManager.UnitSlotIsSelected += new System.EventHandler(_slotManager_UnitSlotIsSelected);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	#region setup

	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		_planetKey = planetKey;
		_missionKey = missionKey;

        SetupCurrentHero();
		//SetupAvailableUnits();
		//SetupHiredUnits();

		UpdateLeadership();
		//UpdateSoldiersHireAvailability();
	}

    //private void SetupAvailableUnits() {
    //    float availableSoldierImageWidth = _availableSoldierInfo.Button.image.rectTransform.rect.width;

    //    EUnitKey[] units = Global.Instance.Player.City.AvailableUnits.ToArray();
    //    _availableSoldiers = new BaseSoldierData[units.Length];
    //    for (int i = 0; i < units.Length; i++) {
    //        _availableSoldiers[i] = UnitsConfig.Instance.GetSoldierData(units[i]);
    //    }

    //    _availableSoldiersInfo = new UIBattleSetupUnitInfo[_availableSoldiers.Length];
    //    _availableSoldiersInfo[0] = _availableSoldierInfo;
    //    for (int i = 0; i < _availableSoldiers.Length; i++) {
    //        if (i > 0) {
    //            _availableSoldiersInfo[i] = (GameObject.Instantiate(_availableSoldierInfo.gameObject) as GameObject).GetComponent<UIBattleSetupUnitInfo>();
    //            _availableSoldiersInfo[i].transform.SetParent(_availableSoldierInfo.transform.parent, false);
    //            _availableSoldiersInfo[i].gameObject.GetComponent<RectTransform>().anchoredPosition = _availableSoldierInfo.gameObject.GetComponent<RectTransform>().anchoredPosition + new Vector2(i * (availableSoldierImageWidth + _offsetImageAvailableSoldiersX), 0f);
    //        }
    //        int iTmp = i;	//some spike: without this array.Length is passed to listener
    //        _availableSoldiersInfo[i].Button.onClick.AddListener(() => { HireSoldier(iTmp); });
    //        _availableSoldiersInfo[i].LblLeadershipCost.text = _availableSoldiers[i].LeadershipCost.ToString();

    //        Image soldierIcon = _availableSoldiersInfo[i].Button.image;
    //        Sprite enemyIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(_availableSoldiers[i].IconName));
    //        if (enemyIconResource != null)
    //        {
    //            soldierIcon.sprite = enemyIconResource;
    //        }
    //    }
    //}

    void SetupCurrentHero()
    {
        BaseHero hero = Global.Instance.Player.Heroes.Current;
        if (_heroCard != null)
        {
            UIHeroCard heroCard = _heroCard.GetComponent<UIHeroCard>();
            heroCard.UnitKey = hero.Data.Key;
        }
        _txtHeroName.text = hero.Data.PrefabName;
        _txtLevel.text = hero.Level.ToString();
        _txtHealth.text = hero.HealthPoints.ToString();
        _txtDamage.text = hero.Damage.ToString();
        _txtLeadership.text = hero.Leadership.ToString();
        _txtSkills.text = hero.ActiveSkills.ActiveSkills.Count.ToString();
        _slotManager.SetHeroTemplate();

        //MultiImageButton multiButton = _imgCurrrentSoldier.GetComponent<MultiImageButton>()
        //_imgCurrrentSoldier.enabled = false;
    }

    //private void SetupHiredUnits()
    //{
    //    //Sprite heroIconBGResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitBGIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
    //    //if (heroIconBGResource != null)
    //    //{
    //    //    _imgHeroBG.sprite = heroIconBGResource;
    //    //    _imgHeroBG.enabled = true;
    //    //}
    //    //else
    //    //{
    //    //    _imgHeroBG.enabled = false;
    //    //}
    //    //Sprite heroIconFGResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
    //    //if (heroIconFGResource != null)
    //    //{
    //    //    _imgHeroFG.sprite = heroIconFGResource;
    //    //    _imgHeroFG.enabled = true;
    //    //}
    //    //else
    //    //{
    //    //    _imgHeroFG.enabled = false;
    //    //}

    //    float hiredSoldierImageWidth = _btnHiredSoldier.image.rectTransform.rect.width;

    //    _hiredSoldiers = new int[UnitsConfig.Instance.MaxUnitsHeroCanHire];
    //    _hiredSoldiersButtons = new Button[_hiredSoldiers.Length];
    //    _hiredSoldiersButtons[0] = _btnHiredSoldier;
    //    for (int i = 0; i < _hiredSoldiers.Length; i++)
    //    {
    //        _hiredSoldiers[i] = -1;

    //        if (i > 0)
    //        {
    //            _hiredSoldiersButtons[i] = (GameObject.Instantiate(_btnHiredSoldier.gameObject) as GameObject).GetComponent<Button>();
    //            _hiredSoldiersButtons[i].transform.SetParent(_btnHiredSoldier.transform.parent, false);
    //            _hiredSoldiersButtons[i].gameObject.GetComponent<RectTransform>().anchoredPosition 
    //                = _btnHiredSoldier.gameObject.GetComponent<RectTransform>().anchoredPosition + new Vector2(i * (hiredSoldierImageWidth + 40), 0f);
    //        }
    //        int iTmp = i;	//some spike: without this array.Length is passed to listener
    //        _hiredSoldiersButtons[i].onClick.AddListener(() => { DismissSoldier(iTmp); });

    //        if (_hiredSoldiers[i] >= 0)
    //        {
    //            _hiredSoldiersButtons[i].image.sprite = _availableSoldiersInfo[_hiredSoldiers[i]].Button.image.sprite;
    //        }
    //        else
    //        {
    //            _hiredSoldiersButtons[i].image.enabled = false;
    //        }
    //    }
    //}

    void _slotManager_UnitSlotIsSelected(object sender, System.EventArgs e)
    {
        SetSelectedUnit((BaseSoldierData)sender);
    }

    void SetSelectedUnit(BaseSoldierData currentSoldierData)
    {
        if (_currentSoldierData != null)
        {
            UIResourcesManager.Instance.FreeResource(
                string.Format("{0}/{1}", GameConstants.Paths.UI_WINDOWS_PREFAB_RESOURCES, "Unit_card"));
        }
        Rect rectImage = _mbtnCurrrentSoldier.GetComponent<RectTransform>().rect;
        if (rectImage.width == 0 || rectImage.height == 0)
            return;

        _currentSoldierData = currentSoldierData;
        if (_currentSoldierData != null)
        {
            GameObject cardResource = UIResourcesManager.Instance.GetResource<GameObject>(
                string.Format("{0}/{1}", GameConstants.Paths.UI_WINDOWS_PREFAB_RESOURCES, "Unit_card"));
            if (cardResource == null)
                return;

            GameObject cardUnitData = GameObject.Instantiate(cardResource) as GameObject;
            UIUnitCard unitCard = cardUnitData.GetComponent<UIUnitCard>();
            unitCard.UnitKey = _currentSoldierData.Key;
            cardUnitData.transform.SetParent(_mbtnCurrrentSoldier.transform, false);

            RectTransform rectCard = cardUnitData.GetComponent<RectTransform>();
            rectCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectImage.width);
            rectCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectImage.height);
            rectCard.anchoredPosition = new Vector2(rectImage.width / 2, rectImage.height / 2);
            _mbtnCurrrentSoldier.AddChildImages(cardUnitData);
        }
        else
        {
            for (int i = _mbtnCurrrentSoldier.transform.childCount; i > 0; i--)
                GameObject.Destroy(_mbtnCurrrentSoldier.transform.GetChild(i - 1).gameObject);
        }
        UpdateLeadership();
    }

    void OnBtnChangeCurrentSoldier()
    {
        _slotManager.ChangeCurrentUnit();
        UpdateLeadership();
    }

    void OnBtnDeleteCurrentSoldier()
    {
        _slotManager.DeleteCurrentUnit();
        SetSelectedUnit(null);
        UpdateLeadership();
    }

	#endregion

	#region graphics dynamical update

	private void UpdateLeadership() {
		_txtPlayerLeadershipAmount.text = (Global.Instance.Player.Heroes.Current.Leadership - _slotManager.LeaderShipCostSum).ToString();
	}

    //private void UpdateSoldiersHireAvailability() {
    //    int totalLeadershop = Global.Instance.Player.Heroes.Current.Leadership;
    //    for (int i = 0; i < _availableSoldiersInfo.Length; i++) {
    //        _availableSoldiersInfo[i].LblLeadershipCost.color = _leadershipSpent + _availableSoldiers[i].LeadershipCost > totalLeadershop ? Color.red : Color.white;
    //    }
    //}

	#endregion

	#region hire/dismiss
    //private void HireSoldier(int unitIndex) {
    //    if (_leadershipSpent + _availableSoldiers[unitIndex].LeadershipCost > Global.Instance.Player.Heroes.Current.Leadership)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < _hiredSoldiers.Length; i++)
    //    {
    //        if (_hiredSoldiers[i] < 0)
    //        {
    //            _hiredSoldiers[i] = unitIndex;
    //            _hiredSoldiersButtons[i].image.sprite = _availableSoldiersInfo[unitIndex].Button.image.sprite;
    //            _hiredSoldiersButtons[i].image.enabled = true;
    //            break;
    //        }
    //    }

    //    UpdateLeadership();
    //    UpdateSoldiersHireAvailability();
    //}

    //private void DismissSoldier(int unitIndex) {
    //    for (int i = unitIndex; i < _hiredSoldiers.Length - 1; i++) {
    //        _hiredSoldiers[i] = _hiredSoldiers[i + 1];

    //        _hiredSoldiersButtons[i].image.sprite = _hiredSoldiersButtons[i + 1].image.sprite;
    //        _hiredSoldiersButtons[i].image.enabled = _hiredSoldiersButtons[i].image.sprite != null;
    //    }

    //    _hiredSoldiers[_hiredSoldiers.Length - 1] = -1;
    //    _hiredSoldiersButtons[_hiredSoldiersButtons.Length - 1].image.sprite = null;
    //    _hiredSoldiersButtons[_hiredSoldiersButtons.Length - 1].image.enabled = false;

    //    UpdateLeadership();
    //    UpdateSoldiersHireAvailability();
    //}

	#endregion

	#region button listeners
	private void OnBtnPlayClick() {
		Global.Instance.CurrentMission.PlanetKey = _planetKey;
		Global.Instance.CurrentMission.MissionKey = _missionKey;

        _slotManager.SaveHiredSoldiers();

		LoadingScreen.Instance.Show();
		LoadingScreen.Instance.SetProgress(0f);

		FightManager.Setup(EFightMode.Campaign, 
            MissionsConfig.Instance.GetPlanet(Global.Instance.CurrentMission.PlanetKey).GetMission(Global.Instance.CurrentMission.MissionKey));
		Application.LoadLevel(GameConstants.Scenes.FIGHT);
	}

	private void OnBtnBackClick() {
		Hide();
	}

	private void OnWindowHide(UIWindow window) {
		_planetKey = EPlanetKey.None;
		_missionKey = EMissionKey.None;

		//hero
        //if (_imgHero.sprite != null) {
        //    _imgHero.sprite = null;
        //    UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
        //}
        //TODO: free resources and clear data
        _slotManager.Clear();
        SetSelectedUnit(null);
	}
	#endregion
}

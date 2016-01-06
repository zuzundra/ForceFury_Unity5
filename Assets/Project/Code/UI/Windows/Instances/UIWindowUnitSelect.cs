using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIWindowUnitSelect : UIWindow
{
    public event EventHandler UnitIsSelected;
    public event EventHandler UnitSelectIsHided;

    [SerializeField]
    private Button _btnCancel;


    [SerializeField]
    private UIUnitSelectInfo[] _availableUnits = null;
    private ArrayRO<UIUnitSelectInfo> _availableUnitsRO = null;
    public ArrayRO<UIUnitSelectInfo> AvailableUnitsRO
    {
        get
        {
            if (_availableUnitsRO == null)
            {
                _availableUnitsRO = new ArrayRO<UIUnitSelectInfo>(_availableUnits);
            }
            return _availableUnitsRO;
        }
    }

    [SerializeField]
    private Button _btnLeft;
    [SerializeField]
    private Button _btnRight;

    [SerializeField]
    private Button _btnHeroFilter;

    [SerializeField]
    private Image _heroImageReg;

    [SerializeField]
    private Button _btnAllFilter;
    [SerializeField]
    private Button[] _btnLeadershipFilters;

    BaseSoldierData _slotData = null;
    public BaseSoldierData SlotData
    {
        get
        {
            return _slotData;
        }
    }

    BaseSoldierData[] _soldiersData = null;
    int _pageIndex = 0;

    public void Awake()
    {
        _btnCancel.onClick.AddListener(OnBtnCancelClick);
        for (int i = 0; i < AvailableUnitsRO.Length; i++)
            AvailableUnitsRO[i].UnitIsConfirmed += new System.EventHandler(UIWindowUnitSelect_UnitIsConfirmed);
        _btnLeft.onClick.AddListener(OnBtnLeftClick);
        _btnRight.onClick.AddListener(OnBtnRightClick);

        _btnHeroFilter.onClick.AddListener(OnBtnHeroClick);
        _btnAllFilter.onClick.AddListener(OnBtnAllClick);
        for (int i = 0; i < _btnLeadershipFilters.Length; i++)
        {
            int iTmp = i;
            _btnLeadershipFilters[i].onClick.AddListener(() => { SetFilterUnits(iTmp); });
        }
    }

    public void Show(BaseSoldierData slotData)
    {
        _slotData = slotData;
        SetCurrentHero();
        SetAllAvailableUnits();
        Show();
    }

    void SetCurrentHero()
    {

        if (_btnHeroFilter.transform.childCount == 0)
            return;
        //Image heroImage = _btnHeroFilter.transform.GetChild(0).GetComponent<Image>();
        
        Image newheroImage = _heroImageReg.transform.GetComponent<Image>();
        Sprite heroIconFGResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
        if (heroIconFGResource != null)
        {
            
            //heroImage.sprite = heroIconFGResource;
            newheroImage.sprite = heroIconFGResource;
            _btnHeroFilter.image.enabled = true;
         //   _btnHeroImage.image.enabled = true;
        }
        else
        {
            // heroImage.sprite = null;
            newheroImage.sprite = null;
           // _btnHeroFilter.image.enabled = false;
          _heroImageReg.enabled = false;
        }
    }

    void SetAllAvailableUnits()
    {
        EUnitKey[] units = Global.Instance.Player.City.AvailableUnits.ToArray();
        BaseSoldierData[] soldiersData = new BaseSoldierData[units.Length];
        for (int i = 0; i < units.Length; i++)
            soldiersData[i] = UnitsConfig.Instance.GetSoldierData(units[i]);
        SetAvailableUnits(soldiersData);
    }

    void SetAvailableUnits(BaseSoldierData[] soldiersData)
    {
        _soldiersData = soldiersData;
        int pageIndex = 0;
        if (_slotData != null)
        {
            for (int i = 0; i < soldiersData.Length; i++)
            {
                if (soldiersData[i].Equals(_slotData))
                {
                    pageIndex = AvailableUnitsRO.Length > 0 ? (int)(i / AvailableUnitsRO.Length) : 0;
                    break;
                }
            }
        }
        ShowPageUnits(pageIndex);
    }

    void ShowPageUnits(int pageIndex)
    {
        _pageIndex = pageIndex;
        int startSoldierIndex = _pageIndex * AvailableUnitsRO.Length;
        if (startSoldierIndex > _soldiersData.Length - 1)
            startSoldierIndex = (int)((_soldiersData.Length - 1) / AvailableUnitsRO.Length) * AvailableUnitsRO.Length;
        int soldierCount = _soldiersData.Length > startSoldierIndex + AvailableUnitsRO.Length
            ? AvailableUnitsRO.Length : _soldiersData.Length - startSoldierIndex;

        for (int i = 0; i < AvailableUnitsRO.Length; i++)
        {
            AvailableUnitsRO[i].ClearData();
            if (i < soldierCount)
            {
                BaseSoldierData soldierData = _soldiersData[i + startSoldierIndex];
                AvailableUnitsRO[i].LoadSoldierData(soldierData, soldierData.Equals(_slotData));
            }
            else
            {
                AvailableUnitsRO[i].LoadSoldierData(null, false);
            }
        }
        ShowNavigationButtonEnabled(_btnLeft, _pageIndex > 0);
        ShowNavigationButtonEnabled(_btnRight, _soldiersData.Length > startSoldierIndex + AvailableUnitsRO.Length);
    }

    void ShowNavigationButtonEnabled(Button button, bool enabled)
    {
        button.enabled = enabled;
        button.image.CrossFadeColor(enabled ? button.colors.normalColor : button.colors.disabledColor,
            button.colors.fadeDuration, true, true);
    }

    void OnBtnLeftClick()
    {
        if (_pageIndex > 0)
            ShowPageUnits(_pageIndex - 1);
    }

    void OnBtnRightClick()
    {
        if (_pageIndex < (int)(_soldiersData.Length / AvailableUnitsRO.Length) - 1)
            ShowPageUnits(_pageIndex + 1);
    }

    void UIWindowUnitSelect_UnitIsConfirmed(object sender, System.EventArgs e)
    {
        if (UnitIsSelected != null)
            UnitIsSelected(((UIUnitSelectInfo)sender).UnitData, e);
        Hide();
    }

    void OnBtnHeroClick()
    {
        EUnitKey[] units = Global.Instance.Player.City.AvailableUnits.ToArray();
        List<BaseSoldierData> filtersData = new List<BaseSoldierData>();
        for (int i = 0; i < units.Length; i++)
        {
            BaseSoldierData filterData = UnitsConfig.Instance.GetSoldierData(units[i]);
            Debug.Log("Type " + filterData.Type + ", " + Global.Instance.Player.Heroes.Current.Data.Key);
            if (filterData.Type == Global.Instance.Player.Heroes.Current.Data.Key)
                filtersData.Add(filterData);
        }
        SetAvailableUnits(filtersData.ToArray());
    }

    void OnBtnAllClick()
    {
        SetAllAvailableUnits();
    }

    void SetFilterUnits(int filterIndex)
    {
        if (filterIndex > _btnLeadershipFilters.Length - 1)
            return;
        Button filterButton = _btnLeadershipFilters[filterIndex];
        string filterText = filterButton.transform.childCount > 0 ? filterButton.transform.GetChild(0).GetComponent<Text>().text : string.Empty;

        EUnitKey[] units = Global.Instance.Player.City.AvailableUnits.ToArray();
        List<BaseSoldierData> filtersData = new List<BaseSoldierData>();
        for (int i = 0; i < units.Length; i++)
        {
            int leadership;
            BaseSoldierData filterData = UnitsConfig.Instance.GetSoldierData(units[i]);
            if (filterIndex < _btnLeadershipFilters.Length - 1)
            {
                if (int.TryParse(filterText, out leadership) && filterData.LeadershipCost == leadership)
                {
                    filtersData.Add(filterData);
                }
            }
            else if (filterIndex > 0)
            {
                if (int.TryParse(filterText.Replace("+", string.Empty), out leadership) && filterData.LeadershipCost > leadership)
                {
                    filtersData.Add(filterData);
                }
            }
        }
        SetAvailableUnits(filtersData.ToArray());
    }

    void OnBtnCancelClick()
    {
        Hide();
    }

    private void OnWindowHide(UIWindow window)
    {
        if (_btnHeroFilter.transform.childCount != 0)
        {
            Image heroImage = _btnHeroFilter.transform.GetChild(0).GetComponent<Image>();
            if (heroImage.sprite != null)
            {
                heroImage.sprite = null;
                UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
            }
        }
        for (int i = 0; i < AvailableUnitsRO.Length; i++)
            AvailableUnitsRO[i].ClearData();

        _slotData = null;
        _soldiersData = null;
        _pageIndex = 0;
        if (UnitSelectIsHided != null)
            UnitSelectIsHided(this, new EventArgs());
    }
}

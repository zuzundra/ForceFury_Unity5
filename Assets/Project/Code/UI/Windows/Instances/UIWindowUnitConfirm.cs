using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIWindowUnitConfirm : UIWindow
{
    public event EventHandler UnitIsConfirmed;
    public event EventHandler ConfirmIsHided;

    [SerializeField]
    private Button _btnCancel;
    [SerializeField]
    private Button _btnOK;
    [SerializeField]
    private Image _imgUnit;

    [SerializeField]
    private Text _txtDamage;
    public Text TxtDamage
    {
        get { return _txtDamage; }
    }

    [SerializeField]
    private Text _txtHP;
    public Text TxtHP
    {
        get { return _txtHP; }
    }

    [SerializeField]
    private Text _txtInfo;
    public Text TxtInfo
    {
        get { return _txtInfo; }
    }

    BaseSoldierData _unitData = null;
    public BaseSoldierData UnitData
    {
        get
        {
            return _unitData;
        }
    }

    public void Awake()
    {
        AddDisplayAction(EUIWindowDisplayAction.PostHide, OnWindowHide);

        _btnCancel.onClick.AddListener(OnBtnCancelClick);
        _btnOK.onClick.AddListener(OnBtnOKClick);
        _imgUnit.enabled = false;
    }

    public void Show(BaseSoldierData unitData)
    {
        SetUnit(unitData);
        Show();
    }

    void SetUnit(BaseSoldierData unitData)
    {
        _unitData = unitData;
        if (_unitData == null)
            return;

        GameObject cardResource = UIResourcesManager.Instance.GetResource<GameObject>(
            string.Format("{0}/{1}", GameConstants.Paths.UI_WINDOWS_PREFAB_RESOURCES, "Unit_card"));
        if (cardResource == null)
            return;

        GameObject cardUnitData = GameObject.Instantiate(cardResource) as GameObject;
        UIUnitCard unitCard = cardUnitData.GetComponent<UIUnitCard>();
        unitCard.UnitKey = _unitData.Key;
        cardUnitData.transform.SetParent(transform, false);

        RectTransform rectCard = cardUnitData.GetComponent<RectTransform>();
        Rect rectBackground = gameObject.GetComponent<RectTransform>().rect;
        rectCard.anchoredPosition = new Vector2(rectBackground.width / 2, rectBackground.height / 2);
        rectCard.localScale *= _imgUnit.rectTransform.rect.width / rectCard.rect.width;

        _txtDamage.text = "Damage: " + _unitData.BaseDamage.ToString();
        _txtHP.text = "Health Points: " + _unitData.BaseHealthPoints.ToString();
        _txtInfo.text = "About Unit: " + _unitData.AboutInfo;
    }

    void OnBtnOKClick()
    {
        if (UnitIsConfirmed != null)
            UnitIsConfirmed(this, new EventArgs());
        Hide();
    }

    void OnBtnCancelClick()
    {
        Hide();
    }

	private void OnWindowHide(UIWindow window)
    {
        _unitData = null;
        if (ConfirmIsHided != null)
            ConfirmIsHided(this, new EventArgs());
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIHeroCard : MonoBehaviour
{
    [SerializeField]
    private EUnitKey _unitKey = EUnitKey.Idle;
    public EUnitKey UnitKey
    {
        get
        {
            return _unitKey;
        }
        set
        {
            _unitKey = value;
        }
    }

    [SerializeField]
    private Image _imgBG;

    [SerializeField]
    private Image _imgUnit;

    BaseHeroData _heroData = null;

    public void Update()
    {
        if (_unitKey == EUnitKey.Idle || _heroData != null)
            return;

        _heroData = UnitsConfig.Instance.GetHeroData(_unitKey);
        string bgPath = _heroData != null ? GameConstants.Paths.GetUnitBGIconResourcePath(_heroData.IconName) 
            : GameConstants.Paths.GetUnitBGIconResourcePath(_unitKey);
        _imgBG.sprite = UIResourcesManager.Instance.GetResource<Sprite>(bgPath);

        string iconPath = _heroData != null ? GameConstants.Paths.GetUnitIconResourcePath(_heroData.IconName) 
            : GameConstants.Paths.GetUnitIconResourcePath(_unitKey);
        _imgUnit.sprite = UIResourcesManager.Instance.GetResource<Sprite>(iconPath);
    }

    public void OnDestroy()
    {
        if (_heroData != null)
            UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(_heroData.IconName));
        _heroData = null;
    }
}
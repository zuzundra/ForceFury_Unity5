using UnityEngine;
using UnityEngine.UI;
using System;

public class UIUnitCard : MonoBehaviour
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
    private Image _imgUnit;

    [SerializeField]
    private Text _txtLeadership;

    [SerializeField]
    private Text _txtLevel;

    [SerializeField]
    private Text _txtDescription;

    [SerializeField]
    private Transform _panRank;

    [SerializeField]
    private int _rank = 1;

    BaseSoldierData _soldierData = null;

    public void Update()
    {
        if (_unitKey == EUnitKey.Idle || _soldierData != null)
            return;

        _soldierData = UnitsConfig.Instance.GetSoldierData(_unitKey);
        if (_soldierData == null)
            return;

        _imgUnit.sprite = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(_soldierData.IconName));
        if (_imgUnit.sprite == null)
            return;

        _txtLeadership.text = _soldierData.LeadershipCost.ToString();
       // _txtLevel.text = "lvl " + Global.Instance.Player.City.GetSoldierUpgradesInfo(_soldierData.Key).Level.ToString();
        _txtDescription.text = _soldierData.AboutInfo;

        Sprite starImage = UIResourcesManager.Instance.GetResource<Sprite>(
            string.Format("{0}/{1}", GameConstants.Paths.UI_WINDOWS_PREFAB_RESOURCES, "rank_mini"));
        if (starImage != null)
        {
            for (int i = 0; i < _rank && i <= 5; i++)
            {
                GameObject star = new GameObject("Star_" + (i + 1).ToString());
                star.AddComponent<Image>().sprite = GameObject.Instantiate(starImage) as Sprite;
                star.transform.SetParent(_panRank.transform, false);
            }
        }
    }

    public void OnDestroy()
    {
        if (_soldierData != null)
            UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(_soldierData.IconName));
        _soldierData = null;

        UIResourcesManager.Instance.FreeResource(string.Format("{0}/{1}", GameConstants.Paths.UI_WINDOWS_PREFAB_RESOURCES, "rank_mini"));
    }
}

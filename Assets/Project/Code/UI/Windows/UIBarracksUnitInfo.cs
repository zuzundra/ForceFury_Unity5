using UnityEngine;
using UnityEngine.UI;

public class UIBarracksUnitInfo : MonoBehaviour {
	[SerializeField]
	private Button _button;
	public Button Button {
		get { return _button; }
	}

	[SerializeField]
	private Image _imgUnitIcon;

	[SerializeField]
	private Text _lblLeadershipCost;
	[SerializeField]
	private Text _lblLevel;

	//TODO: unit upgrades level

	private BaseSoldierData _unitData = null;
	public BaseSoldierData UnitData {
		get { return _unitData; }
	}

	public void Awake() {
		enabled = false;
	}

	public void Setup(EUnitKey unitKey) {
		_unitData = UnitsConfig.Instance.GetSoldierData(unitKey);

		_lblLeadershipCost.text = _unitData.LeadershipCost.ToString();
		//TODO: set leved

		Sprite enemyIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(_unitData.IconName));
		if (enemyIconResource != null) {
			_imgUnitIcon.sprite = enemyIconResource;
		}
	}

	public void Clear() {
		_lblLeadershipCost.text = "0";
		_lblLevel.text = "1";

		if (_imgUnitIcon.sprite != null) {
			_imgUnitIcon.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(_unitData.IconName));
		}

		_unitData = null;
	}
}

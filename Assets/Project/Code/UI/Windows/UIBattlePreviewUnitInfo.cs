using UnityEngine;
using UnityEngine.UI;

public class UIBattlePreviewUnitInfo : MonoBehaviour {
	[SerializeField]
	private Image _unitBG;
	public Image UnitBG {
		get { return _unitBG; }
	}
	[SerializeField]
	private Image _unitFG;
	public Image UnitFG {
		get { return _unitFG; }
	}

	private EUnitKey _unitKey = EUnitKey.Idle;

	public void Setup(EUnitKey unitKey) {
		_unitKey = unitKey;

		Sprite enemyIconBGResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitBGIconResourcePath(unitKey));
		if (enemyIconBGResource != null) {
			_unitBG.sprite = enemyIconBGResource;
			_unitBG.enabled = true;
		} else {
			_unitBG.enabled = false;
		}
		Sprite enemyIconFGResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(unitKey));
		if (enemyIconFGResource != null) {
			_unitFG.sprite = enemyIconFGResource;
			_unitFG.enabled = true;
		} else {
			_unitFG.enabled = false;
		}
	}

	public void Clear() {
		if (_unitBG.sprite != null) {
			_unitBG.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitBGIconResourcePath(_unitKey));
		}
		if (_unitFG.sprite != null) {
			_unitFG.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(_unitKey));
		}

		_unitKey = EUnitKey.Idle;
	}
}

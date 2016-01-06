using UnityEngine;
using UnityEngine.UI;

public class UIBattleSetupUnitInfo : MonoBehaviour {
	[SerializeField]
	private Button _button;
	public Button Button {
		get { return _button; }
	}

	[SerializeField]
	private Text _lblLeadershipCost;
	public Text LblLeadershipCost {
		get { return _lblLeadershipCost; }
	}

	//TODO: unit upgrades level

	public void Awake() {
		enabled = false;
	}
}

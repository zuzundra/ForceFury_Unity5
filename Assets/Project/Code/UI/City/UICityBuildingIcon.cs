using UnityEngine;
using UnityEngine.UI;

public class UICityBuildingIcon : MonoBehaviour {
	[SerializeField]
	private Image _imgBuilding;
	public Image ImgBuilding {
		get { return _imgBuilding; }
	}
	[SerializeField]
	private Button _btnBuilding;
	public Button BtnBuilding {
		get { return _btnBuilding; }
	}

	[SerializeField]
	private Text _lblBuildingName;

	[SerializeField]
	private ECityBuildingKey _buildingKey = ECityBuildingKey.Idle;

	public void Start() {
		//TODO: setup building name

		_btnBuilding.onClick.AddListener(OnBtnBuildingClick);
	}

	private void OnBtnBuildingClick() {
		UICity.SceneInstance.BuildingPopup.Show(_buildingKey);
	}
}

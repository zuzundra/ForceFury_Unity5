using UnityEngine;
using UnityEngine.UI;

public class UIWindowSets : UIWindow {
	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnTrooper;
	[SerializeField]
	private Button _btnUnitUnlocked;

	public void Awake() {
		_btnUnitUnlocked.gameObject.SetActive(false);
	}

	public void Start() {
		_btnTrooper.gameObject.SetActive(UIWindowShop._trooperPurchased);

		_btnTrooper.onClick.AddListener(OnBtnTrooperClick);
		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnUnitUnlocked.onClick.AddListener(OnBtnUnitUnlockedClickClick);
	}

	#region listeners
	private void OnBtnTrooperClick() {
		if (UIWindowShop._trooperPurchased && Global.Instance.Player.City.GetBuilding(ECityBuildingKey.Barracks).Level < 2) {
			Global.Instance.Player.City.StartConstruction(ECityBuildingKey.Barracks);

			_btnTrooper.interactable = false;

			_btnUnitUnlocked.gameObject.SetActive(true);
		}
	}

	private void OnBtnBackClick() {
		Hide();
	}

	private void OnBtnUnitUnlockedClickClick() {
		_btnUnitUnlocked.gameObject.SetActive(false);
	}
	#endregion
}

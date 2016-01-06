using UnityEngine;
using UnityEngine.UI;

public class UIWindowShop : UIWindow {
	public static bool _trooperPurchased = false;

	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnTrooper;
	[SerializeField]
	private Button _btnUnitPurchased;

	public void Awake() {
		_btnUnitPurchased.gameObject.SetActive(false);
	}

	public void Start() {
		_btnTrooper.onClick.AddListener(OnBtnTrooperClick);
		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnUnitPurchased.onClick.AddListener(OnBtnUnitPurchasedClick);
	}

	#region listeners
	private void OnBtnTrooperClick() {
		if (!_trooperPurchased) {
			Global.Instance.Player.Resources.Minerals -= 10;
			_trooperPurchased = true;
			_btnUnitPurchased.gameObject.SetActive(true);
		}
	}

	private void OnBtnBackClick() {
		Hide();
	}

	private void OnBtnUnitPurchasedClick() {
		_btnUnitPurchased.gameObject.SetActive(false);
	}
	#endregion
}

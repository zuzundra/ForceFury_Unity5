using UnityEngine;
using UnityEngine.UI;

public class UIWindowUpgrades : UIWindow {
	[SerializeField]
	private Button _btnBack;

	[SerializeField]
	private Button _btnScout;
	[SerializeField]
	private Button _btnTrooper;

	[SerializeField]
	private UIUnitUpgrade _unitUpgrade;

	public void Start() {
		if (Global.Instance.Player.City.AvailableUnits.IndexOf(EUnitKey.Trooper) != -1) {
			_btnTrooper.gameObject.SetActive(true);
			_btnScout.image.rectTransform.anchoredPosition = new Vector2(-250f, 0f);
			_btnTrooper.image.rectTransform.anchoredPosition = new Vector2(250f, 0f);
		} else {
			_btnTrooper.gameObject.SetActive(false);
			_btnScout.image.rectTransform.anchoredPosition = new Vector2(0, 0f);
		}

		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnScout.onClick.AddListener(OnBtnScoutClick);
		_btnTrooper.onClick.AddListener(OnBtnTrooperClick);

		_unitUpgrade.Hide();
	}

	private void OnBtnScoutClick() {
		_unitUpgrade.Setup(EUnitKey.Scout, _btnScout.image.sprite);
		_unitUpgrade.Show();
	}

	private void OnBtnTrooperClick() {
		_unitUpgrade.Setup(EUnitKey.Trooper, _btnTrooper.image.sprite);
		_unitUpgrade.Show();
	}

	private void OnBtnBackClick() {
		Hide();
	}
}

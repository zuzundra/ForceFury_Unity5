using UnityEngine;
using UnityEngine.UI;

public class UIWindowPlanetOverlay : UIWindow {
	[SerializeField]
	private Button _btnBack;

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
	}

	private void OnBtnBackClick() {
		Application.LoadLevel("MainMenu");
	}
}

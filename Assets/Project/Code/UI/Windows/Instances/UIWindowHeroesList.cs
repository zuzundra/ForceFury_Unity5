using UnityEngine;
using UnityEngine.UI;

public class UIWindowHeroesList : UIWindow {
	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnHero;

	public void Start() {
		_btnHero.onClick.AddListener(OnBtnHeroClick);
		_btnBack.onClick.AddListener(OnBtnBackClick);
	}

	#region listeners
	private void OnBtnHeroClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowHeroInfo>(EUIWindowKey.HeroInfo).Show();
	}

	private void OnBtnBackClick() {
		Hide();
	}
	#endregion
}

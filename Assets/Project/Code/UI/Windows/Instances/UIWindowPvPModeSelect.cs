using UnityEngine;
using UnityEngine.UI;

public class UIWindowPvPModeSelect : UIWindow {
	[SerializeField]
	private Button _btnBack;
	[SerializeField]
	private Button _btnRatingGame;
	[SerializeField]
	private Button _btnPracticeGame;

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
		_btnRatingGame.onClick.AddListener(OnBtnRatingGameClick);
		_btnPracticeGame.onClick.AddListener(OnBtnPracticeGameClick);
	}

	#region listeners
	private void OnBtnBackClick() {
		Hide();
	}

	private void OnBtnRatingGameClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowPvPBattleSetup>(EUIWindowKey.PvPBattleSetup).Show(EPlanetKey.None, EMissionKey.None);
	}

	private void OnBtnPracticeGameClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowPvPBattleSetup>(EUIWindowKey.PvPBattleSetup).Show(EPlanetKey.None, EMissionKey.None);
	}
	#endregion
}

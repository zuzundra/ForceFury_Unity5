using UnityEngine;
using UnityEngine.UI;

public class UIWindowBattleDefeat : UIWindow {
	[SerializeField]
	private Text _lblExpAmount;

	[SerializeField]
	private Image _imgHeroBG;
	[SerializeField]
	private Image _imgHeroFG;

	[SerializeField]
	private Button _btnPlay;
	[SerializeField]
	private Button _btnReplay;

	private EPlanetKey _planetKey = EPlanetKey.None;
	private EMissionKey _missionKey = EMissionKey.None;

	public void Awake() {
		AddDisplayAction(EUIWindowDisplayAction.PostHide, OnWindowHide);

		_btnPlay.onClick.AddListener(OnBtnPlayClick);
		_btnReplay.onClick.AddListener(OnBtnReplayClick);
	}

	public void Show(EPlanetKey planetKey, EMissionKey missionKey) {
		Setup(planetKey, missionKey);
		Show();
	}

	#region setup
	public void Setup(EPlanetKey planetKey, EMissionKey missionKey) {
		_planetKey = planetKey;
		_missionKey = missionKey;

		MissionData md = MissionsConfig.Instance.GetPlanet(planetKey).GetMission(missionKey);
		if (md != null) {
			_lblExpAmount.text = string.Format("+ {0}", md.RewardExperienceWin);

			Sprite heroBackIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitBGIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
			if (heroBackIconResource != null) {
				_imgHeroBG.sprite = heroBackIconResource;
				_imgHeroBG.enabled = true;
			} else {
				_imgHeroBG.enabled = false;
			}
			Sprite heroIconResource = UIResourcesManager.Instance.GetResource<Sprite>(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
			if (heroIconResource != null) {
				_imgHeroFG.sprite = heroIconResource;
				_imgHeroFG.enabled = true;
			} else {
				_imgHeroFG.enabled = false;
			}
		}
	}
	#endregion

	#region listeners
	private void OnBtnPlayClick() {
		//TODO: load correct planet scene
		FightManager.SceneInstance.Clear();
		Application.LoadLevel("Planet1");
	}

	private void OnBtnReplayClick() {
		ScenesSwitcher.Instance.AddLevelLoadCallback(() => {
			UIWindowBattlePreview wbp = UIWindowsManager.Instance.GetWindow<UIWindowBattlePreview>(EUIWindowKey.BattlePreview);
			wbp.Show(_planetKey, _missionKey);
		}, 2);

		//TODO: load correct planet scene
		FightManager.SceneInstance.Clear();
		Application.LoadLevel("Planet1");
	}

	private void OnWindowHide(UIWindow window) {
		_planetKey = EPlanetKey.None;
		_missionKey = EMissionKey.None;

		//clear labels
		_lblExpAmount.text = "+ 0";

		//hero
		if (_imgHeroBG.sprite != null) {
			_imgHeroBG.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitBGIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
		}
		if (_imgHeroFG.sprite != null) {
			_imgHeroFG.sprite = null;
			UIResourcesManager.Instance.FreeResource(GameConstants.Paths.GetUnitIconResourcePath(Global.Instance.Player.Heroes.Current.Data.IconName));
		}
	}
	#endregion
}

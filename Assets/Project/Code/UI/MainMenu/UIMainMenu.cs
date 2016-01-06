using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {
	[SerializeField]
	private Button _btnCampaign;
	[SerializeField]
	private Button _btnArena;
	[SerializeField]
	private Button _btnSets;
	[SerializeField]
	private Button _btnShop;
	[SerializeField]
	private Button _btnHeroes;
	[SerializeField]
	private Button _btnSquad;

	[SerializeField]
	private Text _lblCredits;
	[SerializeField]
	private Text _lblMinerals;
	[SerializeField]
	private Text _lblFuel;

	//public void Awake() {
	//	float widthRatio = 1f * Screen.width / 1048;
	//	float heightRatio = 1f * Screen.height / 540;

	//	gameObject.GetComponent<CanvasScaler>().scaleFactor = Mathf.Min(widthRatio, heightRatio);
	//}

	public void Start() {
		Global.Instance.Initialize();

		_btnCampaign.onClick.AddListener(OnBtnCampaignClick);
		_btnArena.onClick.AddListener(OnBtnArenaClick);
		_btnSets.onClick.AddListener(OnBtnSetsClick);
		_btnShop.onClick.AddListener(OnBtnShopClick);
		_btnHeroes.onClick.AddListener(OnBtnHeroesClick);
		_btnSquad.onClick.AddListener(OnBtnSquadClick);

		EventsAggregator.Player.AddListener<int>(EPlayerEvent.CreditsUpdate, OnCreditsUpdate);
		EventsAggregator.Player.AddListener<int>(EPlayerEvent.MineralsUpdate, OnMineralsUpdate);
		EventsAggregator.Player.AddListener<int>(EPlayerEvent.FuelUpdate, OnFuelUpdate);

		OnCreditsUpdate(Global.Instance.Player.Resources.Credits);
		OnMineralsUpdate(Global.Instance.Player.Resources.Minerals);
		OnFuelUpdate(Global.Instance.Player.Resources.Fuel);
	}

	public void OnDestroy() {
		EventsAggregator.Player.RemoveListener<int>(EPlayerEvent.CreditsUpdate, OnCreditsUpdate);
		EventsAggregator.Player.RemoveListener<int>(EPlayerEvent.MineralsUpdate, OnMineralsUpdate);
		EventsAggregator.Player.RemoveListener<int>(EPlayerEvent.FuelUpdate, OnFuelUpdate);
	}

	#region listeners
	private void OnCreditsUpdate(int amount) {
		_lblCredits.text = amount.ToString();
	}

	private void OnMineralsUpdate(int amount) {
		_lblMinerals.text = amount.ToString();
	}

	private void OnFuelUpdate(int amount) {
		_lblFuel.text = amount.ToString();
	}

	private void OnBtnCampaignClick() {
		//TODO: load last planet's scene
		Application.LoadLevel("Missions");
	}

	private void OnBtnArenaClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowPvPModeSelect>(EUIWindowKey.PvPModeSelect).Show();
	}

	private void OnBtnSetsClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowSets>(EUIWindowKey.Sets).Show();
	}

	private void OnBtnShopClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowShop>(EUIWindowKey.Shop).Show();
	}

	private void OnBtnHeroesClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowHeroesList>(EUIWindowKey.HeroesList).Show();
	}

	private void OnBtnSquadClick() {
		UIWindowsManager.Instance.GetWindow<UIWindowUpgrades>(EUIWindowKey.Upgrades).Show();
	}
	#endregion
}

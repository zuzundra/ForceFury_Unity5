using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitsSelectGUI : MonoBehaviour {
	[SerializeField]
	private Text _txtLeadership;
	[SerializeField]
	private Text _txtHiredList;
	[SerializeField]
	private Text _txtUnitName;
	[SerializeField]
	private Text _txtUnitHealth;
	[SerializeField]
	private Text _txtUnitDamage;
	[SerializeField]
	private Text _txtUnitLeadershipCost;
	[SerializeField]
	private Button _btnNext;
	[SerializeField]
	private Button _btnPref;
	[SerializeField]
	private Button _btnHire;
	[SerializeField]
	private Button _btnFight;

	private EUnitKey[] _availableUnits = null;
	private List<EUnitKey> _hiredUnits = new List<EUnitKey>();

	private int _currentUnitIndex = 0;
	private BaseSoldierData _currentSoldierData = null;

	private int _maxUnitsPrice = 0;
	private int _currentUnitsPrice = 0;

	public void Start() {
		Global.Instance.Initialize();

		_availableUnits = Global.Instance.Player.City.AvailableUnits.ToArray();

		_btnNext.onClick.AddListener(NextUnit);
		_btnPref.onClick.AddListener(PrevUnit);
		_btnHire.onClick.AddListener(HireUnit);
		_btnFight.onClick.AddListener(StartFight);

		_maxUnitsPrice = Global.Instance.Player.Heroes.Current.Data.BaseLeadership;
		_txtLeadership.text = _maxUnitsPrice.ToString();

		SelectUnit();
	}

	private void NextUnit() {
		_currentUnitIndex++;
		if (_currentUnitIndex >= _availableUnits.Length) {
			_currentUnitIndex = 0;
		}

		SelectUnit();
	}

	private void PrevUnit() {
		_currentUnitIndex--;
		if (_currentUnitIndex < 0) {
			_currentUnitIndex = _availableUnits.Length - 1;
		}

		SelectUnit();
	}

	private void SelectUnit() {
		_currentSoldierData = UnitsConfig.Instance.GetSoldierData(_availableUnits[_currentUnitIndex]);

		_txtUnitName.text = _availableUnits[_currentUnitIndex].ToString();
		_txtUnitHealth.text = _currentSoldierData.BaseHealthPoints.ToString();
		_txtUnitDamage.text = _currentSoldierData.BaseDamage.ToString();
		_txtUnitLeadershipCost.text = _currentSoldierData.LeadershipCost.ToString();
	}

	private void HireUnit() {
		if (_currentUnitsPrice + _currentSoldierData.LeadershipCost > _maxUnitsPrice) {
			Debug.Log("Can't hire this unit: need more leadership");
			return;
		}
		if (_hiredUnits.Count >= 4) {
			Debug.Log("Max units amount hired");
			return;
		}

		_hiredUnits.Add(_availableUnits[_currentUnitIndex]);
		_currentUnitsPrice += _currentSoldierData.LeadershipCost;

		_txtHiredList.text = string.Empty;
		for (int i = 0; i < _hiredUnits.Count; i++) {
			if(i > 0) {
				_txtHiredList.text += "\n";
			}
			_txtHiredList.text += _hiredUnits[i].ToString();
		}
	}

	private void StartFight() {
		PlayerResources playerResources = Global.Instance.Player.Resources;
		MissionData md = MissionsConfig.Instance.GetPlanet(EPlanetKey.PlanetA).GetMission(EMissionKey.PlanetA_Test1);
		if (playerResources.Fuel < md.FuelWinCost || playerResources.Fuel < md.FuelLoseCost ||
			playerResources.Credits < md.CreditsWinCost || playerResources.Credits < md.CreditsLoseCost ||
			playerResources.Minerals < md.MineralsWinCost || playerResources.Minerals < md.MineralsLoseCost) {
				Debug.LogWarning("Not enough resources");
				return;
		}

		BaseSoldier[] soldiers = new BaseSoldier[_hiredUnits.Count];
		for (int i = 0; i < soldiers.Length; i++) {
			soldiers[i] = new BaseSoldier(UnitsConfig.Instance.GetSoldierData(_hiredUnits[i]), Global.Instance.Player.City.GetSoldierUpgradesInfo(_hiredUnits[i]).Level);
		}

		Global.Instance.CurrentMission.PlanetKey = EPlanetKey.PlanetA;
		Global.Instance.CurrentMission.MissionKey = EMissionKey.PlanetA_Test1;
		Global.Instance.CurrentMission.SelectedSoldiers = new ArrayRO<BaseSoldier>(soldiers);

		FightManager.Setup(EFightMode.Campaign, MissionsConfig.Instance.GetPlanet(Global.Instance.CurrentMission.PlanetKey).GetMission(Global.Instance.CurrentMission.MissionKey));
		Application.LoadLevel("Fight");
	}
}

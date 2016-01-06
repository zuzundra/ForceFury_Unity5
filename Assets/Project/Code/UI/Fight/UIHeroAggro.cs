using UnityEngine;
using UnityEngine.UI;

public class UIHeroAggro : MonoBehaviour {
	[SerializeField]
	private Image _imgAggroMeter;
	[SerializeField]
	private Text _lblAggroValue;

	public void Start() {
		UpdateAggro(Global.Instance.Player.Heroes.Current.AggroCrystals, Global.Instance.Player.Heroes.Current.AggroCrystalsMaximum);
		EventsAggregator.Units.AddListener<float, float>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	public void OnDestroy() {
		EventsAggregator.Units.RemoveListener<float, float>(EUnitEvent.AggroCrystalsUpdate, UpdateAggro);
	}

	private void UpdateAggro(float aggroValue, float aggroMax) {
		_imgAggroMeter.fillAmount = aggroValue - (int)aggroValue;
		_lblAggroValue.text = ((int)aggroValue).ToString();
	}
}

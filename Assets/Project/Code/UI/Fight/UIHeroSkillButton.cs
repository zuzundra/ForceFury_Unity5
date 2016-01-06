using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIHeroSkillButton : MonoBehaviour {
	[SerializeField]
	private Image _imgAbilitiIcon;
	[SerializeField]
	private Text _txtAbilityCost;
	[SerializeField]
	private UIHeroSkillButtonCooldown _cooldown;

	private Button _buttonComponent;

	private ESkillKey _skillKey = ESkillKey.None;
	private float _aggroCost = 0;

	public void Awake() {
		_buttonComponent = gameObject.GetComponent<Button>();
		_buttonComponent.onClick.AddListener(OnClick);

		EventsAggregator.UI.AddListener<ESkillKey, float>(EUIEvent.StartSkillCooldown, OnSkillCooldownStart);
	}

	public void OnDestroy() {
		if (_imgAbilitiIcon.sprite != null) {
			UIResourcesManager.Instance.FreeResource(_imgAbilitiIcon.sprite);
			_imgAbilitiIcon.sprite = null;
		}

		EventsAggregator.UI.RemoveListener<ESkillKey, float>(EUIEvent.StartSkillCooldown, OnSkillCooldownStart);
	}

	public void Setup(SkillParameters skillParams) {
		_skillKey = skillParams.Key;
		_aggroCost = skillParams.AggroCrystalsCost;

		if (!skillParams.IconPath.Equals(string.Empty)) {
			Sprite skillSprite = UIResourcesManager.Instance.GetResource<Sprite>(string.Format("{0}/{1}", GameConstants.Paths.UI_ABILITY_ICONS_RESOURCES, skillParams.IconPath));
			if (skillSprite != null) {
				_imgAbilitiIcon.sprite = skillSprite;
				_imgAbilitiIcon.gameObject.SetActive(true);
			}
		}

		if (_txtAbilityCost != null) {
			_txtAbilityCost.text = _aggroCost.ToString();
		}
	}

	private void OnClick() {
		if (Global.Instance.Player.Heroes.Current.AggroCrystals >= _aggroCost) {
			EventsAggregator.Units.Broadcast<ESkillKey>(EUnitEvent.SkillUsage, _skillKey);
		} else {
			//TODO: play some red blink animation (unable to cast ability)
		}
	}

	private void OnSkillCooldownStart(ESkillKey skillKey, float duration) {
		if (skillKey == _skillKey) {
			_cooldown.StartCooldown(duration);
		}
	}
}

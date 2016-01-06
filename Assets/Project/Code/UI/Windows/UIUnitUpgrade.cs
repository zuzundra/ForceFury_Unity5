using UnityEngine;
using UnityEngine.UI;

public class UIUnitUpgrade : MonoBehaviour {
	[SerializeField]
	private Button _btnBackground;
	[SerializeField]
	private Button _btnLevelUp;

	[SerializeField]
	private Text _lblUnitLevel;
	[SerializeField]
	private Text _lblUnitInfo;
	[SerializeField]
	private Image _imgUnitIcon;

	private EUnitKey _unitKey;

	public void Start() {
		_btnBackground.onClick.AddListener(Hide);
		_btnLevelUp.onClick.AddListener(OnLevelUpClick);
	}

	public void Setup(EUnitKey unitkey, Sprite sprIcon) {
		_imgUnitIcon.sprite = sprIcon;
		_unitKey = unitkey;
		UpdateUnitInfo();
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	private void OnLevelUpClick() {
		Global.Instance.Player.City.SoldierLevelUp(_unitKey);
		UpdateUnitInfo();
	}

	private void UpdateUnitInfo() {
		_lblUnitLevel.text = Global.Instance.Player.City.GetSoldierUpgradesInfo(_unitKey).Level.ToString();

		int innateDamage = UnitsConfig.Instance.GetSoldierData(_unitKey).BaseDamage;
		BaseItem lHandWeapon = ItemsConfig.Instance.GetItem(UnitsConfig.Instance.GetSoldierData(_unitKey).GetBaseItemInSlot(EUnitEqupmentSlot.Weapon_LHand));
		BaseItem rHandWeapon = ItemsConfig.Instance.GetItem(UnitsConfig.Instance.GetSoldierData(_unitKey).GetBaseItemInSlot(EUnitEqupmentSlot.Weapon_RHand));
		int weaponDamage = lHandWeapon != null ? lHandWeapon.ModDamage : (rHandWeapon != null ? rHandWeapon.ModDamage : 0);

		int levelsDamage = UnitsConfig.Instance.GetSoldierUpgrades(_unitKey).GetTotalLevelUpgrades(Global.Instance.Player.City.GetSoldierUpgradesInfo(_unitKey).Level).ModifierDamage;

		_lblUnitInfo.text = "Base damage: " + (innateDamage + weaponDamage) + "\nLevel-ups damage: " + levelsDamage + "\n-----\nTotal damage: " + (innateDamage + weaponDamage + levelsDamage);
	}
}

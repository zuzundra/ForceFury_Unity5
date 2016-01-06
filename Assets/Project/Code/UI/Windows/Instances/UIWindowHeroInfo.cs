using UnityEngine;
using UnityEngine.UI;

public class UIWindowHeroInfo : UIWindow {
	[SerializeField]
	private Button _btnBack;

	[SerializeField]
	private Button _btnSkill1;
	[SerializeField]
	private Button _btnSkill2;
	[SerializeField]
	private Button _btnSkillEmpty;

	[SerializeField]
	private Button _btnSkillSelected1;
	[SerializeField]
	private Button _btnSkillSelected2;

	[SerializeField]
	private Text _lblSkillSelected1;
	[SerializeField]
	private Text _lblSkillSelected2;

	private int _selectedSkillsAmount = 0;
	private ESkillKey[] _selectedSkills = new ESkillKey[] { ESkillKey.None, ESkillKey.None };

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);

		_btnSkill1.onClick.AddListener(OnSkill1Click);
		_btnSkill2.onClick.AddListener(OnSkill2Click);

		_btnSkillSelected1.onClick.AddListener(OnSelectedSkill1Click);
		_btnSkillSelected2.onClick.AddListener(OnSelectedSkill2Click);

		_btnSkillSelected1.image.sprite = _btnSkillEmpty.image.sprite;
		_lblSkillSelected1.text = string.Empty;
		_btnSkillSelected2.image.sprite = _btnSkillEmpty.image.sprite;
		_lblSkillSelected2.text = string.Empty;

		ListRO<ESkillKey> playerSkills = Global.Instance.Player.HeroSkills.GetHeroSkills(EUnitKey.Hero_Sniper);
		for (int i = 0; i < playerSkills.Count; i++) {
			if (playerSkills[i] == ESkillKey.ExplosiveCharges) {
				OnSkill1Click();
			} else if(playerSkills[i] == ESkillKey.StunGrenade) {
				OnSkill2Click();
			}
		}
	}

	#region listeners
	private void OnBtnBackClick() {
		Hide();
	}

	private void OnSkill1Click() {
		if (_selectedSkillsAmount == 0) {
			_btnSkillSelected1.image.sprite = _btnSkill1.image.sprite;
			_lblSkillSelected1.text = SkillsConfig.Instance.GetSkillParameters(ESkillKey.ExplosiveCharges).AggroCrystalsCost.ToString();
			Global.Instance.Player.HeroSkills.AddSkill(EUnitKey.Hero_Sniper, ESkillKey.ExplosiveCharges);
			_selectedSkillsAmount++;
			_selectedSkills[0] = ESkillKey.ExplosiveCharges;
		} else if(_selectedSkillsAmount == 1) {
			if (_selectedSkills[0] == ESkillKey.ExplosiveCharges) {
				return;
			}
			_btnSkillSelected2.image.sprite = _btnSkill1.image.sprite;
			_lblSkillSelected2.text = SkillsConfig.Instance.GetSkillParameters(ESkillKey.ExplosiveCharges).AggroCrystalsCost.ToString();
			Global.Instance.Player.HeroSkills.AddSkill(EUnitKey.Hero_Sniper, ESkillKey.ExplosiveCharges);
			_selectedSkillsAmount++;
			_selectedSkills[1] = ESkillKey.ExplosiveCharges;
		}
	}

	private void OnSkill2Click() {
		if (_selectedSkillsAmount == 0) {
			_btnSkillSelected1.image.sprite = _btnSkill2.image.sprite;
			_lblSkillSelected1.text = SkillsConfig.Instance.GetSkillParameters(ESkillKey.StunGrenade).AggroCrystalsCost.ToString();
			Global.Instance.Player.HeroSkills.AddSkill(EUnitKey.Hero_Sniper, ESkillKey.StunGrenade);
			_selectedSkillsAmount++;
			_selectedSkills[0] = ESkillKey.StunGrenade;
		} else if (_selectedSkillsAmount == 1) {
			if (_selectedSkills[0] == ESkillKey.StunGrenade) {
				return;
			}
			_btnSkillSelected2.image.sprite = _btnSkill2.image.sprite;
			_lblSkillSelected2.text = SkillsConfig.Instance.GetSkillParameters(ESkillKey.StunGrenade).AggroCrystalsCost.ToString();
			Global.Instance.Player.HeroSkills.AddSkill(EUnitKey.Hero_Sniper, ESkillKey.StunGrenade);
			_selectedSkillsAmount++;
			_selectedSkills[1] = ESkillKey.StunGrenade;
		}
	}

	private void OnSelectedSkill1Click() {
		if (_selectedSkillsAmount == 2) {
			_btnSkillSelected1.image.sprite = _btnSkillSelected1.image.sprite;
			_lblSkillSelected1.text = _lblSkillSelected2.text;
			Global.Instance.Player.HeroSkills.RemoveSkill(EUnitKey.Hero_Sniper, _selectedSkills[0]);
			_selectedSkills[0] = _selectedSkills[1];
			_selectedSkills[1] = ESkillKey.None;
			OnSelectedSkill2Click();
		} else if (_selectedSkillsAmount == 1) {
			_btnSkillSelected1.image.sprite = _btnSkillEmpty.image.sprite;
			_lblSkillSelected1.text = string.Empty;
			Global.Instance.Player.HeroSkills.RemoveSkill(EUnitKey.Hero_Sniper, _selectedSkills[0]);
			_selectedSkillsAmount--;
			_selectedSkills[0] = ESkillKey.None;
		}
	}

	private void OnSelectedSkill2Click() {
		if (_selectedSkillsAmount == 2) {
			_btnSkillSelected2.image.sprite = _btnSkillEmpty.image.sprite;
			_lblSkillSelected2.text = string.Empty;
			Global.Instance.Player.HeroSkills.RemoveSkill(EUnitKey.Hero_Sniper, _selectedSkills[1]);
			_selectedSkillsAmount--;
			_selectedSkills[1] = ESkillKey.None;
		}
	}
	#endregion
}

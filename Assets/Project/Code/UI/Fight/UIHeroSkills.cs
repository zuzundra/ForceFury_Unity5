using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroSkills : MonoBehaviour {
	[SerializeField]
	private UIHeroSkillButton[] _skillButtons;

	public void Start() {
		ListRO<ESkillKey> playerHeroSkillKeys = Global.Instance.Player.HeroSkills.GetHeroSkills(Global.Instance.Player.Heroes.Current.Data.Key);

		List<SkillParameters> playerHeroSkillParams = new List<SkillParameters>();
		playerHeroSkillParams.Add(SkillsConfig.Instance.HetHeroSkillParameters(Global.Instance.Player.Heroes.Current.Data.Key));

		SkillParameters skillParams = null;
		for (int i = 0; i < playerHeroSkillKeys.Count; i++) {
			skillParams = SkillsConfig.Instance.GetSkillParameters(playerHeroSkillKeys[i]);
			if (skillParams != null) {
				playerHeroSkillParams.Add(skillParams);
			}
		}

		for (int i = 0; i < _skillButtons.Length; i++) {
			if (playerHeroSkillParams.Count > i && playerHeroSkillParams[i] != null) {
				_skillButtons[i].Setup(playerHeroSkillParams[i]);
				_skillButtons[i].gameObject.SetActive(true);
			} else {
				_skillButtons[i].gameObject.SetActive(false);
			}
		}
	}
}

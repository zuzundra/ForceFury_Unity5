using System.Collections.Generic;
public class PlayerHeroSkills {
	private static int _maxSkillsAmount = 4;

	private Dictionary<EUnitKey, List<ESkillKey>> _heroSkillsData = new Dictionary<EUnitKey, List<ESkillKey>>();

	public ListRO<ESkillKey> GetHeroSkills(EUnitKey heroKey) {
		return new ListRO<ESkillKey>(GetHeroSkillsInternal(heroKey));
	}

	public void AddSkill(EUnitKey heroKey, ESkillKey skillKey, int index) {
		AddSkill(heroKey, skillKey);
		SetSkillIndex(heroKey, skillKey, index);
	}

	public void AddSkill(EUnitKey heroKey, ESkillKey skillKey) {
		List<ESkillKey> heroSkills = GetHeroSkillsInternal(heroKey);
		if (heroSkills.Count >= _maxSkillsAmount) {
			return;
		}
		if (heroSkills.IndexOf(skillKey) != -1) {
			return;
		}
		heroSkills.Add(skillKey);
	}

	public void  RemoveSkill(EUnitKey heroKey, ESkillKey skillKey) {
		List<ESkillKey> heroSkills = GetHeroSkillsInternal(heroKey);
		heroSkills.Remove(skillKey);
	}

	public void SetSkillIndex(EUnitKey heroKey, ESkillKey skillKey, int index) {
		List<ESkillKey> heroSkills = GetHeroSkillsInternal(heroKey);
		if (index < 0 || index >= heroSkills.Count) {
			return;
		}
		if (heroSkills.IndexOf(skillKey) != -1 && heroSkills.IndexOf(skillKey) != index) {
			heroSkills.Remove(skillKey);
			heroSkills.Insert(index, skillKey);
		}
	}

	private List<ESkillKey> GetHeroSkillsInternal(EUnitKey heroKey) {
		if (!_heroSkillsData.ContainsKey(heroKey)) {
			_heroSkillsData.Add(heroKey, new List<ESkillKey>());
		}
		if (_heroSkillsData[heroKey] == null) {
			_heroSkillsData[heroKey] = new List<ESkillKey>();
		}
		return _heroSkillsData[heroKey];
	}
}

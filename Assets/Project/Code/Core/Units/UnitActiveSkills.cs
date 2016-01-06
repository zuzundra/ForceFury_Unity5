using System.Collections.Generic;

public class UnitActiveSkills {
	private static Dictionary<ESkillKey, int> _skillDamagePriorities = new Dictionary<ESkillKey, int>() {
		{ ESkillKey.ClipDischarge, 1 },
		{ ESkillKey.ExplosiveCharges, 2 }
	};

	private List<BaseUnitSkill> _activeSkills = new List<BaseUnitSkill>();
	private ListRO<BaseUnitSkill> _activeSkillsRO = null;
	public ListRO<BaseUnitSkill> ActiveSkills {
		get {
			if (_activeSkillsRO == null) {
				_activeSkillsRO = new ListRO<BaseUnitSkill>(_activeSkills);
			}
			return _activeSkillsRO;
		}
	}

	private List<BaseUnitSkill> _damageModifyingSkills = new List<BaseUnitSkill>();

	public void RegisterSkill(BaseUnitSkill skill) {
		_activeSkills.Add(skill);

		if (_skillDamagePriorities.ContainsKey(skill.SkillParameters.Key)) {
			int skillPriority = _skillDamagePriorities[skill.SkillParameters.Key];
			int insertIndex = _damageModifyingSkills.Count;
			for (int i = 0; i < _damageModifyingSkills.Count; i++) {
				if (skillPriority < _skillDamagePriorities[_damageModifyingSkills[i].SkillParameters.Key]) {
					insertIndex = i;
					break;
				}
			}

			_damageModifyingSkills.Insert(insertIndex, skill);
		}
	}

	public void UnregisterSkill(BaseUnitSkill skill) {
		int skillIndex = _activeSkills.IndexOf(skill);
		if (skillIndex != -1) {
			_activeSkills.RemoveAt(skillIndex);
		}

		skillIndex = _damageModifyingSkills.IndexOf(skill);
		if (skillIndex != -1) {
			_damageModifyingSkills.RemoveAt(skillIndex);
		}
	}

	public bool HasActiveSkills() {
		return _activeSkills.Count > 0;
	}

	public int GetDamageFromSkillModifiers(int initialDamage) {
		float resultDamage = initialDamage;
		for (int i = 0; i < _damageModifyingSkills.Count; i++) {
			resultDamage *= _damageModifyingSkills[i].SkillParameters.ModDamagePercents;
		}

		return (int)resultDamage;
	}

	public void Clear() {
		_activeSkills.Clear();
		_damageModifyingSkills.Clear();
	}
}
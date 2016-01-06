using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillsConfig : MonoBehaviourResourceSingleton<SkillsConfig> {
#pragma warning disable 0414
	private static string _path = "Config/SkillsConfig";
#pragma warning restore 0414

	[SerializeField]
	private HeroUniqueSkillsData[] _heroesUniqueSkills = null;

	[SerializeField]
	private SkillParameters[] _skillsData = null;
	private ArrayRO<SkillParameters> _skillsDataRO = null;
	public ArrayRO<SkillParameters> SkillsData {
		get {
			if (_skillsDataRO == null) {
				_skillsDataRO = new ArrayRO<SkillParameters>(_skillsData);
			}
			return _skillsDataRO;
		}
	}

	public SkillParameters HetHeroSkillParameters(EUnitKey heroKey) {
		for (int i = 0; i < _heroesUniqueSkills.Length; i++) {
			if (_heroesUniqueSkills[i].HeroKey == heroKey) {
				return _heroesUniqueSkills[i].Skill;
			}
		}
		return null;
	}

	public SkillParameters GetSkillParameters(ESkillKey skillKey) {
		for (int i = 0; i < _skillsData.Length; i++) {
			if (_skillsData[i].Key == skillKey) {
				return _skillsData[i];
			}
		}
		return null;
	}

	public BaseUnitSkill GetSkillInstance(SkillParameters skillParams) {
		switch (skillParams.Key) {
			case ESkillKey.ClipDischarge:
				return new SkillClipDischarge(skillParams);
			case ESkillKey.ExplosiveCharges:
				return new SkillExplosiveCharges(skillParams);
			case ESkillKey.StunGrenade:
				return new SkillStunGrenade(skillParams);
		}

		return null;
	}

	/*
	public HeroSkillsData GetHeroSkillsData(EUnitKey heroKey) {
		for (int i = 0; i < _heroSkills.Length; i++) {
			if (_heroSkills[i].HeroKey == heroKey) {
				return _heroSkills[i];
			}
		}
		return null;
	}

	public Dictionary<ESkillKey, BaseUnitSkill> GetHeroSkillsInstances(EUnitKey heroKey) {
		Dictionary<ESkillKey, BaseUnitSkill> result = new Dictionary<ESkillKey, BaseUnitSkill>();

		HeroSkillsData heroSkillsData = GetHeroSkillsData(heroKey);
		if (heroSkillsData != null) {
			for (int i = 0; i < heroSkillsData.Skills.Length; i++) {
				result.Add(heroSkillsData.Skills[i].Key, GetSkillInstance(heroSkillsData.Skills[i]));
			}
		}

		return result;
	}
	*/
}

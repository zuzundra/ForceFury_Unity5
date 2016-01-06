using UnityEngine;

[System.Serializable]
public class HeroUniqueSkillsData {
	[SerializeField]
	private EUnitKey _heroKey = EUnitKey.Idle;
	public EUnitKey HeroKey {
		get { return _heroKey; }
	}

	[SerializeField]
	private SkillParameters _skill = null;
	public SkillParameters Skill {
		get { return _skill; }
	}
}

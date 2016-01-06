public enum ESkillKey {
	None = 0,
	ClipDischarge,		//разрядить обойму
	ExplosiveCharges,	//разрывные снаряды
	StunGrenade,		//оглушающая граната
	Skill3,		//temp
	Skill4		//temp
}

public enum ESkillType {
	Buff,
	Action
}

public enum ESkillTarget {
	None = 0,

	AnyAlly = 1,
	AllyHero,
	AllySoldier,

	AnyEnemy = 11,
	EnemyHero,
	EnemySoldier
}
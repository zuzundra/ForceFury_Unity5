using UnityEngine;

[System.Serializable]
public class BaseSoldierData : BaseUnitData {
    [SerializeField]
    protected EUnitRarity _rarity = EUnitRarity.Usual; //unit rarity
    public EUnitRarity Rarity
    {
        get { return _rarity; }
    }

	[SerializeField]
	protected int _leadershipCost = 0;	//how much leadership hero must have to hire the soldier
	public int LeadershipCost {
		get { return _leadershipCost; }
	}

	[SerializeField]
	protected int _npcReward = 0;
	public int NpcReward {
		get { return _npcReward; }
	}

    [SerializeField]
    protected int _agroCost = 0;
    public int AgroCost
    {
        get { return _agroCost; }
    }

    [SerializeField]
    protected EUnitKey _type = EUnitKey.Idle;
    public EUnitKey Type
    {
        get { return _type; }
    }
}

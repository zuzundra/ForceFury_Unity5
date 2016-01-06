using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero : BaseUnit {
	[SerializeField]
	protected new BaseHeroData _data;
	public new BaseHeroData Data {
		get { return _data; }
	}

	public int Leadership { get; private set; }	//hero leadership after all upgrades and level-ups
	public int Experience { get; private set; }	//hero experience
	public float AggroCrystalsMaximum { get; private set; }	//aggro crystals cap after all upgrages and level-ups
	private float _aggroCrystals = 0;	//aggro crystals amount after all upgrades and level-ups
	public float AggroCrystals {
		get { return _aggroCrystals; }
		private set {
			_aggroCrystals = (value < 0 ? 0 : (value > AggroCrystalsMaximum ? AggroCrystalsMaximum : value));
			EventsAggregator.Units.Broadcast<float, float>(EUnitEvent.AggroCrystalsUpdate, _aggroCrystals, AggroCrystalsMaximum);
		}
	}

	public BaseHero(BaseHeroData data, int experience) : base(data) {
		_data = data;
		AddExperience(experience);

		RecalculateParamsInternal();

		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.AddListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	~BaseHero() {
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.AllyDeath, OnAllyDeath);
		EventsAggregator.Fight.RemoveListener<BaseUnit>(EFightEvent.EnemyDeath, OnEnemyDeath);
	}

	public override void Attack(BaseUnit target) {
		base.Attack(target);

		AggroCrystals += _data.BaseAggroCrystalPerAttack;
	}

	public void UseSkill(SkillParameters skill) {
		AggroCrystals -= skill.AggroCrystalsCost;
	}

	public virtual void ResetAggro() {
		AggroCrystals = 0;
	}

	protected override Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		BaseHeroData heroData = base._data as BaseHeroData;
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
			for (int j = 0; j < heroData.AvailableItemTypes.Length; j++) {
				if (heroData.AvailableItemTypes[j].SlotKey == availableSlots[i]) {
					slotsData[availableSlots[i]] = heroData.AvailableItemTypes[j].AvailableItemTypes.DataCopy;
					break;
				}
			}
		}
		return slotsData;
	}

	protected override void RecalculateParamsInternal() {
		base.RecalculateParamsInternal();

		//TODO:
		// - recalculate health after level-ups
		// - recalculate damage after level-ups
		// - recalculate damage range after level-ups
		// - recalculate damage speed after level-ups
		// - recalculate aggro crystals after level-ups
		// - recalculate leadership after level-ups
		if (_data != null) {
			Leadership = _data.BaseLeadership;
			AggroCrystalsMaximum = _data.BaseAggroCrystalsMaximum;
		}

		EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.RecalculateParams, this);
	}

	public void AddExperience(int expAmount) {
		if (expAmount > 0) {
			Experience += expAmount;

			//TODO: check new level and level up if necessary
		}
	}

	#region listeners
	protected void OnAllyDeath(BaseUnit unit) {
		if (unit == this) {
			return;
		}
        //AggroCrystals += unit.Data.AggroCrystalsForDeathToAlly;
    }

	protected void OnEnemyDeath(BaseUnit unit) {
        //AggroCrystals += unit.Data.AggroCrystalsForDeathToEnemy;
	}
	#endregion
}

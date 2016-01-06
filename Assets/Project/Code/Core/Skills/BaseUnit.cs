using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseUnit  {
	protected BaseUnitData _data;
	public BaseUnitData Data {
		get { return _data; }
	}

	protected int _level = 1;
	public int Level {
		get { return _level; }
		set {
			if (value > _level) {
				_level = value;
				RecalculateParams();
			}
		}
	}

	public int HealthPoints { get; private set; }	//health amount after all upgrades applied
	public int Damage { get; private set; }	//damage amount after all upgrades applied

    UnitPlace _templatePlace = new UnitPlace() { Range = EUnitRange.None, Position = EUnitPosition.None };
    public UnitPlace TemplatePlace
    {
        get
        {
            return _templatePlace;
        }
        set
        {
            _templatePlace = value;
        }
    }

	//public float AR { get; private set; }	//damage range after all upgrades applied
	//public float AttackSpeed { get; private set; }	//damage speed after all upgrades applied

	//public int Armor { get; private set; }	//armor amount after all upgrades applied
	//public int ArmorDamageAbsorb { get; private set; }	//how much damage will be absorbed by armor all upgrades applied
	
	//public int CritChance { get; private set; }	//critical hit chance after all upgrades applied
	//public float CritDamageMultiplier { get; private set; }	//critical hit damage multiplier after all upgrades applied

	public UnitInventory Inventory { get; private set; }

	public int DamageTaken { get; private set; }
	public bool IsDead { get { return DamageTaken >= HealthPoints; } }

	protected UnitActiveSkills _activeSkills = new UnitActiveSkills();
	public UnitActiveSkills ActiveSkills {
		get { return _activeSkills; }
	}

	public BaseUnit(BaseUnitData data) {
		_data = data;
		Inventory = new UnitInventory(CreateSlotsData(), _data.BaseEquipment, OnEquipmentUpdate);

		RecalculateParamsInternal();
	}

	public virtual void Attack(BaseUnit target) {
		target.ApplyDamage(GetAttackInfo(true));//, true));
	}

	public virtual AttackInfo GetAttackInfo(bool withSkillEffects) { //, bool withCrit) {
		//bool isCrit = withCrit && Random.Range(0, 100) < CritChance;
		int damage = withSkillEffects ? _activeSkills.GetDamageFromSkillModifiers(Damage) : Damage;
        //return new AttackInfo(isCrit ? (int)(damage * CritDamageMultiplier) : damage, isCrit);
        return new AttackInfo(damage);
    }

	public virtual void ApplyDamage(AttackInfo attackInfo) {

		//attackInfo.DamageAmount -= ArmorDamageAbsorb;
		if (IsDead || attackInfo.DamageAmount <= 0) {
			return;
		}

		DamageTaken += attackInfo.DamageAmount;

		//broadcast hit
		EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.HitReceived, this, 
            new HitInfo(HealthPoints - DamageTaken + attackInfo.DamageAmount, HealthPoints - DamageTaken));//, attackInfo.IsCritical));

		if (IsDead) {
			//broadcast death
			EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.DeathCame, this);
		}
	}

	public virtual void ApplyHeal(AttackInfo attackInfo, bool revive) {
		if (attackInfo.DamageAmount <= 0) {
			return;
		}

		bool preHealDeadState = IsDead;

		if (IsDead && !revive) {
			return;
		}

		DamageTaken -= attackInfo.DamageAmount;
		
		if (preHealDeadState && !IsDead) {
			//broadcast revive
			EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.ReviveCame, this, 
                new HitInfo(HealthPoints - DamageTaken - attackInfo.DamageAmount, HealthPoints - DamageTaken));//, attackInfo.IsCritical));
		} else {
			//broadcast heal
			EventsAggregator.Units.Broadcast<BaseUnit, HitInfo>(EUnitEvent.HitReceived, this, 
                new HitInfo(HealthPoints - DamageTaken - attackInfo.DamageAmount, HealthPoints - DamageTaken));//, attackInfo.IsCritical));
		}
	}

	public virtual void ResetDamageTaken() {
		DamageTaken = 0;
	}

	protected virtual Dictionary<EUnitEqupmentSlot, EItemType[]> CreateSlotsData() {
		Dictionary<EUnitEqupmentSlot, EItemType[]> slotsData = new Dictionary<EUnitEqupmentSlot, EItemType[]>();
		ArrayRO<EUnitEqupmentSlot> availableSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		for (int i = 0; i < availableSlots.Length; i++) {
			slotsData.Add(availableSlots[i], new EItemType[0]);
		}
		return slotsData;
	}

	protected virtual void RecalculateParamsInternal() {
		HealthPoints = _data.BaseHealthPoints;
		Damage = _data.BaseDamage;

		//AR = _data.BaseAR;
		//AttackSpeed = _data.BaseAttackSpeed;

		//Armor = _data.BaseArmor;
		//CritChance = _data.BaseCritChance;
		//CritDamageMultiplier = _data.BaseCritDamageMultiplier;

		//levels bonuses
		SoldierUpgrade soldierUpgradesData = UnitsConfig.Instance.GetSoldierUpgrades(_data.Key);
		if (soldierUpgradesData != null) {
			SoldierUpgradeLevel upgradesLevelData = UnitsConfig.Instance.GetSoldierUpgrades(_data.Key).GetTotalLevelUpgrades(_level);
			Damage += upgradesLevelData.ModifierDamage;
		}

		//equipment bonuses
		ArrayRO<EUnitEqupmentSlot> equipmentSlots = UnitsConfig.Instance.GetUnitEquipmentSlots(this);
		BaseItem itemData = null;
		for (int i = 0; i < equipmentSlots.Length; i++) {
			itemData = ItemsConfig.Instance.GetItem(Inventory.GetItemInSlot(i));
			if (itemData != null) {
				HealthPoints += itemData.ModHealth;
				Damage += itemData.ModDamage;

				//Armor += itemData.ModArmor;
				//AttackRange += itemData.ModDamageRange;
				//AttackSpeed += itemData.ModDamageSpeed;
				//CritChance += itemData.ModCritChance;
				//CritDamageMultiplier = itemData.ModCritDamageMultiplier;
			}
		}
		//ArmorDamageAbsorb = Mathf.CeilToInt(UnitsConfig.Instance.DamageReducePerOneArmor * Armor);
    }

	protected void RecalculateParams() {
		RecalculateParamsInternal();
		EventsAggregator.Units.Broadcast<BaseUnit>(EUnitEvent.RecalculateParams, this);
	}

	protected virtual void OnEquipmentUpdate(EUnitEqupmentSlot slot, EItemKey oldItemKey, EItemKey newItemKey) {
		RecalculateParams();

		EventsAggregator.Units.Broadcast<BaseUnit, EUnitEqupmentSlot, EItemKey, EItemKey>(EUnitEvent.EquipmentUpdate, this, slot, oldItemKey, newItemKey);
	}
}

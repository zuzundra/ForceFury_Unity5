using UnityEngine;
using System.Collections.Generic;

public class FightLogger {
	private struct UnitDamageInfo {
		private EUnitKey _attacker;
		public EUnitKey Attacker {
			get { return _attacker; }
		}

		private EUnitKey _target;
		public EUnitKey Target {
			get { return _target; }
		}

		private int _damageAmount;
		public int DamageAmount {
			get { return _damageAmount; }
		}

		public UnitDamageInfo(EUnitKey attacker, EUnitKey target, int damageAmount) {
			_attacker = attacker;
			_target = target;
			_damageAmount = damageAmount;
		}
	}

	private Dictionary<EUnitKey, List<UnitDamageInfo>> _alliesAttacks = new Dictionary<EUnitKey, List<UnitDamageInfo>>();
	private Dictionary<EUnitKey, List<UnitDamageInfo>> _enemyAttacks = new Dictionary<EUnitKey, List<UnitDamageInfo>>();

	public void LogDamage(BaseUnitBehaviour attacker, BaseUnitBehaviour target) {
		GetAttacksList(attacker).Add(new UnitDamageInfo(attacker.UnitData.Data.Key, target.UnitData.Data.Key, attacker.UnitData.Damage));

		//WARNING! temp info
		Debug.Log(attacker.gameObject.tag + " unit " + attacker.UnitData.Data.Key + " (" + attacker.name
            + ") attacks " + target.gameObject.tag + " unit " + target.UnitData.Data.Key + " (" + target.name
            + ") for " + attacker.UnitData.Damage + " damage");
	}

	public string ToJSON() {
		//TODO: format data for server
		return string.Empty;
	}

	public void Clear() {
		_alliesAttacks.Clear();
		_enemyAttacks.Clear();
	}

	private List<UnitDamageInfo> GetAttacksList(BaseUnitBehaviour unitBeh) {
		if (unitBeh.IsAlly) {
			if (!_alliesAttacks.ContainsKey(unitBeh.UnitData.Data.Key)) {
				_alliesAttacks.Add(unitBeh.UnitData.Data.Key, new List<UnitDamageInfo>());
			}
			return _alliesAttacks[unitBeh.UnitData.Data.Key];
		}

		if (!_enemyAttacks.ContainsKey(unitBeh.UnitData.Data.Key)) {
			_enemyAttacks.Add(unitBeh.UnitData.Data.Key, new List<UnitDamageInfo>());
		}
		return _enemyAttacks[unitBeh.UnitData.Data.Key];
	}
}

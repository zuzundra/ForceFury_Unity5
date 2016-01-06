using UnityEngine;

[System.Serializable]
public class SoldierUpgradeLevel {
	[SerializeField]
	private int _modifierDamage = 0;
	public int ModifierDamage {
		get { return _modifierDamage; }
	}

	public SoldierUpgradeLevel() { }
	public SoldierUpgradeLevel(int modDamage) {
		_modifierDamage = modDamage;
	}
}

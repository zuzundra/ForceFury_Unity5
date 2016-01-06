public struct AttackInfo {
	private int _damageAmount;
	public int DamageAmount {
		get { return _damageAmount; }
		set { _damageAmount = value; }
	}

    //private bool _isCritical;
    //public bool IsCritical {
    //    get { return _isCritical; }
    //    set { _isCritical = value; }
    //}

	public AttackInfo(int damageAmount) {//, bool isCritical) {
		_damageAmount = damageAmount;
		//_isCritical = isCritical;
	}
}

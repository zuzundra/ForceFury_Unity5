public struct HitInfo {
	private int _healthBefore;
	public int HealthBefore {
		get { return _healthBefore; }
	}

	private int _healthAfter;
	public int HealthAfter {
		get { return _healthAfter; }
	}

    //private bool _isCritical;
    //public bool IsCritical {
    //    get { return _isCritical; }
    //    set { _isCritical = value; }
    //}

	public bool IsLethal {
		get { return _healthBefore > 0 && _healthAfter <= 0; }
	}

	public HitInfo(int healthBefore, int healthAfter) {//, bool isCritical) {
		_healthBefore = healthBefore;
		_healthAfter = healthAfter;
		//_isCritical = isCritical;
	}
}
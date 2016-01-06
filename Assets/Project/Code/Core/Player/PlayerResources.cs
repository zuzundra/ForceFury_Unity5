public class PlayerResources {
	private int _fuel = 0;
	public int Fuel {
		get { return _fuel; }
		set {
			if (value != _fuel) {
				_fuel = value;
				if (_fuel < 0) {
					_fuel = 0;
				}

				EventsAggregator.Player.Broadcast<int>(EPlayerEvent.FuelUpdate, _fuel);
			}
		}
	}

	private int _credits = 0;
	public int Credits {
		get { return _credits; }
		set {
			if (value != _credits) {
				_credits = value;
				if (_credits < 0) {
					_credits = 0;
				}

				EventsAggregator.Player.Broadcast<int>(EPlayerEvent.CreditsUpdate, _credits);
			}
		}
	}

	private int _minerals = 0;
	public int Minerals {
		get { return _minerals; }
		set {
			if (value != _minerals) {
				_minerals = value;
				if (_minerals < 0) {
					_minerals = 0;
				}

				EventsAggregator.Player.Broadcast<int>(EPlayerEvent.MineralsUpdate, _minerals);
			}
		}
	}

	public PlayerResources(int fuel, int credits, int minerals) {
		Fuel = fuel;
		Credits = credits;
		Minerals = minerals;
	}
}

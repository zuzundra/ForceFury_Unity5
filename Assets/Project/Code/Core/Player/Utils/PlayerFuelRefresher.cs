public class PlayerFuelRefresher {
	//TODO: check fuel max level and run timer only on low fuel

	public PlayerFuelRefresher(int lastRefreshTime) {
		GameTimer.Instance.AddListener(lastRefreshTime - Utils.UnixTimestamp + GameConstants.City.FUEL_REFRESH_TIME, Update);
	}

	private void Update() {
		Global.Instance.Player.Resources.Fuel += GameConstants.City.FUEL_REFRESH_AMOUNT;
		GameTimer.Instance.AddListener(Utils.UnixTimestamp + GameConstants.City.FUEL_REFRESH_TIME, Update);
	}
}

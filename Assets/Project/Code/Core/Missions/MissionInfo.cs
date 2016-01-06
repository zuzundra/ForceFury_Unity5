public class MissionInfo {
	public EPlanetKey PlanetKey { get; set; }
	public EMissionKey MissionKey { get; set; }

	public ArrayRO<BaseSoldier> SelectedSoldiers { get; set; }

	public void Clear() {
		PlanetKey = EPlanetKey.None;
		MissionKey = EMissionKey.None;

		SelectedSoldiers = null;
	}
}

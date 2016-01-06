using UnityEngine;

public class MissionsConfig : MonoBehaviourResourceSingleton<MissionsConfig> {
#pragma warning disable 0414
	private static string _path = "Config/MissionsConfig";
#pragma warning restore 0414

	[SerializeField]
	private float _unitsXPositionStartOffset = 0f;	//start units offset from battlefield
	public float UnitsXPositionStartOffset {
		get { return _unitsXPositionStartOffset; }
	}

	//private Vector2 _battlefieldSize = new Vector2();	//can be taken frrom Pathfinder class

	[SerializeField]
	private PlanetData[] _planets = null;
	private ArrayRO<PlanetData> _planetsRO = null;
	public ArrayRO<PlanetData> Planets {
		get {
			if (_planetsRO == null) {
				_planetsRO = new ArrayRO<PlanetData>(_planets);
			}
			return _planetsRO;
		}
	}

	public PlanetData GetPreviuosPlanet(EPlanetKey planetKey) {
		for (int i = 0; i < _planets.Length; i++) {
			if (_planets[i].Key == planetKey) {
				if (i > 0) {
					return _planets[i - 1];
				} else {
					return null;
				}
			}
		}
		return null;
	}

	public PlanetData GetPlanet(EPlanetKey planetKey) {
		for (int i = 0; i < _planets.Length; i++) {
			if (_planets[i].Key == planetKey) {
				return _planets[i];
			}
		}
		return null;
	}

	public PlanetData GetPlanet(EMissionKey missionKey) {
		if (missionKey != EMissionKey.None) {
			for (int i = 0; i < _planets.Length; i++) {
				for (int j = 0; j < _planets[i].Missions.Length; j++) {
					if (_planets[i].Missions[j].Key == missionKey) {
						return _planets[i];
					}
				}
			}
		}
		return null;
	}
}

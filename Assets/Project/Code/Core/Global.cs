public class Global {
	#region singleton
	private static Global _instance = null;
	public static Global Instance {
		get {
			if (_instance == null) {
				_instance = new Global();
			}
			return _instance;
		}
	}
	#endregion

	private static bool _isInitialized = false;
	public static bool IsInitialized {
		get { return _isInitialized; }
	}

	public NetworkManager Network { get; private set; }
	public PlayerData Player { get; private set; }
	public MissionInfo CurrentMission { get; private set; }

	public Global() {
		Network = new NetworkManager();
		Player = new PlayerData();
		CurrentMission = new MissionInfo();
	}

	public void Initialize() {
		if (!_isInitialized) {
			UnityEngine.Application.targetFrameRate = 44;
			Player.Load();
			_isInitialized = true;
		}
	}
}

public enum ENetworkEvent {
	Idle = 0,

	PlayerDataLoadStart,
	PlayerDataLoadSuccess,
	PlayerDataLoadFail,

	PlayerDataSaveStart,
	PlayerDataSaveSuccess,
	PlayerDataSaveFail,

	MissionResultsSaveStart,
	MissionResultsSaveSuccess,
	MissionResultsSaveFail,

	FightDataCheckResponse
}
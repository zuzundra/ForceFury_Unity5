using System;

public class NetworkManager {
	//TODO: create loading queue

	public void LoadPlayerData() {
		//TODO: load player resources, progress, heroes
		EventsAggregator.Network.Broadcast(ENetworkEvent.PlayerDataLoadSuccess);
	}

	public void SavePlayerData(PlayerData data) {
		//TODO: save player resources, progress, heroes
		EventsAggregator.Network.Broadcast(ENetworkEvent.PlayerDataSaveSuccess);
	}

	public void SendFightResults(string data) {
		//TODO: send fight data and wait for server answer
		EventsAggregator.Network.Broadcast<bool>(ENetworkEvent.FightDataCheckResponse, true);
	}

	public void SaveMissionSuccessResults() {
		//TODO: save mission results
		EventsAggregator.Network.Broadcast(ENetworkEvent.MissionResultsSaveSuccess, true);
	}

	public void SaveMissionFailResults() {
		//TODO: save mission results
		EventsAggregator.Network.Broadcast(ENetworkEvent.MissionResultsSaveSuccess, true);
	}
}

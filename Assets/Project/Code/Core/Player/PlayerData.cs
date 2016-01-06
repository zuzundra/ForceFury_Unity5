using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Complete player information
/// </summary>
public class PlayerData {
	public PlayerResources Resources { get; private set; }
	public PlayerInventory Inventory { get; private set; }
	public PlayerHeroes Heroes { get; private set; }
	public PlayerHeroSkills HeroSkills { get; private set; }
	public PlayerCity City { get; private set; }
	public PlayerStoryProgress StoryProgress { get; private set; }
	public PlayerStatistics Statistics { get; private set; }

	private EPlayerVIP _vipStatus = EPlayerVIP.None;
	private EPlayerVIP VIPStatus {
		get { return _vipStatus; }
		set {
			_vipStatus = value;
			EventsAggregator.Player.Broadcast<EPlayerVIP>(EPlayerEvent.VIPUpdate, _vipStatus);
		}
	}

	//load player data
	public void Load() {
		EventsAggregator.Network.AddListener(ENetworkEvent.PlayerDataLoadSuccess, OnLoadSuccess);
		Global.Instance.Network.LoadPlayerData();
	}

	//player data load success callback
	private void OnLoadSuccess() {
		EventsAggregator.Network.RemoveListener(ENetworkEvent.PlayerDataLoadSuccess, OnLoadSuccess);

		//TODO: assign correct values
		Resources = new PlayerResources(10, 2500, 100);
		Inventory = new PlayerInventory();
		Heroes = new PlayerHeroes(new BaseHero[] { new BaseHero(UnitsConfig.Instance.GetHeroData(EUnitKey.Hero_Sniper), 0) }, 0);
		HeroSkills = new PlayerHeroSkills();
		City = new PlayerCity();
		StoryProgress = new PlayerStoryProgress();
		Statistics = new PlayerStatistics();
		VIPStatus = EPlayerVIP.None;
	}

	//save player data
	public void Save() {
		Global.Instance.Network.SavePlayerData(this);
	}

	~PlayerData() {
		EventsAggregator.Network.RemoveListener(ENetworkEvent.PlayerDataLoadSuccess, OnLoadSuccess);
	}
}

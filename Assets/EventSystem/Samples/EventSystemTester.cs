using UnityEngine;
using System.Collections;

public class EventSystemTester : MonoBehaviour {
	int i = 0;
	
	void Start () {
		//EventsAggregator.gameState.AddListener<int>(GameStateEvent.GameOver, Qwe);
	}
	
	private void Broadcast() {
		float t = Time.realtimeSinceStartup;
		for(int i = 0; i < 100000; i++) {
			//EventsAggregator.gameState.Broadcast(GameStateEvent.GameOver, 1);
		}
		Debug.Log("Testing EventSystem: time per 100k broadcasts in " + (Time.realtimeSinceStartup - t) + " sec.");
	}
	
	private void Qwe(int q) {
		i += q;
	}
	
	public void OnGUI() {
		if(GUI.Button(new Rect(5, 5, 100, 50), "Text")) {
			Broadcast();
		}
	}
}

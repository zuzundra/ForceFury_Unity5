using UnityEngine;

public class test : MonoBehaviour {
	public void Start() {

	}

	public void OnGUI() {
		if (GUI.Button(new Rect(5, 5, 100, 50), "Log")) {
			//HCCPathfinder.logThisShit = true;
		}
	}
}

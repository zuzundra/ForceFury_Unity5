using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasResolutionAdapter : MonoBehaviour {
	public void Start() {
		Utils.UI.AdaptCanvasResolution(GameConstants.DEFAULT_RESOLUTION_WIDTH, GameConstants.DEFAULT_RESOLUTION_HEIGHT, gameObject.GetComponent<Canvas>());
		this.enabled = false;
	}
}

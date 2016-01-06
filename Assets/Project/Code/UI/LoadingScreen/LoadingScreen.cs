using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviourResourceSingleton<LoadingScreen> {
#pragma warning disable 0414
	private static string _path = "UI/LoadingScreen";
#pragma warning restore 0414

	[SerializeField]
	private Image _imgProgress;

	public void Awake() {
		float widthRatio = 1f * Screen.width / GameConstants.DEFAULT_RESOLUTION_WIDTH;
		float heightRatio = 1f * Screen.height / GameConstants.DEFAULT_RESOLUTION_HEIGHT;

		gameObject.GetComponent<CanvasScaler>().scaleFactor = Mathf.Max(widthRatio, heightRatio);
		Hide();
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void SetProgress(float progress) {
		_imgProgress.fillAmount = progress;
	}
}

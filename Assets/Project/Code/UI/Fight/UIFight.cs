using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFight : MonoBehaviour {
	[SerializeField]
	private Canvas _canvasUI;
	public Canvas CanvasUI {
		get { return _canvasUI; }
	}
	[SerializeField]
	private Canvas _canvasBG;
	public Canvas CanvasBG {
		get { return _canvasBG; }
	}

	[SerializeField]
	private Image _imgMapBackground;
	public Image ImgMapBackground {
		get { return _imgMapBackground; }
	}

	[SerializeField]
	private Button _btnPause;
	[SerializeField]
	private Button _btnWithdraw;
	[SerializeField]
	private Button _btnNextMap;
	[SerializeField]
	private Image _imgFader;

	public void Awake() {
		EventsAggregator.Fight.AddListener(EFightEvent.MapComplete, OnMapComplete);
	}

	public void Start() {
		_btnPause.onClick.AddListener(FightManager.SceneInstance.TogglePause);
		_btnWithdraw.onClick.AddListener(FightManager.SceneInstance.Withdraw);
		_btnNextMap.onClick.AddListener(NextMap);

		_btnNextMap.gameObject.SetActive(false);

		HideFader();
	}

	public void OnDestroy() {
		EventsAggregator.Fight.RemoveListener(EFightEvent.MapComplete, OnMapComplete);
	}

	private void NextMap() {
		_btnNextMap.gameObject.SetActive(false);
		FightManager.SceneInstance.PrepareMapSwitch();
	}

	private void OnMapComplete() {
		if (!FightManager.SceneInstance.IsLastMap) {
			_btnNextMap.gameObject.SetActive(true);
		}
	}

	#region fading
	public void ShowFader(float duration) {
		StartCoroutine(FadeRoutine(duration));
	}

	public void HideFader() {
		_imgFader.enabled = false;
	}

	private IEnumerator FadeRoutine(float duration) {
		float startTime = Time.time;
		float endTime = startTime + duration;

		Color color = _imgFader.color;
		color.a = 0f;
		_imgFader.color = color;

		_imgFader.enabled = true;

		while(Time.time < endTime) {
			yield return null;
			color.a = (Time.time - startTime) / duration;
			_imgFader.color = color;
		}
	}
	#endregion
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitMonolog : MonoBehaviour {
	[SerializeField]
	private MeshRenderer _mrCharacterScreen;
	[SerializeField]
	private Text _lblText;
	[SerializeField]
	private RectTransform _textRootTransform;
	[SerializeField]
	private Canvas _canvasOverlay;
	[SerializeField]
	private Button _btnOverlay;
	[SerializeField]
	private Camera _characterCamera;

	private RenderTexture _rTex;

	public void Awake() {
		_characterCamera.transform.parent = null;
		_canvasOverlay.transform.SetParent(null, false);

		_rTex = new RenderTexture(512, 512, 24);
		_rTex.antiAliasing = 1;
		_rTex.wrapMode = TextureWrapMode.Clamp;
		_rTex.filterMode = FilterMode.Bilinear;
		_rTex.anisoLevel = 0;

		_mrCharacterScreen.material.mainTexture = _rTex;
		_characterCamera.targetTexture = _rTex;

		FightCamera.AdaptDialog(4096, transform);
		Hide();
	}

	public void OnDestroy() {
		GameObject.Destroy(_characterCamera.gameObject);

		GameObject.Destroy(_rTex);
		GameObject.Destroy(_canvasOverlay.gameObject);
		_rTex = null;
	}

	public void Show(UnitDialodEntity dialogData, Action clickCallback) {
		//set click callback
		_btnOverlay.onClick.AddListener(() => { clickCallback(); });

		//set text position
		Vector2 textPosition = _textRootTransform.anchoredPosition;
		textPosition.x = dialogData.Speaker == EFightDialogSpeaker.PlayerHero ? Mathf.Abs(textPosition.x) : -Mathf.Abs(textPosition.x);
		_textRootTransform.anchoredPosition = textPosition;

		//target camera to speaker
		if (dialogData.Speaker == EFightDialogSpeaker.PlayerHero) {
			_characterCamera.transform.position = FightManager.SceneInstance.AllyHero.transform.position + dialogData.CameraOffset;
			FightManager.SceneInstance.AllyHero.ModelView.PlaySpeakAnimation();
		} else if(dialogData.Speaker == EFightDialogSpeaker.EnemyUnit && dialogData.UnitKey != EUnitKey.Idle) {
			for (int i = 0; i < FightManager.SceneInstance.EnemyUnits.Length; i++) {
				if (FightManager.SceneInstance.EnemyUnits[i].UnitData.Data.Key == dialogData.UnitKey) {
					_characterCamera.transform.position = FightManager.SceneInstance.EnemyUnits[i].transform.position + dialogData.CameraOffset;
				}
			}
		}

		//position self
		float screenWidthOffset = 0f;
		switch (dialogData.Speaker) {
			case EFightDialogSpeaker.PlayerHero:
				screenWidthOffset = 0.2f;
				break;
			case EFightDialogSpeaker.EnemyUnit:
				screenWidthOffset = 0.85f;
				break;
		}

		Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * screenWidthOffset, Screen.height * 0.5f, Camera.main.nearClipPlane));
		worldPos.z += 1f;

		transform.position = worldPos;
		transform.localRotation = Quaternion.Euler(GameConstants.CAMERA_ROTATION);

		_lblText.text = dialogData.Text;
		gameObject.SetActive(true);
		_characterCamera.gameObject.SetActive(true);
		_canvasOverlay.enabled = true;
	}

	public void Hide() {
		_btnOverlay.onClick.RemoveAllListeners();
		gameObject.SetActive(false);
		_characterCamera.gameObject.SetActive(false);
		_canvasOverlay.enabled = false;
	}
}

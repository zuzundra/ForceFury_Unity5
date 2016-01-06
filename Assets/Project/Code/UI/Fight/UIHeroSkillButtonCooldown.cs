using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHeroSkillButtonCooldown : MonoBehaviour {
	private Image _image = null;

	private float _duration = 0f;
	private float _timeStart = 0f;

	public void Awake() {
		_image = gameObject.GetComponent<Image>();

		EndCooldown();
	}

	public void Update() {
		_image.fillAmount = 1f - ((Time.time - _timeStart) / _duration);
		if (Time.time > _timeStart + _duration) {
			EndCooldown();
		}
	}

	public void StartCooldown(float duration) {
		_duration = duration;
		_timeStart = Time.time;

		_image.fillAmount = 1f;
		gameObject.SetActive(true);
	}

	public void EndCooldown() {
		_duration = 0f;
		_timeStart = 0f;

		_image.fillAmount = 0f;
		gameObject.SetActive(false);
	}
}

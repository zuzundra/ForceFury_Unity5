using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {
	[SerializeField]
	private Image _hp;

	[SerializeField]
	private Text _damageText;
	private Text[] _damageTextArray;

    //[SerializeField]
    //private Text _critText;
    //private Text[] _critTextArray;

	[SerializeField]
	private int _textAnimationsCount = 0;

	private float _initialWidth = 0f;

	public void Awake() {
		_damageTextArray = new Text[7];
		_damageTextArray[0] = _damageText;
		_damageText.enabled = false;
		for (int i = 1; i < _damageTextArray.Length; i++) {
			_damageTextArray[i] = (GameObject.Instantiate(_damageText.gameObject) as GameObject).GetComponent<Text>();
			_damageTextArray[i].transform.SetParent(transform, false);
			_damageTextArray[i].enabled = false;
		}

        //_critTextArray = new Text[3];
        //_critTextArray[0] = _critText;
        //_critText.enabled = false;
        //for (int i = 1; i < _critTextArray.Length; i++) {
        //    _critTextArray[i] = (GameObject.Instantiate(_critText.gameObject) as GameObject).GetComponent<Text>();
        //    _critTextArray[i].transform.SetParent(transform, false);
        //    _critTextArray[i].enabled = false;
        //}

		_initialWidth = gameObject.GetComponent<RectTransform>().rect.width;
	}

	public void UpdateHealthBar(float percents) {
		_hp.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _initialWidth * percents);
	}

	public void ApplyDamage(int totalHealth, HitInfo hitInfo) {
		UpdateHealthBar(1f * hitInfo.HealthAfter / totalHealth);

		StartCoroutine(PlayFloatingTextAnimation(GetFreeFloatingText(_damageTextArray),//hitInfo.IsCritical ? _critTextArray : _damageTextArray), 
            hitInfo.HealthBefore - hitInfo.HealthAfter));
	}

	public void Reset() {
		StopAllCoroutines();

		for (int i = 0; i < _damageTextArray.Length; i++) {
			_damageTextArray[i].enabled = false;
		}
        //for (int i = 0; i < _critTextArray.Length; i++) {
        //    _critTextArray[i].enabled = false;
        //}
	}

	private IEnumerator PlayFloatingTextAnimation(Text t, int damageAmount) {
		if (t == null) {
			yield break;
		}

		t.text = damageAmount.ToString();
		t.enabled = true;

		//TODO: optimize
		Animator a = t.GetComponent<Animator>();
		a.Play("TextFloat" + Random.Range(1, _textAnimationsCount + 1), 0, 0);
		yield return null;
		yield return new WaitForSeconds(a.GetCurrentAnimatorStateInfo(0).length);

		t.enabled = false;
	}

	private Text GetFreeFloatingText(Text[] textArray) {
		for (int i = 0; i < textArray.Length; i++) {
			if (!textArray[i].enabled) {
				return textArray[i];
			}
		}
		return null;
	}
}

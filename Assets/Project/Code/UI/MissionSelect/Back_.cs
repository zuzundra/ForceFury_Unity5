using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Back_ : MonoBehaviour {

	public Button _btnBack;
	
	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);
	}
	
	private void OnBtnBackClick() {
		Application.LoadLevel("MainMenu");
	}
}

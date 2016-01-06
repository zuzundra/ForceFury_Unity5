using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIWindowCityBarracks : UIWindow {
	[SerializeField]
	private Button _btnBack;

	[SerializeField]
	private Text _lblUnitName;

	[SerializeField]
	private Text _lblParamHealth;
	[SerializeField]
	private Text _lblParamDamage;
	[SerializeField]
	private Text _lblParamSpeed;
	[SerializeField]
	private Text _lblParamRange;

	[SerializeField]
	private Image _imgHexUnit;

	[SerializeField]
	private UIBarracksUnitInfo _unitImage;
	[SerializeField]
	private Vector2 _unitImageOffset;

	[SerializeField]
	private Button _btnSort_all;
	[SerializeField]
	private Button _btnSort_1;
	[SerializeField]
	private Button _btnSort_2;
	[SerializeField]
	private Button _btnSort_3;
	[SerializeField]
	private Button _btnSort_4;
	[SerializeField]
	private Button _btnSort_5;

	private UIBarracksUnitInfo[] _unitImages = null;
	private Dictionary<string, Sprite> _hexagonalResources = new Dictionary<string,Sprite>();

	private Vector2 _startUnitImagesPosition = Vector2.zero;

	public void Awake() {
#if UNITY_EDITOR && BARRACKS_TEST
		if (!Global.IsInitialized) {
			Global.Instance.Initialize();
		}
#endif

		AddDisplayAction(EUIWindowDisplayAction.PreShow, SetupUnits);
		AddDisplayAction(EUIWindowDisplayAction.PostShow, (UIWindow window) => { ArrangeUnitImages(); });
		AddDisplayAction(EUIWindowDisplayAction.PostHide, ClearUnits);
	}

	public void Start() {
		_btnBack.onClick.AddListener(OnBtnBackClick);

		_btnSort_all.onClick.AddListener(OnBtnSortAllClick);
		_btnSort_1.onClick.AddListener(OnBtnSort1Click);
		_btnSort_2.onClick.AddListener(OnBtnSort2Click);
		_btnSort_3.onClick.AddListener(OnBtnSort3Click);
		_btnSort_4.onClick.AddListener(OnBtnSort4Click);
		_btnSort_5.onClick.AddListener(OnBtnSort5Click);
	}

	private void SetupUnits(UIWindow window) {
		_startUnitImagesPosition = _unitImage.gameObject.GetComponent<RectTransform>().anchoredPosition;

		List<EUnitKey> playerUnitKeys = Global.Instance.Player.City.AvailableUnits;

		_unitImages = new UIBarracksUnitInfo[playerUnitKeys.Count];
		_unitImages[0] = _unitImage;
		for (int i = 0; i < playerUnitKeys.Count; i++) {
			if (i > 0) {
				_unitImages[i] = (GameObject.Instantiate(_unitImage.gameObject) as GameObject).GetComponent<UIBarracksUnitInfo>();
				_unitImages[i].transform.SetParent(_unitImage.transform.parent, false);
			}

			int iTmp = i;	//some spike: without this array.Length is passed to listener
			_unitImages[i].Button.onClick.AddListener(() => { ShowSoldierInfo(iTmp); });
			_unitImages[i].Setup(playerUnitKeys[i]);
		}
		ArrangeUnitImages();
		ShowSoldierInfo(0);
	}

	private void ClearUnits(UIWindow window) {
		for (int i = 0; i < _unitImages.Length; i++) {
			_unitImages[i].Clear();

			if (i > 0) {
				GameObject.Destroy(_unitImages[i].gameObject);
			}
		}

		_imgHexUnit.sprite = null;
		foreach (KeyValuePair<string, Sprite> kvp in _hexagonalResources) {
			UIResourcesManager.Instance.FreeResource(kvp.Key);
		}
		_hexagonalResources.Clear();
	}

	private void ShowSoldierInfo(int index) {
		//TODO: set correct name
		_lblUnitName.text = _unitImages[index].UnitData.Key.ToString();

		//TODO: set correct data (including ammunition and level-ups)
		_lblParamHealth.text = _unitImages[index].UnitData.BaseHealthPoints.ToString();
		_lblParamDamage.text = _unitImages[index].UnitData.BaseDamage.ToString();
        _lblParamSpeed.text = "No speed";//_unitImages[index].UnitData.BaseAttackSpeed.ToString();
		_lblParamRange.text = _unitImages[index].UnitData.BaseRange.ToString();//BaseAR.ToString();

		_imgHexUnit.sprite = GetHexagonalIconResource(_unitImages[index].UnitData.HexIconName);
	}

	private void ArrangeUnitImages() {
		for (int i = 0, q = 0; i < _unitImages.Length; i++) {
			if (_unitImages[i].gameObject.activeInHierarchy) {
				_unitImages[i].gameObject.GetComponent<RectTransform>().anchoredPosition = _startUnitImagesPosition + new Vector2((q % 3) * _unitImageOffset.x, Mathf.Floor(q / 3) * _unitImageOffset.y);
				q++;
			}
		}
	}

	private void SortUnits(int leadershipCost) {
		if (leadershipCost < 0) {
			for (int i = 0; i < _unitImages.Length; i++) {
				_unitImages[i].gameObject.SetActive(true);
			}
		} else {
			for (int i = 0; i < _unitImages.Length; i++) {
				_unitImages[i].gameObject.SetActive(_unitImages[i].UnitData.LeadershipCost == leadershipCost);
			}
		}

		ArrangeUnitImages();
	}

	#region listeners
	private void OnBtnBackClick() {
		Hide();
	}

	private void OnBtnSortAllClick() {
		SortUnits(-1);
	}

	private void OnBtnSort1Click() {
		SortUnits(1);
	}

	private void OnBtnSort2Click() {
		SortUnits(2);
	}

	private void OnBtnSort3Click() {
		SortUnits(3);
	}

	private void OnBtnSort4Click() {
		SortUnits(4);
	}

	private void OnBtnSort5Click() {
		SortUnits(5);
	}
	#endregion

	#region auxiliary
	private Sprite GetHexagonalIconResource(string iconPath) {
		if (iconPath != string.Empty) {
			iconPath = string.Format("{0}/{1}", GameConstants.Paths.UI_UNIT_ICONS_RESOURCES, iconPath);
			if (!_hexagonalResources.ContainsKey(iconPath)) {
				_hexagonalResources.Add(iconPath, UIResourcesManager.Instance.GetResource<Sprite>(iconPath));
			}
			return _hexagonalResources[iconPath];
		}
		return null;
	}
	#endregion
}

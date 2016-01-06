using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIWindow : MonoBehaviour {
	[SerializeField]
	protected EUIWindowKey _windowKey = EUIWindowKey.None;
	public EUIWindowKey WindowKey {
		get { return _windowKey; }
	}

    [SerializeField]
    private bool _isActive = false;
    public bool IsActive
    {
        get { return _isActive; }
    }

	private Dictionary<EUIWindowDisplayAction, Action<UIWindow>> _displayActions = null;

	public UIWindow() {
		_displayActions = new Dictionary<EUIWindowDisplayAction, Action<UIWindow>>() {
			{ EUIWindowDisplayAction.PreShow, null },
			{ EUIWindowDisplayAction.PostShow, null },
			{ EUIWindowDisplayAction.PreHide, null },
			{ EUIWindowDisplayAction.PostHide, null }
		};
	}

	~UIWindow() {
		_displayActions.Clear();
		_displayActions = null;
	}

	public void Show() {
		if (_displayActions[EUIWindowDisplayAction.PreShow] != null) {
			_displayActions[EUIWindowDisplayAction.PreShow](this);
		}

		gameObject.SetActive(true);

		if (_displayActions[EUIWindowDisplayAction.PostShow] != null) {
			_displayActions[EUIWindowDisplayAction.PostShow](this);
		}
	}

	public void Hide() {
		if (_displayActions[EUIWindowDisplayAction.PreHide] != null) {
			_displayActions[EUIWindowDisplayAction.PreHide](this);
		}

		gameObject.SetActive(false);

		if (_displayActions[EUIWindowDisplayAction.PostHide] != null) {
			_displayActions[EUIWindowDisplayAction.PostHide](this);
		}
	}

	public void AddDisplayAction(EUIWindowDisplayAction type, Action<UIWindow> act) {
		_displayActions[type] += act;
	}

	public void RemoveDisplayAction(EUIWindowDisplayAction type, Action<UIWindow> act) {
		_displayActions[type] -= act;
	}
}

using UnityEngine;
using System.Collections.Generic;

public class UIWindowsManager : MonoBehaviourSingleton<UIWindowsManager> {
	private Dictionary<EUIWindowKey, string> _windowResources = new Dictionary<EUIWindowKey, string>() {

		//перечисление всех префабов

		{ EUIWindowKey.BattlePreview, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_PREVIEW },
		{ EUIWindowKey.BattleSetup, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_SETUP },
		{ EUIWindowKey.BattleVictory, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_VICTORY },
		{ EUIWindowKey.BattleDefeat, GameConstants.Paths.Prefabs.UI_WIN_BATTLE_DEFEAT },
        { EUIWindowKey.UnitSelect, GameConstants.Paths.Prefabs.UI_WIN_UNIT_SELECT },
        { EUIWindowKey.UnitConfirm, GameConstants.Paths.Prefabs.UI_WIN_UNIT_CONFIRM },
		
		{ EUIWindowKey.CityBuildingUpgrade, GameConstants.Paths.Prefabs.UI_WIN_CITY_BUILDING_UPGRADE },
		{ EUIWindowKey.CityBarracks, GameConstants.Paths.Prefabs.UI_WIN_CITY_BARRACKS },

		{ EUIWindowKey.HeroesList, GameConstants.Paths.Prefabs.UI_WIN_HEROES_LIST },
		{ EUIWindowKey.HeroInfo, GameConstants.Paths.Prefabs.UI_WIN_HERO_INFO },

		{ EUIWindowKey.PvPModeSelect, GameConstants.Paths.Prefabs.UI_WIN_PVP_MODE_SELECT },
		{ EUIWindowKey.PvPBattleSetup, GameConstants.Paths.Prefabs.UI_WIN_PVP_BATTLE_SETUP },

		{ EUIWindowKey.Sets, GameConstants.Paths.Prefabs.UI_WIN_SETS },
		{ EUIWindowKey.Shop, GameConstants.Paths.Prefabs.UI_WIN_SHOP },
		{ EUIWindowKey.Upgrades, GameConstants.Paths.Prefabs.UI_WIN_UPGRADES },

		{ EUIWindowKey.PlanetOverlay, GameConstants.Paths.Prefabs.UI_WIN_PLANET_OVERAY },
	};

	private List<UIWindow> _activeWindows = new List<UIWindow>();
	private UIWindow _activeWindow = null;
	public UIWindow ActiveWindow {
		get { return _activeWindow; }
	}

	public T GetWindow<T>(EUIWindowKey windowKey) where T : UIWindow {
		UIWindow window = GetWindow(windowKey);
		return window != null ? window as T : null;
	}

	public UIWindow GetWindow(EUIWindowKey windowKey, Transform parentTransform) 
    {
        GameObject windowResource = UIResourcesManager.Instance.GetResource<GameObject>(_windowResources[windowKey]);
        if (windowResource != null)
        {
            UIWindow windowInstance = (GameObject.Instantiate(windowResource) as GameObject).GetComponent<UIWindow>();
            windowInstance.transform.SetParent(parentTransform, false);
            windowInstance.gameObject.SetActive(false);

            windowInstance.AddDisplayAction(EUIWindowDisplayAction.PreShow, OnWindowShow);
            windowInstance.AddDisplayAction(EUIWindowDisplayAction.PreHide, OnWindowHide);

            return windowInstance;
        }
        return null;
    }

    public UIWindow GetWindow(EUIWindowKey windowKey)
    {
        int windowIndex = _activeWindows.FindIndex((UIWindow w) => { return w.WindowKey == windowKey; });
        if (windowIndex != -1)
        {
            return _activeWindows[windowIndex];
        }
        GameObject windowResource = UIResourcesManager.Instance.GetResource<GameObject>(_windowResources[windowKey]);
        if (windowResource != null)
        {
            UIWindow windowInstance = (GameObject.Instantiate(windowResource) as GameObject).GetComponent<UIWindow>();
            windowInstance.transform.SetParent(Utils.UI.GetWindowsCanvas().transform, false);
            windowInstance.gameObject.SetActive(false);
            windowInstance.AddDisplayAction(EUIWindowDisplayAction.PreShow, OnWindowShow);
            windowInstance.AddDisplayAction(EUIWindowDisplayAction.PreHide, OnWindowHide);

            _activeWindows.Add(windowInstance);

            return windowInstance;
        }
        return null;
    }

	public void Clear() {
		for (int i = 0; i < _activeWindows.Count; i++) {
			if (_activeWindows[i] != null) {
				UIResourcesManager.Instance.FreeResource(_activeWindows[i].gameObject);
				GameObject.Destroy(_activeWindows[i].gameObject);

			}
		}
		_activeWindows.Clear();
	}

	#region listeners
	private void OnWindowShow(UIWindow window) {
		if (_activeWindow == window) {
			return;
		}
        if (_activeWindow != null && _activeWindow != window)
        {
            if (!_activeWindow.IsActive)
                _activeWindow.gameObject.SetActive(false);
        }
        int windowIndex = _activeWindows.FindIndex((UIWindow w) => { return w.WindowKey == window.WindowKey; });
        if (windowIndex != -1)
        {
            _activeWindows.RemoveAt(windowIndex);
            _activeWindows.Insert(0, window);
        }
		_activeWindow = window;
	}

	private void OnWindowHide(UIWindow window) {
		window.RemoveDisplayAction(EUIWindowDisplayAction.PreShow, OnWindowShow);
		window.RemoveDisplayAction(EUIWindowDisplayAction.PreHide, OnWindowHide);

		if (_activeWindow == window) {
			_activeWindow = null;
		}

		int windowIndex = _activeWindows.FindIndex((UIWindow w) => { return w.WindowKey == window.WindowKey; });
		if (windowIndex != -1) {
			_activeWindows.RemoveAt(windowIndex);

			UIResourcesManager.Instance.FreeResource(window.gameObject);
			GameObject.Destroy(window.gameObject);

			if (windowIndex == 0 && _activeWindows.Count > 0) {
				_activeWindows[0].gameObject.SetActive(true);
				_activeWindow = _activeWindows[0];
			}
		}
	}
	#endregion

	#region unity funcs
	public void OnLevelWasLoaded(int levelNumber) {
		Clear();
	}

	public void OnApplicationQuit() {
		Clear();
	}
	#endregion
}

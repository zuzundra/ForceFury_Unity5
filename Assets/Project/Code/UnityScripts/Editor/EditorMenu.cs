#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class EditorMenu : MonoBehaviour {
	[MenuItem ("HCClone/Config/Items config")]
	public static void InstantiateItemsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab <ItemsConfig>();
	}

	[MenuItem("HCClone/Config/Units config")]
	public static void InstantiateUnitsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<UnitsConfig>();
	}

	[MenuItem("HCClone/Config/Skills config")]
	public static void InstantiateSkillsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<SkillsConfig>();
	}

	[MenuItem("HCClone/Config/Missions config")]
	public static void InstantiateMissionsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<MissionsConfig>();
	}

	[MenuItem("HCClone/Config/City config")]
	public static void InstantiateCityConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<CityConfig>();
	}

	[MenuItem("HCClone/Config/Fight dialogs config")]
	public static void InstantiateFightDialogsConfig() {
		Selection.activeObject = InstantiateSingletonPrefab<UnitDialogs>();
	}

	private static GameObject InstantiateSingletonPrefab<T>() where T : MonoBehaviour {
		GameObject goData = null;
		T data = (T)FindObjectOfType(typeof(T));
		if (data == null) {
			string path = typeof(T).GetField("_path", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null).ToString();
			goData = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(path) as GameObject) as GameObject;
		} else {
			goData = data.gameObject;
		}

		return goData;
	}
}
#endif
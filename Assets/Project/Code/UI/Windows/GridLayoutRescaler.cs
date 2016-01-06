using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridLayoutRescaler : MonoBehaviour {

	GridLayoutGroup gridLayoutGroup;
	RectTransform rect;
	public float height;
	public int cellCount = 5;
	
	void Start ()
	{
		gridLayoutGroup = GetComponent<GridLayoutGroup> ();
		rect = GetComponent<RectTransform> ();
		
		gridLayoutGroup.cellSize = new Vector2 (rect.rect.height, rect.rect.height);
		cellCount = GetComponentsInChildren<RectTransform> ().Length;
	}
	
	void OnRectTransformDimensionsChange ()
	{
		if (gridLayoutGroup != null && rect != null)
			if ((rect.rect.height + (gridLayoutGroup.padding.horizontal * 2)) * cellCount < rect.rect.width)
				gridLayoutGroup.cellSize = new Vector2 (rect.rect.height, rect.rect.height);
	}
}

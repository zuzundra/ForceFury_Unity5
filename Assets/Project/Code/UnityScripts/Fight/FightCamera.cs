using UnityEngine;

public static class FightCamera {
	//TODO: finalize this stuff
	public static void AdaptMain() {
		float w = 640f;
		float h = 480f;
		//float y = 4.7f;
		float s = 4.25f;

		float diffWidth = w / Screen.width;
		float diffHeight = h / Screen.height;
		float diffRatio = diffWidth / diffHeight;

		Camera.main.orthographicSize = s * diffRatio;

		//old way
		//float w = 520f;
		//float h = 540f;
		//float y = 7f;
		//float s = 5f;

		//float diffW = (Screen.width / w) / (Screen.height / h);

		//float newY = y;
		//if (diffW < 1f) {
		//	//newY = y;
		//	newY = y / diffW;
		//	newY = newY - (newY - y) * 0.33f;
		//} else {
		//	newY = y / diffW;
		//	newY = newY + (y - newY) * 0.33f;
		//}
		//float newS = s / diffW;

		//Camera.main.orthographicSize = newS;
		//Vector3 cameraPos = Camera.main.transform.position;
		//cameraPos.y = newY;
		//Camera.main.transform.position = cameraPos;
	}

	public static void AdaptCanvas(float defaultWidth, Canvas canvasBG) {
		canvasBG.scaleFactor = Screen.width / defaultWidth;
	}

	public static void AdaptDialog(float defaultWidth, Transform dialogRoot) {
		float scale = Screen.width / defaultWidth;
		dialogRoot.localScale = new Vector3(scale, scale, scale);
	}
}

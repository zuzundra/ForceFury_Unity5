using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CBMarketData : CBBaseData {
	private EItemKey[] _availableItems = new EItemKey[0];	//TODO: setup available items list
	public EItemKey[] AvailableItems {
		get { return _availableItems; }
	}
}

using UnityEngine;

[System.Serializable]
public class ItemPriceData {
	[SerializeField]
	private int _buyCreditsCost = 0;
	public int BuyCreditsCost {
		get { return _buyCreditsCost; }
	}

	[SerializeField]
	private int _sellCreditsCost = 0;
	public int SellCreditsCost {
		get { return _sellCreditsCost; }
	}

	[SerializeField]
	private int _buyMineralsCost = 0;
	public int BuyMineralsCost {
		get { return _buyMineralsCost; }
	}

	[SerializeField]
	private int _sellMineralsCost = 0;
	public int SellMineralsCost {
		get { return _sellMineralsCost; }
	}
}

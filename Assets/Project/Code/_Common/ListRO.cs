using System.Collections.Generic;

public class ListRO<T> {
	private List<T> _data;

	public ListRO(List<T> data) {
		_data = data;
	}

	public T this[int i] {
		get { return _data[i]; }
	}

	public int Count {
		get { return _data.Count; }
	}
}
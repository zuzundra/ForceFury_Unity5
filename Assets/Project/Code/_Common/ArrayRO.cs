public class ArrayRO<T> {
	private T[] _data;

	public ArrayRO(T[] data) {
		_data = data;
	}

	public T this[int i] {
		get { return _data[i]; }
	}

	public int Length {
		get { return _data.Length; }
	}

	public T[] DataCopy {
		get {
			T[] dataCopy = new T[_data.Length];
			for (int i = 0; i < _data.Length; i++) {
				dataCopy[i] = _data[i];
			}
			return dataCopy;
		}
	}
}
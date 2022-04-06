using System;

[Serializable]
public struct WeightedValue<T> {
	public T value;
	public float weight;
}
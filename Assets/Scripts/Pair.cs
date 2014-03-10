using UnityEngine;

public class Pair<T, U> {
	public Pair() {
	}
	
	public Pair(T first, U second) {
		this.First = first;
		this.Second = second;
	}
	
	public T First { get; set; }
	public U Second { get; set; }

	public override string ToString() {
		return string.Format("Pair({0}, {1})", First, Second);
	}
	
	public override int GetHashCode() {
		int hash = 17;
		hash = hash * 23 + First.GetHashCode();
		hash = hash * 23 + Second.GetHashCode();
		return hash;
	}
	
	public override bool Equals(object o) {
		if (o.GetType() != typeof(Pair<T, U>)) {
			return false;
		}

		var other = (Pair<T, U>) o;
	
		return this == other;
	}
	
	public static bool operator==(Pair<T, U> a, Pair<T, U> b) {
		if (System.Object.ReferenceEquals(a, b)) {
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))	{
			return false;
		}

		return a.First.Equals(b.First) && a.Second.Equals(b.Second);            
	}
	
	public static bool operator!=(Pair<T, U> a, Pair<T, U> b) {
		return !(a == b);
	}
}
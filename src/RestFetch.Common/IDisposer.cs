namespace RestFetch;

public interface IDisposer<T> {
	static abstract void Dispose(ref T instance);
}
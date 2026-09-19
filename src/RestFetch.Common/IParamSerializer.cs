namespace RestFetch;

// public interface IParamSerializer<in T> {
// 	static abstract string Serialize(string[] aliasArray, string[] urlArray, T value);
// }

public interface IParamSerializer<in TValue, out TReturn> {
	static abstract TReturn Serialize(string aliasKey, TValue value);
}

public interface IParamEncodeSerializer<in TValue, out TReturn> {
	static abstract TReturn Encode(string aliasKey, TValue value);
}
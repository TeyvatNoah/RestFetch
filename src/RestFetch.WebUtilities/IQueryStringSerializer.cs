namespace RestFetch.WebUtilities;

// 源生成器需要先检测是否ValueOnly, false意味着有key
// 其次需要检测参数是否可空,根据是否可空决定调用哪个接口的方法
// QS, Param, Header, Body必须在request之前被序列化完成

// 无key且检测参数可空优先这个
// 有key则不会调用此方法
// 用作内部使用,用作用户实现,用作类方法
// 无key且value非null时优先调用
public interface IQueryStringSerializer<in T> where T: allows ref struct {
	#pragma warning disable CA1000
	static abstract string Serialize(T value);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);
	#pragma warning restore CA1000
}

// 有key则key必不为null,因为都是编译时确定
// 无key则不会调用此方法
// 用作内部使用,用作用户实现,不用作类方法
// 有key且value可空时调用
public interface IAliasQueryStringSerializer<in T> where T: allows ref struct {
	// TODO 考虑此方法private或internal
	static abstract string Serialize(string aliasKey, T value);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);
}

// 不用作内部使用,用作用户实现,用作类方法
public interface IQueryStringSerializer {
	// 如果T是字面量null则无法自动推导,不过源生成器会保证不传字面量null
	static abstract string Serialize<T>(T? value) where T: allows ref struct;
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);
}

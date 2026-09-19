namespace RestFetch;

public interface IRequestIdGenerator {
	static abstract RequestId NextId();
}
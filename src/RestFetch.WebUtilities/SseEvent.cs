using System.Runtime.CompilerServices;

namespace RestFetch.WebUtilities;

public readonly struct SseEvent<T>(T dat): IEquatable<SseEvent<T>> {
	public readonly string? Id;

	public readonly string? Event;

	public readonly TimeSpan? Retry;

	public readonly T Data = dat;

	// public readonly Exception? Error;

	public SseEvent(string? id, string? @event, long? retry, T data, Exception? error = null): this(data) {
		Id = id;
		Event = @event;
		Retry = retry is long r ? TimeSpan.FromMilliseconds(r) : null;
		// Error = error;
	}

	public SseEvent(string? id, string? @event, TimeSpan? retry, T data, Exception? error = null): this(data) {
		Id = id;
		Event = @event;
		Retry = retry;
		// Error = error;
	}

	public readonly override string ToString() => Data?.ToString() ?? "";

	public readonly override bool Equals(object? obj) => obj is SseEvent<T> o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(SseEvent<T> obj) => this == obj;

	public readonly override int GetHashCode() => (Data, Id, Event).GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(SseEvent<T> left, SseEvent<T> right) => Equals(left.Data, right.Data) && left.Id == right.Id && left.Event == right.Event;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(SseEvent<T> left, SseEvent<T> right) => !(left == right);
}

public readonly struct SseEvent(string dat): IEquatable<SseEvent> {
	public readonly string? Id;

	public readonly string? Event;

	public readonly TimeSpan? Retry;

	public readonly string Data = dat;

	// public readonly Exception? Error;

	public SseEvent(string? id, string? @event, long? retry, string data, Exception? error = null): this(data) {
		Id = id;
		Event = @event;
		Retry = retry is long r ? TimeSpan.FromMilliseconds(r) : null;
		// Error = error;
	}

	public SseEvent(string? id, string? @event, TimeSpan? retry, string data, Exception? error = null): this(data) {
		Id = id;
		Event = @event;
		Retry = retry;
		// Error = error;
	}

	public readonly override string ToString() => Data.ToString();

	public readonly override bool Equals(object? obj) => obj is SseEvent o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(SseEvent obj) => this == obj;

	public readonly override int GetHashCode() => (Data, Id, Event).GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(SseEvent left, SseEvent right) => left.Data == right.Data && left.Id == right.Id && left.Event == right.Event;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(SseEvent left, SseEvent right) => !(left == right);
}

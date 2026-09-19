using System.Runtime.CompilerServices;

namespace RestFetch;

public struct URLProxy(string url): IURL, IEquatable<URLProxy> {
	public string URL { get; init; } = url;

	public Uri? Uri;

	public readonly override string ToString() => URL;
	public readonly override bool Equals(object? obj) => URL == (string?)obj;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(URLProxy obj) => URL == obj.URL;

	public readonly override int GetHashCode() => URL.GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(URLProxy left, URLProxy right) => left.URL == right.URL;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(URLProxy left, URLProxy right) => left.URL != right.URL;
}
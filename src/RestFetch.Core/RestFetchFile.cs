using System.Runtime.CompilerServices;

namespace RestFetch;

public readonly struct RestFetchFileInfo(
	string name,
	string fileName,
	FileInfo file
): IEquatable<RestFetchFileInfo> {
	public readonly string Name = name;

	public readonly string  FileName = fileName;

	public readonly FileInfo File = file;

	public RestFetchFileInfo(FileInfo fileInfo): this(
		Path.GetFileNameWithoutExtension(fileInfo.Name),
		fileInfo.Name,
		fileInfo
	) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator FileInfo(RestFetchFileInfo restFetchFileInfo) => restFetchFileInfo.File;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator RestFetchFileInfo(FileInfo fileInfo) => new(fileInfo);

	public readonly override string ToString() => FileName;

	public readonly override bool Equals(object? obj) => obj is RestFetchFileInfo o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(RestFetchFileInfo obj) => this == obj;

	public readonly override int GetHashCode() => HashCode.Combine(Name, FileName, File);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(RestFetchFileInfo left, RestFetchFileInfo right) => left.Name == right.Name && left.FileName == right.FileName && left.File == right.File;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(RestFetchFileInfo left, RestFetchFileInfo right) => !(left == right);
}
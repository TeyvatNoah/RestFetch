#nullable enable

using System.Globalization;
using System.Runtime.CompilerServices;

using Cysharp.Text;

namespace RestFetch.WebUtilities;


public partial class QueryString<TEncoder, TDecoder>:
	// for well known key-value collection
	IQueryStringSerializer<string>,
	IAliasQueryStringSerializer<string>,
	IAliasQueryStringSerializer<IEnumerable<string>>,
	IAliasQueryStringSerializer<string?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, string?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, string?>>>,
	IQueryStringSerializer<sbyte>,
	IAliasQueryStringSerializer<sbyte>,
	IQueryStringSerializer<sbyte?>,
	IAliasQueryStringSerializer<sbyte?>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, sbyte>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, sbyte?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, sbyte>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, sbyte?>>>,
	IQueryStringSerializer<short>,
	IAliasQueryStringSerializer<short>,
	IQueryStringSerializer<short?>,
	IAliasQueryStringSerializer<short?>,
	IAliasQueryStringSerializer<IEnumerable<short>>,
	IAliasQueryStringSerializer<IEnumerable<short?>>,
	IAliasQueryStringSerializer<short[]>,
	IAliasQueryStringSerializer<short?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, short>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, short?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, short>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, short?>>>,
	IQueryStringSerializer<int>,
	IAliasQueryStringSerializer<int>,
	IQueryStringSerializer<int?>,
	IAliasQueryStringSerializer<int?>,
	IAliasQueryStringSerializer<IEnumerable<int>>,
	IAliasQueryStringSerializer<IEnumerable<int?>>,
	IAliasQueryStringSerializer<int[]>,
	IAliasQueryStringSerializer<int?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, int>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, int?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, int>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, int?>>>,
	IQueryStringSerializer<long>,
	IAliasQueryStringSerializer<long>,
	IQueryStringSerializer<long?>,
	IAliasQueryStringSerializer<long?>,
	IAliasQueryStringSerializer<IEnumerable<long>>,
	IAliasQueryStringSerializer<IEnumerable<long?>>,
	IAliasQueryStringSerializer<long[]>,
	IAliasQueryStringSerializer<long?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, long>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, long?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, long>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, long?>>>,
	IQueryStringSerializer<byte>,
	IAliasQueryStringSerializer<byte>,
	IQueryStringSerializer<byte?>,
	IAliasQueryStringSerializer<byte?>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, byte>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, byte?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, byte>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, byte?>>>,
	IQueryStringSerializer<ushort>,
	IAliasQueryStringSerializer<ushort>,
	IQueryStringSerializer<ushort?>,
	IAliasQueryStringSerializer<ushort?>,
	IAliasQueryStringSerializer<IEnumerable<ushort>>,
	IAliasQueryStringSerializer<IEnumerable<ushort?>>,
	IAliasQueryStringSerializer<ushort[]>,
	IAliasQueryStringSerializer<ushort?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, ushort>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, ushort?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, ushort>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, ushort?>>>,
	IQueryStringSerializer<uint>,
	IAliasQueryStringSerializer<uint>,
	IQueryStringSerializer<uint?>,
	IAliasQueryStringSerializer<uint?>,
	IAliasQueryStringSerializer<IEnumerable<uint>>,
	IAliasQueryStringSerializer<IEnumerable<uint?>>,
	IAliasQueryStringSerializer<uint[]>,
	IAliasQueryStringSerializer<uint?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, uint>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, uint?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, uint>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, uint?>>>,
	IQueryStringSerializer<ulong>,
	IAliasQueryStringSerializer<ulong>,
	IQueryStringSerializer<ulong?>,
	IAliasQueryStringSerializer<ulong?>,
	IAliasQueryStringSerializer<IEnumerable<ulong>>,
	IAliasQueryStringSerializer<IEnumerable<ulong?>>,
	IAliasQueryStringSerializer<ulong[]>,
	IAliasQueryStringSerializer<ulong?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, ulong>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, ulong?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, ulong>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, ulong?>>>,
	IQueryStringSerializer<float>,
	IAliasQueryStringSerializer<float>,
	IQueryStringSerializer<float?>,
	IAliasQueryStringSerializer<float?>,
	IAliasQueryStringSerializer<IEnumerable<float>>,
	IAliasQueryStringSerializer<IEnumerable<float?>>,
	IAliasQueryStringSerializer<float[]>,
	IAliasQueryStringSerializer<float?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, float>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, float?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, float>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, float?>>>,
	IQueryStringSerializer<double>,
	IAliasQueryStringSerializer<double>,
	IQueryStringSerializer<double?>,
	IAliasQueryStringSerializer<double?>,
	IAliasQueryStringSerializer<IEnumerable<double>>,
	IAliasQueryStringSerializer<IEnumerable<double?>>,
	IAliasQueryStringSerializer<double[]>,
	IAliasQueryStringSerializer<double?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, double>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, double?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, double>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, double?>>>,
	IQueryStringSerializer<decimal>,
	IAliasQueryStringSerializer<decimal>,
	IQueryStringSerializer<decimal?>,
	IAliasQueryStringSerializer<decimal?>,
	IAliasQueryStringSerializer<IEnumerable<decimal>>,
	IAliasQueryStringSerializer<IEnumerable<decimal?>>,
	IAliasQueryStringSerializer<decimal[]>,
	IAliasQueryStringSerializer<decimal?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, decimal>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, decimal?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, decimal>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, decimal?>>>,
	IQueryStringSerializer<bool>,
	IAliasQueryStringSerializer<bool>,
	IQueryStringSerializer<bool?>,
	IAliasQueryStringSerializer<bool?>,
	IAliasQueryStringSerializer<IEnumerable<bool>>,
	IAliasQueryStringSerializer<IEnumerable<bool?>>,
	IAliasQueryStringSerializer<bool[]>,
	IAliasQueryStringSerializer<bool?[]>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, bool>>>,
	IQueryStringSerializer<IEnumerable<KeyValuePair<string, bool?>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, bool>>>,
	IAliasQueryStringSerializer<IEnumerable<KeyValuePair<string, bool?>>>,
	// for self
	ICollection<KeyValuePair<string, string>>
	{

	// sbyte
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(sbyte value) => value.ToString(CultureInfo.InvariantCulture);

	// sbyte?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(sbyte? value) => value is sbyte v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// short
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(short value) => value.ToString(CultureInfo.InvariantCulture);

	// short?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(short? value) => value is short v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// int
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(int value) => value.ToString(CultureInfo.InvariantCulture);

	// int?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(int? value) => value is int v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// long
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(long value) => value.ToString(CultureInfo.InvariantCulture);

	// long?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(long? value) => value is long v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// byte
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(byte value) => value.ToString(CultureInfo.InvariantCulture);

	// byte?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(byte? value) => value is byte v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// ushort
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(ushort value) => value.ToString(CultureInfo.InvariantCulture);

	// ushort?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(ushort? value) => value is ushort v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// uint
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(uint value) => value.ToString(CultureInfo.InvariantCulture);

	// uint?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(uint? value) => value is uint v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// ulong
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(ulong value) => value.ToString(CultureInfo.InvariantCulture);

	// ulong?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(ulong? value) => value is ulong v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// float
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(float value) => value.ToString(CultureInfo.InvariantCulture);

	// float?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(float? value) => value is float v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// double
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(double value) => value.ToString(CultureInfo.InvariantCulture);

	// double?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(double? value) => value is double v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// decimal
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(decimal value) => value.ToString(CultureInfo.InvariantCulture);

	// decimal?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(decimal? value) => value is decimal v ? v.ToString(CultureInfo.InvariantCulture) : "";

	// sbyte
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, sbyte value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// sbyte?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, sbyte? value) => value is sbyte v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// short
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, short value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// short?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, short? value) => value is short v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// int
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, int value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// int?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, int? value) => value is int v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// long
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, long value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// long?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, long? value) => value is long v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// byte
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, byte value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// byte?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, byte? value) => value is byte v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// ushort
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, ushort value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// ushort?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, ushort? value) => value is ushort v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// uint
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, uint value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// uint?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, uint? value) => value is uint v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// ulong
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, ulong value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// ulong?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, ulong? value) => value is ulong v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// float
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, float value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// float?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, float? value) => value is float v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// double
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, double value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// double?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, double? value) => value is double v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// decimal
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, decimal value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// decimal?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, decimal? value) => value is decimal v ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v) : "";

	// sbyte[]
	public static string Serialize(string aliasKey, sbyte[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// sbyte?[]
	public static string Serialize(string aliasKey, sbyte?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is sbyte v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is sbyte vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// short[]
	public static string Serialize(string aliasKey, short[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// short?[]
	public static string Serialize(string aliasKey, short?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is short v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is short vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// int[]
	public static string Serialize(string aliasKey, int[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// int?[]
	public static string Serialize(string aliasKey, int?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is int v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is int vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// long[]
	public static string Serialize(string aliasKey, long[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// long?[]
	public static string Serialize(string aliasKey, long?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is long v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is long vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// byte[]
	public static string Serialize(string aliasKey, byte[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// byte?[]
	public static string Serialize(string aliasKey, byte?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is byte v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is byte vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// ushort[]
	public static string Serialize(string aliasKey, ushort[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// ushort?[]
	public static string Serialize(string aliasKey, ushort?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is ushort v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is ushort vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// uint[]
	public static string Serialize(string aliasKey, uint[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// uint?[]
	public static string Serialize(string aliasKey, uint?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is uint v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is uint vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// ulong[]
	public static string Serialize(string aliasKey, ulong[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// ulong?[]
	public static string Serialize(string aliasKey, ulong?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is ulong v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is ulong vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// float[]
	public static string Serialize(string aliasKey, float[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// float?[]
	public static string Serialize(string aliasKey, float?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is float v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is float vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// double[]
	public static string Serialize(string aliasKey, double[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// double?[]
	public static string Serialize(string aliasKey, double?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is double v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is double vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// decimal[]
	public static string Serialize(string aliasKey, decimal[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, array[0]));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(array[0]);
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, array[i]));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(array[i]);
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// decimal?[]
	public static string Serialize(string aliasKey, decimal?[] array) {
		using var sb = ZString.CreateStringBuilder(true);

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is decimal v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(v);
			}
			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is decimal vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(vv);
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// IEnumerable<sbyte>
	public static string Serialize(string aliasKey, IEnumerable<sbyte> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<sbyte?>
	public static string Serialize(string aliasKey, IEnumerable<sbyte?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is sbyte v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is sbyte vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<short>
	public static string Serialize(string aliasKey, IEnumerable<short> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<short?>
	public static string Serialize(string aliasKey, IEnumerable<short?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is short v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is short vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<int>
	public static string Serialize(string aliasKey, IEnumerable<int> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<int?>
	public static string Serialize(string aliasKey, IEnumerable<int?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is int v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is int vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<long>
	public static string Serialize(string aliasKey, IEnumerable<long> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<long?>
	public static string Serialize(string aliasKey, IEnumerable<long?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is long v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is long vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<byte>
	public static string Serialize(string aliasKey, IEnumerable<byte> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<byte?>
	public static string Serialize(string aliasKey, IEnumerable<byte?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is byte v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is byte vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<ushort>
	public static string Serialize(string aliasKey, IEnumerable<ushort> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<ushort?>
	public static string Serialize(string aliasKey, IEnumerable<ushort?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is ushort v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is ushort vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<uint>
	public static string Serialize(string aliasKey, IEnumerable<uint> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<uint?>
	public static string Serialize(string aliasKey, IEnumerable<uint?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is uint v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is uint vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<ulong>
	public static string Serialize(string aliasKey, IEnumerable<ulong> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<ulong?>
	public static string Serialize(string aliasKey, IEnumerable<ulong?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is ulong v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is ulong vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<float>
	public static string Serialize(string aliasKey, IEnumerable<float> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<float?>
	public static string Serialize(string aliasKey, IEnumerable<float?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is float v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is float vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<double>
	public static string Serialize(string aliasKey, IEnumerable<double> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<double?>
	public static string Serialize(string aliasKey, IEnumerable<double?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is double v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is double vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<decimal>
	public static string Serialize(string aliasKey, IEnumerable<decimal> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, iterator.Current));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(iterator.Current);
		}

		return sb.ToString();
	}

	// IEnumerable<decimal?>
	public static string Serialize(string aliasKey, IEnumerable<decimal?> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is decimal v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is decimal vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, vv));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, sbyte>>
	public static string Serialize(IEnumerable<KeyValuePair<string, sbyte>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, sbyte?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, sbyte?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is sbyte v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is sbyte vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, sbyte>>/IEnumerable<KeyValuePair<string, sbyte>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, sbyte>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, sbyte?>>/IEnumerable<KeyValuePair<string, sbyte?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, sbyte?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, short>>
	public static string Serialize(IEnumerable<KeyValuePair<string, short>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, short?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, short?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is short v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is short vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, short>>/IEnumerable<KeyValuePair<string, short>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, short>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, short?>>/IEnumerable<KeyValuePair<string, short?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, short?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, int>>
	public static string Serialize(IEnumerable<KeyValuePair<string, int>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, int?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, int?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is int v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is int vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, int>>/IEnumerable<KeyValuePair<string, int>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, int>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, int?>>/IEnumerable<KeyValuePair<string, int?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, int?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, long>>
	public static string Serialize(IEnumerable<KeyValuePair<string, long>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, long?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, long?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is long v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is long vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, long>>/IEnumerable<KeyValuePair<string, long>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, long>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, long?>>/IEnumerable<KeyValuePair<string, long?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, long?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, byte>>
	public static string Serialize(IEnumerable<KeyValuePair<string, byte>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, byte?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, byte?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is byte v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is byte vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, byte>>/IEnumerable<KeyValuePair<string, byte>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, byte>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, byte?>>/IEnumerable<KeyValuePair<string, byte?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, byte?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, ushort>>
	public static string Serialize(IEnumerable<KeyValuePair<string, ushort>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, ushort?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, ushort?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is ushort v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is ushort vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, ushort>>/IEnumerable<KeyValuePair<string, ushort>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, ushort>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, ushort?>>/IEnumerable<KeyValuePair<string, ushort?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, ushort?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, uint>>
	public static string Serialize(IEnumerable<KeyValuePair<string, uint>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, uint?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, uint?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is uint v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is uint vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, uint>>/IEnumerable<KeyValuePair<string, uint>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, uint>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, uint?>>/IEnumerable<KeyValuePair<string, uint?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, uint?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, ulong>>
	public static string Serialize(IEnumerable<KeyValuePair<string, ulong>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, ulong?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, ulong?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is ulong v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is ulong vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, ulong>>/IEnumerable<KeyValuePair<string, ulong>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, ulong>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, ulong?>>/IEnumerable<KeyValuePair<string, ulong?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, ulong?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, float>>
	public static string Serialize(IEnumerable<KeyValuePair<string, float>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, float?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, float?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is float v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is float vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, float>>/IEnumerable<KeyValuePair<string, float>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, float>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, float?>>/IEnumerable<KeyValuePair<string, float?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, float?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, double>>
	public static string Serialize(IEnumerable<KeyValuePair<string, double>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, double?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, double?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is double v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is double vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, double>>/IEnumerable<KeyValuePair<string, double>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, double>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, double?>>/IEnumerable<KeyValuePair<string, double?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, double?>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, decimal>>
	public static string Serialize(IEnumerable<KeyValuePair<string, decimal>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, value));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, innerValue));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(innerValue);
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, decimal?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, decimal?>> collection) {
		using var sb = ZString.CreateStringBuilder(true);
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is decimal v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, v));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(v);
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is decimal vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, vv));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(vv);
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, decimal>>/IEnumerable<KeyValuePair<string, decimal>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, decimal>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, decimal?>>/IEnumerable<KeyValuePair<string, decimal?>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, decimal?>> collection) => Serialize(collection);
}
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

namespace RestFetch;

public sealed class ParamSerializer :
	IParamSerializer<byte, byte>,
	IParamSerializer<sbyte, sbyte>,
	IParamSerializer<short, short>,
	IParamSerializer<ushort, ushort>,
	IParamSerializer<int, int>,
	IParamSerializer<uint, uint>,
	IParamSerializer<long, long>,
	IParamSerializer<ulong, ulong>,
	IParamSerializer<float, float>,
	IParamSerializer<double, double>,
	IParamSerializer<decimal, decimal>,
	IParamSerializer<bool, bool>,
	IParamSerializer<string, string>,
	IParamEncodeSerializer<byte, byte>,
	IParamEncodeSerializer<sbyte, sbyte>,
	IParamEncodeSerializer<short, short>,
	IParamEncodeSerializer<ushort, ushort>,
	IParamEncodeSerializer<int, int>,
	IParamEncodeSerializer<uint, uint>,
	IParamEncodeSerializer<long, long>,
	IParamEncodeSerializer<ulong, ulong>,
	IParamEncodeSerializer<float, float>,
	IParamEncodeSerializer<double, double>,
	IParamEncodeSerializer<decimal, decimal>,
	IParamEncodeSerializer<bool, bool>,
	IParamEncodeSerializer<string, string> {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Serialize(string aliasKey, byte value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sbyte Serialize(string aliasKey, sbyte value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Serialize(string aliasKey, short value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Serialize(string aliasKey, ushort value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Serialize(string aliasKey, int value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Serialize(string aliasKey, uint value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Serialize(string aliasKey, long value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Serialize(string aliasKey, ulong value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Serialize(string aliasKey, float value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Serialize(string aliasKey, double value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Serialize(string aliasKey, decimal value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Serialize(string aliasKey, bool value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, string value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Encode(string aliasKey, byte value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sbyte Encode(string aliasKey, sbyte value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Encode(string aliasKey, short value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort Encode(string aliasKey, ushort value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Encode(string aliasKey, int value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Encode(string aliasKey, uint value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Encode(string aliasKey, long value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong Encode(string aliasKey, ulong value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Encode(string aliasKey, float value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Encode(string aliasKey, double value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Encode(string aliasKey, decimal value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Encode(string aliasKey, bool value) => value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Encode(string aliasKey, string value) => UrlEncoder.Default.Encode(value);
}

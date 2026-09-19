using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestFetch;

public sealed class EmptyStringToSByteConverter: JsonConverter<sbyte?> {
	public override sbyte? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetSByte(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, sbyte? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToUshortConverter: JsonConverter<ushort?> {
	public override ushort? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetUInt16(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, ushort? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToUintConverter: JsonConverter<uint?> {
	public override uint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetUInt32(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, uint? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToUlongConverter: JsonConverter<ulong?> {
	public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetUInt64(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToByteConverter: JsonConverter<byte?> {
	public override byte? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetByte(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, byte? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToShortConverter: JsonConverter<short?> {
	public override short? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetInt16(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, short? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToIntConverter: JsonConverter<int?> {
	public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetInt32(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToLongConverter: JsonConverter<long?> {
	public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetInt64(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToDecimalConverter: JsonConverter<decimal?> {
	public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetDecimal(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToFloatConverter: JsonConverter<float?> {
	public override float? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetSingle(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, float? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToDoubleConverter: JsonConverter<double?> {
	public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TryGetDouble(out var result) ? result : null;

	public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}

public sealed class EmptyStringToBoolConverter: JsonConverter<bool?> {
	public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType == JsonTokenType.Null ? null : reader.GetBoolean();

	public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options) => writer.WriteStringValue(value is null ? "null" : Convert.ToString(value, CultureInfo.InvariantCulture));
}
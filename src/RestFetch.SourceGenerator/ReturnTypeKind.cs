namespace RestFetch.SourceGenerator;

public enum ReturnTypeKind {
	Void,
	Other,
	SystemString,
	RawResponse,
	Stream,
	JsonNode,
	JsonDocument,
	SseEvent,
	// SseEvent<TNotString>
	SseEventT,
	// SseEvent<string>
	SseEventString,
	// SseEvent<JsonNode>
	SseEventJsonNode,
	// SseEvent<JsonDocument>
	SseEventJsonDocument,
	// SseItem<TNotString>
	SseItemT,
	// SseItem<string>
	SseItemString,
	// SseItem<JsonNode>
	SseItemJsonNode,
	// SseItem<JsonDocument>
	SseItemJsonDocument,
	ApiResponseT,
	ApiResponseString,
	ApiResponseJsonNode,
	ApiResponseJsonDocument,
}
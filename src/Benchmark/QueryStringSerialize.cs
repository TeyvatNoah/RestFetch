using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

using RestFetch.WebUtilities;

using SixTatami.Utilities;

using KV = (string key, string value);

namespace TODO.Benchmark;

// [SimpleJob(RuntimeMoniker.Net80)]
// [SimpleJob(RuntimeMoniker.NativeAot80)]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 3)]
[SimpleJob(RuntimeMoniker.NativeAot80, launchCount:3)]
[AsciiDocExporter]
// [HtmlExporter]
// [CsvExporter]
// [MarkdownExporterAttribute.GitHub]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class QueryStringSerialize {
	public static string CharsetShouldEncode = "各种文字符号用来测试需要编码比较随机大小合适";

	public static CharsetType CharsetShouldNotEncode = CharsetType.AlphabetLower | CharsetType.AlphabetUpper | CharsetType.Numbers;

	public static KV[] KVarray3ShouldNotEncode = new KV[3];

	public static KV[] KVarray8ShouldNotEncode = new KV[8];

	public static KV[] KVarray20ShouldNotEncode = new KV[20];

	public static KV[] KVarray3ShouldEncode = new KV[3];

	public static KV[] KVarray8ShouldEncode = new KV[8];

	public static KV[] KVarray20ShouldEncode = new KV[20];

	public static KeyValuePair<string, string[]> KVWithArray = new(RandomString.GenerateFast(CharsetShouldNotEncode, 8), [
		RandomString.GenerateFast(CharsetShouldNotEncode, 4),
		RandomString.GenerateFast(CharsetShouldNotEncode, 4),
		RandomString.GenerateFast(CharsetShouldNotEncode, 4),
		RandomString.GenerateFast(CharsetShouldNotEncode, 4),
		RandomString.GenerateFast(CharsetShouldNotEncode, 4),
	]);

	static QueryStringSerialize() {
		for (var i = 0; i < 3; ++i) {
			KVarray3ShouldEncode[i] = (RandomString.GenerateFast(CharsetShouldEncode, 8), RandomString.GenerateFast(CharsetShouldEncode, 4));
			KVarray3ShouldNotEncode[i] = (RandomString.GenerateFast(CharsetShouldNotEncode, 8), RandomString.GenerateFast(CharsetShouldNotEncode, 4));
		}

		for (var i = 0; i < 8; ++i) {
			KVarray8ShouldEncode[i] = (RandomString.GenerateFast(CharsetShouldEncode, 8), RandomString.GenerateFast(CharsetShouldEncode, 4));
			KVarray8ShouldNotEncode[i] = (RandomString.GenerateFast(CharsetShouldNotEncode, 8), RandomString.GenerateFast(CharsetShouldNotEncode, 4));
		}

		for (var i = 0; i < 20; ++i) {
			KVarray20ShouldEncode[i] = (RandomString.GenerateFast(CharsetShouldEncode, 8), RandomString.GenerateFast(CharsetShouldEncode, 4));
			KVarray20ShouldNotEncode[i] = (RandomString.GenerateFast(CharsetShouldNotEncode, 8), RandomString.GenerateFast(CharsetShouldNotEncode, 4));
		}
	}

	[BenchmarkCategory("3 kv pair should not encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method1() {
		var qs = new QueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should not encode"), Benchmark(Description = "FastQueryString")]
	public string Method2() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should not encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method59() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should not encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method3() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should not encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method4() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("8 kv pair should not encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method5() {
		var qs = new QueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should not encode"), Benchmark(Description = "FastQueryString")]
	public string Method6() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should not encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method60() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should not encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method7() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should not encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method8() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("20 kv pair should not encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method9() {
		var qs = new QueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should not encode"), Benchmark(Description = "FastQueryString")]
	public string Method10() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should not encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method58() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should not encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method11() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should not encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method12() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("3 kv pair should encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method13() {
		var qs = new QueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should encode"), Benchmark(Description = "FastQueryString")]
	public string Method14() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method57() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method15() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method16() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("8 kv pair should encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method17() {
		var qs = new QueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should encode"), Benchmark(Description = "FastQueryString")]
	public string Method18() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method56() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method19() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method20() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("20 kv pair should encode"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method21() {
		var qs = new QueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should encode"), Benchmark(Description = "FastQueryString")]
	public string Method22() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should encode"), Benchmark(Description = "ASP.QueryString")]
	public string Method55() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should encode"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method23() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should encode"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method24() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("3 kv pair should not encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method25() {
		var qs = new QueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should not encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method26() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method54() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}

		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method27() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method28() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return result;
	}

	[BenchmarkCategory("8 kv pair should not encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method29() {
		var qs = new QueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should not encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method30() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method53() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method31() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method32() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("20 kv pair should not encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method33() {
		var qs = new QueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should not encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method34() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method52() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs = qs.Add(item.key, item.value);
		}
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method35() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should not encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method36() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("3 kv pair should encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method37() {
		var qs = new QueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method38() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method51() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method39() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("3 kv pair should encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method40() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("8 kv pair should encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method41() {
		var qs = new QueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method42() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method50() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("8 kv pair should encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method43() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("8 kv pair should encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method44() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return QueryHelpers.AddQueryString("", qs);
	}

	[BenchmarkCategory("20 kv pair should encode with array"), Benchmark(Baseline = true, Description = "RestFetch")]
	public string Method45() {
		var qs = new QueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should encode with array"), Benchmark(Description = "FastQueryString")]
	public string Method46() {
		using var qs = new FastQueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should encode with array"), Benchmark(Description = "ASP.QueryString")]
	public string Method49() {
		var qs = new Microsoft.AspNetCore.Http.QueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs = qs.Add(item.key, item.value);
		}
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			qs = qs.Add(k, arr[i]);
		}
		return qs.ToUriComponent();
	}

	[BenchmarkCategory("20 kv pair should encode with array"), Benchmark(Description = "ASP.QueryBuilder")]
	public string Method47() {
		var qs = new QueryBuilder();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		return qs.ToString();
	}

	[BenchmarkCategory("20 kv pair should encode with array"), Benchmark(Description = "ASP.QueryHelper")]
	public string Method48() {
		var qs = new Dictionary<string, string?>();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		var result = QueryHelpers.AddQueryString("", qs);
		var (k, arr) = KVWithArray;
		for (var i = 0; i < arr.Length; ++i) {
			result = QueryHelpers.AddQueryString(result, k, arr[i]);
		}
		return QueryHelpers.AddQueryString("", qs);
	}
}
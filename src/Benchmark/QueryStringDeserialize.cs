using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

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
public class QueryStringDeserialize {
	
	public static string Pairs3ShouldDecode = "";
	public static string Pairs8ShouldDecode = "";
	public static string Pairs20ShouldDecode = "";
	public static string Pairs3ShouldNotDecode = "";
	public static string Pairs8ShouldNotDecode = "";
	public static string Pairs20ShouldNotDecode = "";
	public static string Pairs3ShouldDecodeWithArray = "";
	public static string Pairs8ShouldDecodeWithArray = "";
	public static string Pairs20ShouldDecodeWithArray = "";
	public static string Pairs3ShouldNotDecodeWithArray = "";
	public static string Pairs8ShouldNotDecodeWithArray = "";
	public static string Pairs20ShouldNotDecodeWithArray = "";

	static QueryStringDeserialize() {
		string CharsetShouldEncode = "各种文字符号用来测试需要编码比较随机大小合适";
		CharsetType CharsetShouldNotEncode = CharsetType.AlphabetLower | CharsetType.AlphabetUpper | CharsetType.Numbers;
		KV[] KVarray3ShouldNotEncode = new KV[3];
		KV[] KVarray8ShouldNotEncode = new KV[8];
		KV[] KVarray20ShouldNotEncode = new KV[20];
		KV[] KVarray3ShouldEncode = new KV[3];
		KV[] KVarray8ShouldEncode = new KV[8];
		KV[] KVarray20ShouldEncode = new KV[20];
		KeyValuePair<string, string[]> KVWithArray = new(RandomString.GenerateFast(CharsetShouldNotEncode, 8), [
			RandomString.GenerateFast(CharsetShouldNotEncode, 4),
			RandomString.GenerateFast(CharsetShouldNotEncode, 4),
			RandomString.GenerateFast(CharsetShouldNotEncode, 4),
			RandomString.GenerateFast(CharsetShouldNotEncode, 4),
			RandomString.GenerateFast(CharsetShouldNotEncode, 4),
		]);

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
		
		var qs = new QueryString();
		foreach (var item in KVarray3ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs3ShouldDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs3ShouldDecodeWithArray = qs.ToUriComponent();

		qs = new QueryString();
		foreach (var item in KVarray8ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs8ShouldDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs8ShouldDecodeWithArray = qs.ToUriComponent();

		qs = new QueryString();
		foreach (var item in KVarray20ShouldEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs20ShouldDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs20ShouldDecodeWithArray = qs.ToUriComponent();
		
		qs = new QueryString();
		foreach (var item in KVarray3ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs3ShouldNotDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs3ShouldNotDecodeWithArray = qs.ToUriComponent();

		qs = new QueryString();
		foreach (var item in KVarray8ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs8ShouldNotDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs8ShouldNotDecodeWithArray = qs.ToUriComponent();

		qs = new QueryString();
		foreach (var item in KVarray20ShouldNotEncode) {
			qs.Add(item.key, item.value);
		}
		Pairs20ShouldNotDecode = qs.ToUriComponent();
		qs.Add(KVWithArray.Key, KVWithArray.Value);
		Pairs20ShouldNotDecodeWithArray = qs.ToUriComponent();
	}

	[BenchmarkCategory("3 kv pair should decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method1() => QueryString.Parse(Pairs3ShouldDecode);

	[BenchmarkCategory("3 kv pair should decode"), Benchmark(Description = "ParseSimple")]
	public void Method2() => QueryString.ParseSimple(Pairs3ShouldDecode);

	[BenchmarkCategory("3 kv pair should decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method3() => QueryString.ParseFromFastQS(Pairs3ShouldDecode);

	[BenchmarkCategory("3 kv pair should decode"), Benchmark(Description = "QueryHelpers")]
	public void Method4() => QueryHelpers.ParseQuery(Pairs3ShouldDecode);

	[BenchmarkCategory("8 kv pair should decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method5() => QueryString.Parse(Pairs8ShouldDecode);

	[BenchmarkCategory("8 kv pair should decode"), Benchmark(Description = "ParseSimple")]
	public void Method6() => QueryString.ParseSimple(Pairs8ShouldDecode);

	[BenchmarkCategory("8 kv pair should decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method7() => QueryString.ParseFromFastQS(Pairs8ShouldDecode);

	[BenchmarkCategory("8 kv pair should decode"), Benchmark(Description = "QueryHelpers")]
	public void Method8() => QueryHelpers.ParseQuery(Pairs8ShouldDecode);

	[BenchmarkCategory("20 kv pair should decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method9() => QueryString.Parse(Pairs20ShouldDecode);

	[BenchmarkCategory("20 kv pair should decode"), Benchmark(Description = "ParseSimple")]
	public void Method10() => QueryString.ParseSimple(Pairs20ShouldDecode);

	[BenchmarkCategory("20 kv pair should decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method11() => QueryString.ParseFromFastQS(Pairs20ShouldDecode);

	[BenchmarkCategory("20 kv pair should decode"), Benchmark(Description = "QueryHelpers")]
	public void Method12() => QueryHelpers.ParseQuery(Pairs20ShouldDecode);

	[BenchmarkCategory("3 kv pair should not decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method13() => QueryString.Parse(Pairs3ShouldNotDecode);

	[BenchmarkCategory("3 kv pair should not decode"), Benchmark(Description = "ParseSimple")]
	public void Method14() => QueryString.ParseSimple(Pairs3ShouldNotDecode);

	[BenchmarkCategory("3 kv pair should not decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method15() => QueryString.ParseFromFastQS(Pairs3ShouldNotDecode);

	[BenchmarkCategory("3 kv pair should not decode"), Benchmark(Description = "QueryHelpers")]
	public void Method16() => QueryHelpers.ParseQuery(Pairs3ShouldNotDecode);

	[BenchmarkCategory("8 kv pair should not decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method17() => QueryString.Parse(Pairs8ShouldNotDecode);

	[BenchmarkCategory("8 kv pair should not decode"), Benchmark(Description = "ParseSimple")]
	public void Method18() => QueryString.ParseSimple(Pairs8ShouldNotDecode);

	[BenchmarkCategory("8 kv pair should not decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method19() => QueryString.ParseFromFastQS(Pairs8ShouldNotDecode);

	[BenchmarkCategory("8 kv pair should not decode"), Benchmark(Description = "QueryHelpers")]
	public void Method20() => QueryHelpers.ParseQuery(Pairs8ShouldNotDecode);

	[BenchmarkCategory("20 kv pair should not decode"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method21() => QueryString.Parse(Pairs20ShouldNotDecode);

	[BenchmarkCategory("20 kv pair should not decode"), Benchmark(Description = "ParseSimple")]
	public void Method22() => QueryString.ParseSimple(Pairs20ShouldNotDecode);

	[BenchmarkCategory("20 kv pair should not decode"), Benchmark(Description = "ParseFromFastQS")]
	public void Method23() => QueryString.ParseFromFastQS(Pairs20ShouldNotDecode);

	[BenchmarkCategory("20 kv pair should not decode"), Benchmark(Description = "QueryHelpers")]
	public void Method24() => QueryHelpers.ParseQuery(Pairs20ShouldNotDecode);

	[BenchmarkCategory("3 kv pair should decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method25() => QueryString.Parse(Pairs3ShouldDecodeWithArray);

	[BenchmarkCategory("3 kv pair should decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method26() => QueryString.ParseSimple(Pairs3ShouldDecodeWithArray);

	[BenchmarkCategory("3 kv pair should decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method27() => QueryString.ParseFromFastQS(Pairs3ShouldDecodeWithArray);

	[BenchmarkCategory("3 kv pair should decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method28() => QueryHelpers.ParseQuery(Pairs3ShouldDecodeWithArray);

	[BenchmarkCategory("8 kv pair should decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method29() => QueryString.Parse(Pairs8ShouldDecodeWithArray);

	[BenchmarkCategory("8 kv pair should decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method30() => QueryString.ParseSimple(Pairs8ShouldDecodeWithArray);

	[BenchmarkCategory("8 kv pair should decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method31() => QueryString.ParseFromFastQS(Pairs8ShouldDecodeWithArray);

	[BenchmarkCategory("8 kv pair should decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method32() => QueryHelpers.ParseQuery(Pairs8ShouldDecodeWithArray);

	[BenchmarkCategory("20 kv pair should decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method33() => QueryString.Parse(Pairs20ShouldDecodeWithArray);

	[BenchmarkCategory("20 kv pair should decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method34() => QueryString.ParseSimple(Pairs20ShouldDecodeWithArray);

	[BenchmarkCategory("20 kv pair should decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method35() => QueryString.ParseFromFastQS(Pairs20ShouldDecodeWithArray);

	[BenchmarkCategory("20 kv pair should decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method36() => QueryHelpers.ParseQuery(Pairs20ShouldDecodeWithArray);

	[BenchmarkCategory("3 kv pair should not decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method37() => QueryString.Parse(Pairs3ShouldNotDecodeWithArray);

	[BenchmarkCategory("3 kv pair should not decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method38() => QueryString.ParseSimple(Pairs3ShouldNotDecodeWithArray);

	[BenchmarkCategory("3 kv pair should not decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method39() => QueryString.ParseFromFastQS(Pairs3ShouldNotDecodeWithArray);

	[BenchmarkCategory("3 kv pair should not decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method40() => QueryHelpers.ParseQuery(Pairs3ShouldNotDecodeWithArray);

	[BenchmarkCategory("8 kv pair should not decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method41() => QueryString.Parse(Pairs8ShouldNotDecodeWithArray);

	[BenchmarkCategory("8 kv pair should not decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method42() => QueryString.ParseSimple(Pairs8ShouldNotDecodeWithArray);

	[BenchmarkCategory("8 kv pair should not decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method43() => QueryString.ParseFromFastQS(Pairs8ShouldNotDecodeWithArray);

	[BenchmarkCategory("8 kv pair should not decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method44() => QueryHelpers.ParseQuery(Pairs8ShouldNotDecodeWithArray);

	[BenchmarkCategory("20 kv pair should not decode with array"), Benchmark(Baseline = true, Description = "QueryString.Parse")]
	public void Method45() => QueryString.Parse(Pairs20ShouldNotDecodeWithArray);

	[BenchmarkCategory("20 kv pair should not decode with array"), Benchmark(Description = "ParseSimple")]
	public void Method46() => QueryString.ParseSimple(Pairs20ShouldNotDecodeWithArray);

	[BenchmarkCategory("20 kv pair should not decode with array"), Benchmark(Description = "ParseFromFastQS")]
	public void Method47() => QueryString.ParseFromFastQS(Pairs20ShouldNotDecodeWithArray);

	[BenchmarkCategory("20 kv pair should not decode with array"), Benchmark(Description = "QueryHelpers")]
	public void Method48() => QueryHelpers.ParseQuery(Pairs20ShouldNotDecodeWithArray);
}

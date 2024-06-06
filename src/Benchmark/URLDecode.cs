using System.Web;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using RestFetch.WebUtilities;

using SixTatami.Utilities;

namespace TODO.Benchmark;

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
public class URLDecode {
	public static string LongStrShouldDecode = "%E5%AD%97%23%E8%BF%98un++x()vqG%E6%B5%8Bq%E5%87%A0%E7%94%A8w%23%E4%B8%8D%23t%E5%A4%9A%E6%B5%8BMV1%E8%AF%95%E9%95%BFJ%E7%82%B9%E4%B8%8Dfh%E6%A0%B7%E5%90%A7wwn%22q%23%E7%BB%ADF%2F7SL%E6%9B%B4.%26T%22%E6%83%B3%24X%E4%BA%86%E5%90%A7U%3D%2A%3A6%23MjK.%E5%B0%B1%2F%E6%9B%B4%60S%E8%BF%98QNVfk%27%E5%B0%B19%E6%8D%AEh";

	public static string ShortStrShouldDecode = "%E8%BF%98++%E6%B5";

	public static string LongStrShouldNotDecode = RandomString.GenerateFast(CharsetType.AlphabetLower | CharsetType.AlphabetUpper | CharsetType.Numbers, 80);

	public static string ShortStrShouldNotDecode = RandomString.GenerateFast(CharsetType.AlphabetLower | CharsetType.AlphabetUpper | CharsetType.Numbers, 10);

	[BenchmarkCategory("Long String should decode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method1() => URLHelper.DecodeURIComponent(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "Uri")]
	public string Method2() => Uri.UnescapeDataString(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "UriFromBCL")]
	public string Method3() => UriFromBCL.UnescapeDataString(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "HttpUtility")]
	public string Method4() => HttpUtility.UrlDecode(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "System.Net.WebUtility")]
	public string Method5() => System.Net.WebUtility.UrlDecode(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "WebUtility")]
	public string Method6() => WebUtility.UrlDecode(LongStrShouldDecode);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "BunURL1")]
	public string Method7() => BunURL.DecodeURIComponent(LongStrShouldDecode, true);

	[BenchmarkCategory("Long String should decode"), Benchmark(Description = "BunURL2")]
	public string Method8() => BunURL.DecodeURIComponent(LongStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method9() => URLHelper.DecodeURIComponent(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "Uri")]
	public string Method10() => Uri.UnescapeDataString(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "UriFromBCL")]
	public string Method11() => UriFromBCL.UnescapeDataString(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "HttpUtility")]
	public string Method12() => HttpUtility.UrlDecode(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "System.Net.WebUtility")]
	public string Method13() => System.Net.WebUtility.UrlDecode(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "WebUtility")]
	public string Method14() => WebUtility.UrlDecode(ShortStrShouldDecode);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "BunURL1")]
	public string Method15() => BunURL.DecodeURIComponent(ShortStrShouldDecode, true);

	[BenchmarkCategory("Short String should decode"), Benchmark(Description = "BunURL2")]
	public string Method16() => BunURL.DecodeURIComponent(ShortStrShouldDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method17() => URLHelper.DecodeURIComponent(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "Uri")]
	public string Method18() => Uri.UnescapeDataString(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "UriFromBCL")]
	public string Method19() => UriFromBCL.UnescapeDataString(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "HttpUtility")]
	public string Method20() => HttpUtility.UrlDecode(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "System.Net.WebUtility")]
	public string Method21() => System.Net.WebUtility.UrlDecode(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "WebUtility")]
	public string Method22() => WebUtility.UrlDecode(LongStrShouldNotDecode);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "BunURL1")]
	public string Method23() => BunURL.DecodeURIComponent(LongStrShouldNotDecode, true);

	[BenchmarkCategory("Long String should not decode"), Benchmark(Description = "BunURL2")]
	public string Method24() => BunURL.DecodeURIComponent(LongStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method25() => URLHelper.DecodeURIComponent(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "Uri")]
	public string Method26() => Uri.UnescapeDataString(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "UriFromBCL")]
	public string Method27() => UriFromBCL.UnescapeDataString(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "HttpUtility")]
	public string Method28() => HttpUtility.UrlDecode(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "System.Net.WebUtility")]
	public string Method29() => System.Net.WebUtility.UrlDecode(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "WebUtility")]
	public string Method30() => WebUtility.UrlDecode(ShortStrShouldNotDecode);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "BunURL1")]
	public string Method31() => BunURL.DecodeURIComponent(ShortStrShouldNotDecode, true);

	[BenchmarkCategory("Short String should not decode"), Benchmark(Description = "BunURL2")]
	public string Method32() => BunURL.DecodeURIComponent(ShortStrShouldNotDecode);
}
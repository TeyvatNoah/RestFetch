using System.Text.Encodings.Web;
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
public class URLEncode {
	public static string CharsetShouldEncode = CharsetType.All.Value() + "测试数据集合多一点就好再长几个字符更长两下就这样好了吧还不行继续加用力想";

	public static CharsetType CharsetShouldNotEncode = CharsetType.AlphabetLower | CharsetType.AlphabetUpper | CharsetType.Numbers;

	public static string LongStrShouldEncode = RandomString.GenerateFast(CharsetShouldEncode, 80);

	public static string ShortStrShouldEncode = RandomString.GenerateFast("多侧记个字符看下不然可能有问题不是吗", 10);

	public static string LongStrShouldNotEncode = RandomString.GenerateFast(CharsetShouldNotEncode, 80);

	public static string ShortStrShouldNotEncode = RandomString.GenerateFast(CharsetShouldNotEncode, 10);

	[BenchmarkCategory("Long String should encode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method1() => URLHelper.EncodeURIComponent(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "Uri")]
	public string Method2() => Uri.EscapeDataString(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "HttpUtility")]
	public string Method3() => HttpUtility.UrlEncode(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "WebUtility")]
	public string Method4() => System.Net.WebUtility.UrlEncode(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "UrlEncoder")]
	public string Method5() => UrlEncoder.Default.Encode(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "BunURL")]
	public string Method6() => BunURL.EncodeURIComponent(LongStrShouldEncode);

	[BenchmarkCategory("Long String should encode"), Benchmark(Description = "NodeURL")]
	public string Method7() => NodeURL.EncodeURIComponent(LongStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method8() => URLHelper.EncodeURIComponent(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "Uri")]
	public string Method9() => Uri.EscapeDataString(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "HttpUtility")]
	public string Method10() => HttpUtility.UrlEncode(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "WebUtility")]
	public string Method11() => System.Net.WebUtility.UrlEncode(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "UrlEncoder")]
	public string Method12() => UrlEncoder.Default.Encode(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "BunURL")]
	public string Method13() => BunURL.EncodeURIComponent(ShortStrShouldEncode);

	[BenchmarkCategory("Short String should encode"), Benchmark(Description = "NodeURL")]
	public string Method14() => NodeURL.EncodeURIComponent(ShortStrShouldEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method15() => URLHelper.EncodeURIComponent(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "Uri")]
	public string Method16() => Uri.EscapeDataString(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "HttpUtility")]
	public string Method17() => HttpUtility.UrlEncode(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "WebUtility")]
	public string Method18() => System.Net.WebUtility.UrlEncode(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "UrlEncoder")]
	public string Method19() => UrlEncoder.Default.Encode(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "BunURL")]
	public string Method20() => BunURL.EncodeURIComponent(LongStrShouldNotEncode);

	[BenchmarkCategory("Long String should not encode"), Benchmark(Description = "NodeURL")]
	public string Method21() => NodeURL.EncodeURIComponent(LongStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Baseline = true, Description = "URLHelper")]
	public string Method22() => URLHelper.EncodeURIComponent(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "Uri")]
	public string Method23() => Uri.EscapeDataString(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "HttpUtility")]
	public string Method24() => HttpUtility.UrlEncode(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "WebUtility")]
	public string Method25() => System.Net.WebUtility.UrlEncode(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "UrlEncoder")]
	public string Method26() => UrlEncoder.Default.Encode(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "BunURL")]
	public string Method27() => BunURL.EncodeURIComponent(ShortStrShouldNotEncode);

	[BenchmarkCategory("Short String should not encode"), Benchmark(Description = "NodeURL")]
	public string Method28() => NodeURL.EncodeURIComponent(ShortStrShouldNotEncode);
}
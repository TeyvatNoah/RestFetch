using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace TODO.Benchmark;

[SimpleJob(RuntimeMoniker.Net90, launchCount: 3, warmupCount:35)]
[SimpleJob(RuntimeMoniker.NativeAot90, launchCount:3, warmupCount:35)]
[AsciiDocExporter]
// [HtmlExporter]
// [CsvExporter]
// [MarkdownExporterAttribute.GitHub]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class TODOBenchmark {
	[Params(1, 2)]
	public int a;

	[ParamsSource(nameof(GetData))]
	public int b;


	[Benchmark(Baseline = true, Description = "")]
	public int Method1() => 1;

	[Benchmark]
	public int Method2() => 2;

	[Benchmark]
	[BenchmarkCategory("Group1")]
	public int Method3() => 3;

	[Benchmark]
	[Arguments(100, 10)]
	[Arguments(200, 20)]
	public int Method4(int a, int b) => 4;

	[Benchmark]
	[ArgumentsSource(nameof(GetData))]
	public int Method4(int a) => 5;

	[GlobalSetup]
	// [GlobalSetup(Target = nameof(Method1))]
	public void Init() {}

	[GlobalCleanup]
	public void Cleanup() {}

	private int[] GetData() => [1, 2, 3];
}

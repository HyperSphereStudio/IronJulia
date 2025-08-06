using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using IronJulia.CoreLib;

namespace Benchmarks;

/*
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Intel Core Ultra 7 155U 1.70GHz, 1 CPU, 14 logical and 12 physical cores
.NET SDK 10.0.100-preview.5.25277.114
  [Host]     : .NET 10.0.0 (10.0.25.27814), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.0 (10.0.25.27814), X64 RyuJIT AVX2


| Method             | N       | Mean              | Error          | StdDev         | Median            |
|------------------- |-------- |------------------:|---------------:|---------------:|------------------:|
| AddNJuliaUnmanaged | 1       |         4.6507 ns |      0.1176 ns |      0.2029 ns |         4.5992 ns |
| AddNNetUnmanaged   | 1       |         1.7459 ns |      0.0386 ns |      0.0322 ns |         1.7350 ns |
| AddNJuliaOnly      | 1       |         3.2138 ns |      0.0479 ns |      0.0448 ns |         3.2110 ns |
| AddNNetOnly        | 1       |         0.7330 ns |      0.0139 ns |      0.0116 ns |         0.7299 ns |
//Net Iterating      | 1       |         1.0129 ns
//Julia Iterating    | 1       |         1.43689 ns

| AddNJuliaUnmanaged | 5       |        11.2616 ns |      0.0737 ns |      0.0653 ns |        11.2787 ns |
| AddNNetUnmanaged   | 5       |         8.6545 ns |      0.1189 ns |      0.1054 ns |         8.6898 ns |
| AddNJuliaOnly      | 5       |         8.7372 ns |      0.0677 ns |      0.0565 ns |         8.7531 ns |
| AddNNetOnly        | 5       |         4.6182 ns |      0.0281 ns |      0.0249 ns |         4.6231 ns |
//Net Iterating      | 5       |         4.0363 ns
//Julia Iterating    | 5       |         2.5244 ns

| AddNJuliaUnmanaged | 1000    |     1,620.1421 ns |     10.9418 ns |      9.6996 ns |     1,616.8069 ns |
| AddNNetUnmanaged   | 1000    |     1,078.4147 ns |      9.1742 ns |      8.5815 ns |     1,075.3862 ns |
| AddNJuliaOnly      | 1000    |     1,247.0429 ns |     24.8256 ns |     62.7374 ns |     1,216.6801 ns |
| AddNNetOnly        | 1000    |       645.6235 ns |     12.5021 ns |     32.7157 ns |       631.5526 ns |
//Net Iterating      | 1000    |         432.7912 ns
//Julia Iterating    | 1000    |         373.0992 ns

| AddNJuliaUnmanaged | 1000000 | 1,616,445.5078 ns | 11,381.7061 ns | 10,089.5860 ns | 1,612,649.4141 ns |
| AddNNetUnmanaged   | 1000000 | 1,049,452.8320 ns | 12,660.2860 ns | 11,223.0138 ns | 1,048,456.3477 ns |
| AddNJuliaOnly      | 1000000 | 1,369,008.3984 ns | 27,317.0352 ns | 25,552.3717 ns | 1,359,680.0781 ns |
| AddNNetOnly        | 1000000 |   588,677.5240 ns |  4,538.9715 ns |  3,790.2468 ns |   588,281.9336 ns |
//Net Iterating      | 1000000 |         ~247,437 ns
//Julia Iterating    | 1000000 |         460,775 ns
*/

public class ArrayBenchmarks
{
    public Core.Array<int, Vals.Int1, ValueTuple<Vals.Int1>> _a;
    private readonly Consumer consumer = new();
    public List<int> _b;

    [Params(1, 5, 1000, 1000000)]
    public int N;
    
    [GlobalSetup]
    public void Setup() {
        _a = new();
        _b = new();
    }

    [Benchmark]
    public void AddNJuliaUnmanaged() {
        _a.Clear();
        for (var i = 0; i < N; i++) {
            _a.Add(i);
        }
        foreach (var k in _a) {
            consumer.Consume(k);
        }
    }

    [Benchmark]
    public void AddNNetUnmanaged() {
        _b.Clear();
        for (var i = 0; i < N; i++) {
            _b.Add(i);
        }
        foreach (var k in _b) {
            consumer.Consume(k);
        }
    }
    
    [Benchmark]
    public void AddNJuliaOnly() {
        _a.Clear();
        for (var i = 0; i < N; i++) {
            _a.Add(i);
        }
    }
    
    [Benchmark]
    public void AddNNetOnly() {
        _b.Clear();
        for (var i = 0; i < N; i++) {
            _b.Add(i);
        }
    }
    
    public static void TestArray() {
        BenchmarkRunner.Run<ArrayBenchmarks>();
    }
}
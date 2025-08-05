using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using IronJulia.CoreLib;

namespace Benchmarks;

/*



 */

public class ArrayBenchmarks
{
    public Core.Array<int, Vals.Int1, ValueTuple<Vals.Int1>> _a;
    public Core.Array<object, Vals.Int1, ValueTuple<Vals.Int1>> _c;
    private readonly Consumer consumer = new();
    public List<int> _b;
    public List<object> _d;

    [Params(1, 5, 1000, 10000, 1000000)]
    public int N;
    
    [GlobalSetup]
    public void Setup() {
        _a = new();
        _b = new();
        _c = new();
        _d = new();
    }

    [Benchmark]
    public void AddNJuliaUnmanaged() {
        _a.Clear();
        for (var i = 0; i < N; i++) {
            _a.Add(i);
        }
    }

    [Benchmark]
    public void AddThenIterateJuliaUnmanaged() {
        AddNJuliaUnmanaged();
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
    }

    [Benchmark]
    public void AddThenIterateNetUnmanaged() {
        AddNNetUnmanaged();
        foreach (var k in _b) {
            consumer.Consume(k);
        }
    }
    
    public static void TestArray() {
        BenchmarkRunner.Run<ArrayBenchmarks>();
    }
}
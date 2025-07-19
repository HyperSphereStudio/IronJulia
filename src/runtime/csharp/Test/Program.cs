// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
var j = Test.myLittleStruct;
GC.Collect();

static class Test {
    public static MyLittleStruct myLittleStruct;
    
    public struct MyLittleStruct {
        public int A;
        public string B;
    }
}
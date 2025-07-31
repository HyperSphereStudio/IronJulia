using System.Collections;
using System.Runtime.CompilerServices;
using static Base;

public partial struct Core {
    public interface AbstractRange : AbstractArray;

    public interface AbstractRange<T> : AbstractRange, AbstractVector<T> {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public struct UnitRange<T>(T start, T stop) : AbstractRange<T> {
        public T start = start, stop = stop;
        
        public IEnumerator<T> GetEnumerator() {
            //Fast path
            if (start is Int sti && stop is Int stopi) {
                stopi += 1;
                for (; sti != stopi; sti++)
                    yield return Unsafe.BitCast<Int, T>(sti);
            }
        }
    }
    
}
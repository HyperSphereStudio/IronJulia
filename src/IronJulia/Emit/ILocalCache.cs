#nullable disable
using System.Reflection.Emit;

namespace IronJulia.Emit;

public interface ILocalCache
{
    LocalBuilder GetLocal(Type type);

    void FreeLocal(LocalBuilder local);
}
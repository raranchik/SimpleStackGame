using Core.MonoConverter;
using Core.Stack.Base.Links;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Base.MonoLinks
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class StackObjectIdMonoLink : MonoLink<StackObjectIdLink>
    {
    }
}
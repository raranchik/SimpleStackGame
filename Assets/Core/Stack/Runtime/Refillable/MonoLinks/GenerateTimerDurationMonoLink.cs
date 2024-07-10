using Core.MonoConverter;
using Core.Stack.Refillable.Links;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Refillable.MonoLinks
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateTimerDurationMonoLink : MonoLink<GenerateTimerDurationLink>
    {
    }
}
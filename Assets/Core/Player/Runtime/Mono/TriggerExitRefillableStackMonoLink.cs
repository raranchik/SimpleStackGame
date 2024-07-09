using Core.MonoConverter;
using Core.Player.Tags;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Player.Mono
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class
        TriggerExitRefillableStackMonoLink :
            TriggerExitPhysicsMonoLink<TriggerExitRefillableStackTag>
    {
    }
}
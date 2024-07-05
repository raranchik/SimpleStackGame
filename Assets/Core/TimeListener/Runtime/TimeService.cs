#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeListener
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimeService
    {
        public float Time;
        public float DeltaTime;
        public float UnscaledTime;
        public float UnscaledDeltaTime;
    }
}
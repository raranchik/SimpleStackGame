#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Time
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimeListener
    {
        public float Time;
        public float DeltaTime;
        public float UnscaledTime;
        public float UnscaledDeltaTime;
    }
}
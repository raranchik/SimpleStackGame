using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Time
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimeListenerRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<TimeListener> m_TimeService = default;

        public void Run(IEcsSystems systems)
        {
            var timeService = m_TimeService.Value;
            timeService.Time = UnityEngine.Time.time;
            timeService.DeltaTime = UnityEngine.Time.deltaTime;
            timeService.UnscaledTime = UnityEngine.Time.unscaledTime;
            timeService.UnscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
        }
    }
}
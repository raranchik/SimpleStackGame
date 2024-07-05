using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeListener
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimeRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<TimeService> m_TimeServiceInject = default;

        public void Run(IEcsSystems systems)
        {
            var timeService = m_TimeServiceInject.Value;
            timeService.Time = Time.time;
            timeService.DeltaTime = Time.deltaTime;
            timeService.UnscaledTime = Time.unscaledTime;
            timeService.UnscaledDeltaTime = Time.unscaledDeltaTime;
        }
    }
}
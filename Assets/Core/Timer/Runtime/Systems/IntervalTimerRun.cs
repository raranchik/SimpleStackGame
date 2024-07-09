using Core.Logger;
using Core.Time;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Timer.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class IntervalTimerRun : IEcsRunSystem
    {
        private readonly ILogger m_Logger;
        private readonly EcsCustomInject<TimeService> m_Time;
        private readonly EcsPoolInject<IsCompleteSelfRequest> m_IsCompletePool;

        private readonly EcsFilterInject<
            Inc<ElapsedComponent, DurationComponent, IsIntervalTag>,
            Exc<IsDisabledTag>
        > m_TimerFilter;

        public IntervalTimerRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(IntervalTimerRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timer in m_TimerFilter.Value)
            {
                ref var elapsed = ref m_TimerFilter.Pools.Inc1.Get(timer);
                elapsed.Value += m_Time.Value.DeltaTime;

                ref var duration = ref m_TimerFilter.Pools.Inc2.Get(timer);
                if (elapsed.Value < duration.Value)
                {
                    continue;
                }

                elapsed.Value = 0f;
                m_IsCompletePool.Value.Add(timer);
            }
        }
    }
}
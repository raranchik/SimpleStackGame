using Core.Logger;
using Core.TimeManagement.Time;
using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.SelfRequests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Timer.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class IntervalTimerRun : IEcsRunSystem
    {
        private readonly ILogger m_Logger;

        private readonly EcsFilterInject<
            Inc<ElapsedTimeComponent, DurationComponent, IsIntervalTimerComponent>,
            Exc<IsDisabledComponent>
        > m_TimerFilter;

        private readonly EcsCustomInject<TimeListener> m_TimeListener;
        private readonly EcsFilterInject<Inc<CompleteSelfRequest>> m_CompleteTimerFilter;

        public IntervalTimerRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(IntervalTimerRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_CompleteTimerFilter.Value.GetEntitiesCount() > 0)
            {
                m_Logger.Log(LogType.Error,
                    $"Requests that have run 1 cycle are in system:" +
                    $" count {m_CompleteTimerFilter.Value.GetEntitiesCount().ToString()}." +
                    $" I deleted them for you");

                foreach (var completeTimer in m_CompleteTimerFilter.Value)
                {
                    m_CompleteTimerFilter.Pools.Inc1.Del(completeTimer);
                }
            }

            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timer in m_TimerFilter.Value)
            {
                ref var elapsed = ref m_TimerFilter.Pools.Inc1.Get(timer);
                elapsed.Value += m_TimeListener.Value.DeltaTime;

                ref var duration = ref m_TimerFilter.Pools.Inc2.Get(timer);
                if (elapsed.Value < duration.Value)
                {
                    continue;
                }

                elapsed.Value = 0f;
                m_CompleteTimerFilter.Pools.Inc1.Add(timer);
            }
        }
    }
}
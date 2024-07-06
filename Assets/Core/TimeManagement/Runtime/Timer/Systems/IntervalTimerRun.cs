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
            Exc<TimerIsDisabledComponent>
        > m_CounterFilter;

        private readonly EcsCustomInject<TimeListener> m_TimeListener;
        private readonly EcsFilterInject<Inc<SelfPingRequest>> m_PingRequestFilter;

        public IntervalTimerRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(IntervalTimerRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_PingRequestFilter.Value.GetEntitiesCount() > 0)
            {
                m_Logger.Log(LogType.Error,
                    $"Requests that have run 1 cycle are in system:" +
                    $" count {m_PingRequestFilter.Value.GetEntitiesCount().ToString()}." +
                    $" I deleted them for you");
                foreach (var pingEntity in m_PingRequestFilter.Value)
                {
                    m_PingRequestFilter.Pools.Inc1.Del(pingEntity);
                }
            }

            if (m_CounterFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var counterEntity in m_CounterFilter.Value)
            {
                ref var counter = ref m_CounterFilter.Pools.Inc1.Get(counterEntity);
                counter.Value += m_TimeListener.Value.DeltaTime;

                ref var pingInterval = ref m_CounterFilter.Pools.Inc2.Get(counterEntity);
                if (counter.Value < pingInterval.Value)
                {
                    continue;
                }

                counter.Value = 0f;
                m_PingRequestFilter.Pools.Inc1.Add(counterEntity);
            }
        }
    }
}
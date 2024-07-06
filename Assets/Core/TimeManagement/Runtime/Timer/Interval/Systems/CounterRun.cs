using Core.TimeManagement.Time;
using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.Interval.Components;
using Core.TimeManagement.Timer.Interval.SelfRequests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Timer.Interval.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class CounterRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<TimerCounterComponent, PingIntervalLink>,
            Exc<TimerIsDisabledComponent>
        > m_CounterFilterInject;

        private readonly EcsPoolInject<SelfPingRequest> m_PingRequestPoolInject;
        private readonly EcsCustomInject<TimeListener> m_TimeListenerInject;
        private readonly EcsFilterInject<Inc<SelfPingRequest>> m_PingRequestFilterInject;

        public void Run(IEcsSystems systems)
        {
            var counterFilter = m_CounterFilterInject.Value;
            if (counterFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var pingSelfRequestFilter = m_PingRequestFilterInject.Value;
            var pingSelfRequestPool = m_PingRequestPoolInject.Value;
            if (pingSelfRequestFilter.GetEntitiesCount() > 0)
            {
                foreach (var pingEntity in pingSelfRequestFilter)
                {
                    pingSelfRequestPool.Del(pingEntity);
                }
            }

            var timeListener = m_TimeListenerInject.Value;
            var counterPool = m_CounterFilterInject.Pools.Inc1;
            var pingIntervalPool = m_CounterFilterInject.Pools.Inc2;
            foreach (var counterEntity in counterFilter)
            {
                ref var counter = ref counterPool.Get(counterEntity);
                counter.Value += timeListener.DeltaTime;

                ref var pingInterval = ref pingIntervalPool.Get(counterEntity);
                if (counter.Value < pingInterval.Value)
                {
                    continue;
                }

                counter.Value = 0f;
                pingSelfRequestPool.Add(counterEntity);
            }
        }
    }
}
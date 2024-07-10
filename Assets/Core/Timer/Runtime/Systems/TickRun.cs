using Core.Time;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Timer.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TickRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<TimeService> m_Time;
        private readonly EcsPoolInject<IsCompleteSelfRequest> m_IsComplete;
        private readonly EcsPoolInject<ElapsedComponent> m_Elapsed;
        private readonly EcsPoolInject<DurationComponent> m_Duration;

        private readonly EcsFilterInject<
            Inc<ElapsedComponent, DurationComponent, IsIntervalTag, IsTimerTag>,
            Exc<IsPausedTag>
        > m_Timers;

        public void Run(IEcsSystems systems)
        {
            if (m_Timers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timer in m_Timers.Value)
            {
                ref var elapsed = ref m_Elapsed.Value.Get(timer);
                elapsed.Value += m_Time.Value.DeltaTime;

                ref var duration = ref m_Duration.Value.Get(timer);
                if (elapsed.Value < duration.Value)
                {
                    continue;
                }

                m_IsComplete.Value.Add(timer);
            }
        }
    }
}
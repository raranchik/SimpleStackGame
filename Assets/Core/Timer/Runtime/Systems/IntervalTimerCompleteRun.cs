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
    public class IntervalTimerCompleteRun : IEcsRunSystem
    {
        private readonly EcsPoolInject<IsCompleteSelfRequest> m_IsComplete;
        private readonly EcsPoolInject<ElapsedComponent> m_Elapsed;

        private readonly EcsFilterInject<
            Inc<IsCompleteSelfRequest, IsIntervalTag, IsTimerTag>,
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
                elapsed.Value = 0f;
                m_IsComplete.Value.Del(timer);
            }
        }
    }
}
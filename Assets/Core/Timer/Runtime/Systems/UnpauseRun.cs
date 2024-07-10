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
    public class UnpauseRun : IEcsRunSystem
    {
        private readonly EcsPoolInject<UnpauseSelfRequest> m_Unpause;
        private readonly EcsPoolInject<IsPausedTag> m_IsPaused;

        private readonly EcsFilterInject<
            Inc<IsPausedTag, UnpauseSelfRequest, IsTimerTag>
        > m_Timers;

        public void Run(IEcsSystems systems)
        {
            if (m_Timers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timer in m_Timers.Value)
            {
                m_IsPaused.Value.Del(timer);
                m_Unpause.Value.Del(timer);
            }
        }
    }
}
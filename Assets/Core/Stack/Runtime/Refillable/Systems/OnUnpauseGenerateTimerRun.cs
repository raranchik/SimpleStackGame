using Core.Stack.Refillable.Links;
using Core.Stack.Refillable.Tags;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Refillable.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class OnUnpauseGenerateTimerRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TimerTextLink> m_TimerText;
        private readonly EcsPoolInject<ParentComponent> m_Parent;

        private readonly EcsFilterInject<
            Inc<GenerateTimerTag, IsTimerTag, UnpauseSelfRequest>,
            Exc<KillSelfRequest, PauseSelfRequest>
        > m_Timers;

        public void Run(IEcsSystems systems)
        {
            if (m_Timers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_Timers.Value)
            {
                ref var parent = ref m_Parent.Value.Get(timerEntity);
                parent.Value.Unpack(m_World.Value, out var parentEntity);

                ref var timerText = ref m_TimerText.Value.Get(parentEntity);
                timerText.Value.gameObject.SetActive(true);
            }
        }
    }
}
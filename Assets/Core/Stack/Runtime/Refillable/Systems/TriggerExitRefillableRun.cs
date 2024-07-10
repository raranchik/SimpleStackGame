using Core.EcsMapper;
using Core.MonoConverter.Requests;
using Core.Stack.Refillable.Components;
using Core.Stack.Refillable.Services;
using Core.Stack.Refillable.Tags;
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
    public class TriggerExitRefillableRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<RefillableService> m_RefillableService;
        private readonly EcsCustomInject<LeoEcsLiteEntityMap> m_Map;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TranslateTimerReferenceComponent> m_TimerPool;
        private readonly EcsPoolInject<TriggerExitRequest> m_Trigger;

        private readonly EcsFilterInject<
            Inc<TriggerExitRefillableTag, TriggerExitRequest>
        > m_Triggers;

        public void Run(IEcsSystems systems)
        {
            if (m_Triggers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var triggerRequestEntity in m_Triggers.Value)
            {
                ref var triggerRequest = ref m_Trigger.Value.Get(triggerRequestEntity);
                triggerRequest.Sender.Unpack(m_World.Value, out var srcEntity);
                if (!m_RefillableService.Value.HasTranslateTimerReference(srcEntity))
                {
                    m_World.Value.DelEntity(triggerRequestEntity);
                    continue;
                }

                var timerEntity = m_RefillableService.Value.GetTranslateTimer(srcEntity);
                m_RefillableService.Value.KillTimerEntity(timerEntity);
                m_RefillableService.Value.RemoveTranslateTimerReference(srcEntity);
                m_World.Value.DelEntity(triggerRequestEntity);
            }
        }
    }
}
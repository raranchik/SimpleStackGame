using Core.EcsMapper;
using Core.MonoConverter.Requests;
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
    public class TriggerStayRefillableRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsCustomInject<LeoEcsLiteEntityMap> m_Mapper;
        private readonly EcsCustomInject<RefillableService> m_RefillableService;
        private readonly EcsPoolInject<TriggerStayRequest> m_Trigger;

        private readonly EcsFilterInject<
            Inc<TriggerStayRefillableTag, TriggerStayRequest>
        > m_Triggers;

        public void Run(IEcsSystems systems)
        {
            if (m_Triggers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var triggerEntity in m_Triggers.Value)
            {
                ref var triggerRequest = ref m_Trigger.Value.Get(triggerEntity);
                if (!m_Mapper.Value.GetEntity(triggerRequest.Value.gameObject, out var dstEntityPacked))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                ref var srcEntityPacked = ref triggerRequest.Sender;
                srcEntityPacked.Unpack(m_World.Value, out var srcEntity);
                if (m_RefillableService.Value.IsEmptyStack(srcEntity))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                dstEntityPacked.Unpack(m_World.Value, out var dstEntity);
                if (m_RefillableService.Value.IsFullStack(dstEntity))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                if (m_RefillableService.Value.HasTranslateTimerReference(srcEntity))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                var duration = m_RefillableService.Value.GetTranslateTimerDuration(dstEntity);
                var translateTimer = m_RefillableService.Value
                    .CreateTimerEntityWithParent<TranslateTimerTag>(duration, srcEntityPacked);

                m_RefillableService.Value
                    .AddTranslateFromToComponent(translateTimer, srcEntityPacked, dstEntityPacked);

                m_RefillableService.Value
                    .AddTranslateTimerReferenceComponent(srcEntity, m_World.Value.PackEntity(translateTimer));

                m_World.Value.DelEntity(triggerEntity);
            }
        }
    }
}
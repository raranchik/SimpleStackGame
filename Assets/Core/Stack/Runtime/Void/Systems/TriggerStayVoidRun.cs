using Core.EcsMapper;
using Core.MonoConverter.Requests;
using Core.Stack.Refillable.Services;
using Core.Stack.Void.Services;
using Core.Stack.Void.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Void.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TriggerStayVoidRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;

        private readonly EcsCustomInject<LeoEcsLiteEntityMap> m_Mapper;
        private readonly EcsCustomInject<RefillableService> m_StackService;
        private readonly EcsCustomInject<VoidService> m_VoidService;
        private readonly EcsPoolInject<TriggerStayRequest> m_Trigger;

        private readonly EcsFilterInject<
            Inc<TriggerStayVoidTag, TriggerStayRequest>
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
                if (!m_Mapper.Value.GetEntity(triggerRequest.Value.gameObject, out var dstEntityPacked))
                {
                    m_World.Value.DelEntity(triggerRequestEntity);
                    continue;
                }

                dstEntityPacked.Unpack(m_World.Value, out var dstEntity);
                if (m_StackService.Value.IsEmptyStack(dstEntity))
                {
                    m_World.Value.DelEntity(triggerRequestEntity);
                    continue;
                }

                triggerRequest.Sender.Unpack(m_World.Value, out var srcEntity);
                if (m_VoidService.Value.HasTranslateTimerReference(srcEntity))
                {
                    m_World.Value.DelEntity(triggerRequestEntity);
                    continue;
                }

                var timerDuration = m_VoidService.Value.GetTranslateTimerDuration(srcEntity);
                var timerEntity = m_VoidService.Value.CreateTimerEntityWithParent(timerDuration, dstEntityPacked);
                m_VoidService.Value.AddTranslateFromToComponent(timerEntity, triggerRequest.Sender, dstEntityPacked);
                m_VoidService.Value.AddTimerReferenceComponent(srcEntity, m_World.Value.PackEntity(timerEntity));

                m_World.Value.DelEntity(triggerRequestEntity);
            }
        }
    }
}
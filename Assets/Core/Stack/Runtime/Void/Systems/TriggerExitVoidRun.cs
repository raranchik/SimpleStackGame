using Core.MonoConverter.Requests;
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
    public class TriggerExitVoidRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<VoidService> m_VoidService;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TriggerExitRequest> m_Trigger;

        private readonly EcsFilterInject<
            Inc<TriggerExitVoidTag, TriggerExitRequest>
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
                if (!m_VoidService.Value.HasTranslateTimerReference(srcEntity))
                {
                    m_World.Value.DelEntity(triggerRequestEntity);
                    continue;
                }

                var timerEntity = m_VoidService.Value.GetTimerEntity(srcEntity);
                m_VoidService.Value.KillTimerEntity(timerEntity);
                m_VoidService.Value.RemoveTranslateTimerReference(srcEntity);
                m_World.Value.DelEntity(triggerRequestEntity);
            }
        }
    }
}
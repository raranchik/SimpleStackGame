using Core.EcsMapper;
using Core.MonoConverter.Requests;
using Core.Player.Components;
using Core.Player.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Player.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TriggerExitRefillableStackPlayerRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<LeoEcsLiteEntityMapper> m_Mapper;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TranslateFromToPlayerTimerComponent> m_TimerPool;

        private readonly EcsFilterInject<
            Inc<TriggerExitRefillableStackTag, TriggerExitRequest>
        > m_TriggerFilter;

        public void Run(IEcsSystems systems)
        {
            if (m_TriggerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var triggerEntity in m_TriggerFilter.Value)
            {
                ref var trigger = ref m_TriggerFilter.Pools.Inc2.Get(triggerEntity);
                if (!m_Mapper.Value.GetEntity(trigger.Value.gameObject, out var dstEntityPacked))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                dstEntityPacked.Unpack(m_World.Value, out var dstEntity);
                if (!m_TimerPool.Value.Has(dstEntity))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                ref var timer = ref m_TimerPool.Value.Get(dstEntity);
                if (timer.Value.Unpack(m_World.Value, out var timerEntity))
                {
                    m_World.Value.DelEntity(timerEntity);
                }

                m_TimerPool.Value.Del(dstEntity);
                m_World.Value.DelEntity(triggerEntity);
            }
        }
    }
}
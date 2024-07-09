using Core.EcsMapper;
using Core.MonoConverter.Requests;
using Core.Player.Components;
using Core.Player.Links;
using Core.Player.Tags;
using Core.Stack.Components;
using Core.Stack.Links;
using Core.Timer.Services;
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
    public class TriggerStayRefillableStackPlayerRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<TimerService> m_TimerService;
        private readonly EcsCustomInject<LeoEcsLiteEntityMapper> m_Mapper;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TranslateFromToComponent> m_TranslatePool;
        private readonly EcsPoolInject<GetStackItemIntervalLink> m_IntervalPool;
        private readonly EcsPoolInject<StackComponent> m_StackPool;
        private readonly EcsPoolInject<StackMaxCountLink> m_StackMaxPool;
        private readonly EcsPoolInject<TranslateFromToPlayerTimerComponent> m_TimerPool;

        private readonly EcsFilterInject<
            Inc<TriggerStayRefillableStackTag, TriggerStayRequest>
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
                ref var dstStack = ref m_StackPool.Value.Get(dstEntity);
                ref var dstStackMax = ref m_StackMaxPool.Value.Get(dstEntity);
                if (dstStack.Value.Count >= dstStackMax.Value)
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                if (m_TimerPool.Value.Has(dstEntity))
                {
                    m_World.Value.DelEntity(triggerEntity);
                    continue;
                }

                trigger.Sender.Unpack(m_World.Value, out var srcEntity);
                ref var srcStack = ref m_StackPool.Value.Get(srcEntity);
                if (srcStack.Value.Count <= 0)
                {
                    m_World.Value.DelEntity(triggerEntity);
                    return;
                }

                ref var interval = ref m_IntervalPool.Value.Get(dstEntity);
                var timerPacked = m_TimerService.Value.DeclareTimer()
                    .AsInterval()
                    .WithDuration(interval.Value)
                    .WithSource(dstEntityPacked)
                    .WithTag<TranslateFromStack2ToStack3PlayerTimerTag>()
                    .Submit();

                timerPacked.Unpack(m_World.Value, out var timerEntity);
                ref var translate = ref m_TranslatePool.Value.Add(timerEntity);
                translate.From = trigger.Sender;
                translate.To = dstEntityPacked;

                ref var timer = ref m_TimerPool.Value.Add(dstEntity);
                timer.Value = timerPacked;

                m_World.Value.DelEntity(triggerEntity);
            }
        }
    }
}
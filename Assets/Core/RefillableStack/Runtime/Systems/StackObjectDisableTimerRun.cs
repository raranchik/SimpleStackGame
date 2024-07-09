using Core.Logger;
using Core.RefillableStack.Links;
using Core.RefillableStack.Tags;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.RefillableStack.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class StackObjectDisableTimerRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<RefillableStackIntervalTimerTag, SourceComponent, DisableSelfRequest>
        > m_TimerFilter;

        private readonly EcsWorldInject m_World;
        private readonly ILogger m_Logger;
        private readonly EcsPoolInject<TimerTextLink> m_TimerTextPool;
        private readonly EcsPoolInject<IsDisabledTag> m_IsDisabledPool;

        public StackObjectDisableTimerRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(StackObjectDisableTimerRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_TimerFilter.Value)
            {
                ref var source = ref m_TimerFilter.Pools.Inc2.Get(timerEntity);
                if (!source.Value.Unpack(m_World.Value, out var sourceEntity))
                {
                    m_World.Value.DelEntity(timerEntity);
                    m_Logger.Log(LogType.Error, $"Source entity is dead. I kill timer for you");
                    continue;
                }

                ref var timerText = ref m_TimerTextPool.Value.Get(sourceEntity);
                timerText.value.gameObject.SetActive(false);
                m_IsDisabledPool.Value.Add(timerEntity);
                m_TimerFilter.Pools.Inc3.Del(timerEntity);
            }
        }
    }
}
using Core.Logger;
using Core.RefillableStack.Links;
using Core.RefillableStack.Tags;
using Core.Timer.Components;
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
    public class StackObjectTimerTickRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<RefillableStackIntervalTimerTag, IsIntervalTag,
                SourceComponent, ElapsedComponent, DurationComponent>,
            Exc<IsDisabledTag>
        > m_TimerFilter;

        private readonly EcsWorldInject m_World;
        private readonly ILogger m_Logger;
        private readonly EcsPoolInject<TimerTextLink> m_TimerTextPool;

        public StackObjectTimerTickRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(StackObjectTimerTickRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_TimerFilter.Value)
            {
                ref var source = ref m_TimerFilter.Pools.Inc3.Get(timerEntity);
                if (!source.Value.Unpack(m_World.Value, out var sourceEntity))
                {
                    m_World.Value.DelEntity(timerEntity);
                    m_Logger.Log(LogType.Error, $"Source entity is dead. I kill timer for you");
                    continue;
                }

                ref var timerText = ref m_TimerTextPool.Value.Get(sourceEntity);
                ref var elapsed = ref m_TimerFilter.Pools.Inc4.Get(timerEntity);
                ref var duration = ref m_TimerFilter.Pools.Inc5.Get(timerEntity);
                var value = Mathf.Clamp(duration.Value - elapsed.Value, 0f, duration.Value);
                timerText.value.text = value.ToString("0.#");
            }
        }
    }
}
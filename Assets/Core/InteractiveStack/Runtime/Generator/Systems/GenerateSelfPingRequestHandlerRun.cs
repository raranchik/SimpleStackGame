using Core.InteractiveStack.Generator.Components;
using Core.InteractiveStack.Generator.SelfRequests;
using Core.Logger;
using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.SelfRequests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Generator.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateSelfPingRequestHandlerRun : IEcsRunSystem
    {
        private readonly ILogger m_Logger;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<GenerateSelfRequest> m_GenerateRequestPool;

        private readonly EcsFilterInject<
            Inc<GenerateSelfPingRequestComponent, SourceComponent, SelfPingRequest, DurationComponent,
                IsIntervalTimerComponent>,
            Exc<TimerIsDisabledComponent>
        > m_TimerFilter;

        public GenerateSelfPingRequestHandlerRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(GenerateSelfPingRequestHandlerRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_TimerFilter.Value)
            {
                ref var sourceEntityComponent = ref m_TimerFilter.Pools.Inc2.Get(timerEntity);
                if (!sourceEntityComponent.Value.Unpack(m_World.Value, out var sourceEntity))
                {
                    m_World.Value.DelEntity(timerEntity);
                    m_Logger.Log(LogType.Error, $"Source entity is dead. I kill timer for you");
                    continue;
                }

                m_GenerateRequestPool.Value.Add(sourceEntity);
                m_TimerFilter.Pools.Inc3.Del(timerEntity);
            }
        }
    }
}
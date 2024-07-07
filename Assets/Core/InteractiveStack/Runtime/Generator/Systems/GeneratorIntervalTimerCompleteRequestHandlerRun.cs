using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.SelfRequests;
using Leopotam.EcsLite;
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
    public class GeneratorIntervalTimerCompleteRequestHandlerRun<TRequest> : IEcsRunSystem
        where TRequest : struct
    {
        private readonly ILogger m_Logger;
        private readonly EcsWorld m_World;
        private readonly EcsFilter m_TimerFilter;
        private readonly EcsPool<TRequest> m_RequestPool;
        private readonly EcsFilter m_RequestFilter;
        private readonly EcsPool<SourceComponent> m_SourcePool;
        private readonly EcsPool<CompleteSelfRequest> m_CompletePool;

        public GeneratorIntervalTimerCompleteRequestHandlerRun(ILogger logger, EcsWorld world, EcsFilter timerFilter)
        {
            m_Logger = logger;
            m_World = world;
            m_TimerFilter = timerFilter;
            m_RequestPool = m_World.GetPool<TRequest>();
            m_RequestFilter = m_World.Filter<TRequest>().End();
            m_SourcePool = m_World.GetPool<SourceComponent>();
            m_CompletePool = m_World.GetPool<CompleteSelfRequest>();
        }

        public void Run(IEcsSystems systems)
        {
            if (m_RequestFilter.GetEntitiesCount() > 0)
            {
                m_Logger.Log(LogType.Error,
                    $"Requests that have run 1 cycle are in system:" +
                    $" count {m_RequestFilter.GetEntitiesCount().ToString()}." +
                    $" I deleted them for you");

                foreach (var requestEntity in m_RequestFilter)
                {
                    m_World.DelEntity(requestEntity);
                }
            }

            if (m_TimerFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timer in m_TimerFilter)
            {
                ref var source = ref m_SourcePool.Get(timer);
                if (!source.Value.Unpack(m_World, out var sourceEntity))
                {
                    m_World.DelEntity(timer);
                    m_Logger.Log(LogType.Error, $"Source entity is dead. I kill timer for you");
                    continue;
                }

                var requestEntity = m_World.NewEntity();
                m_RequestPool.Add(requestEntity);
                m_CompletePool.Del(timer);
            }
        }
    }
}
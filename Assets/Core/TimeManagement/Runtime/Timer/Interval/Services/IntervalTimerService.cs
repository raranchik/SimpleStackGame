using Core.Logger;
using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.Interval.Components;
using Leopotam.EcsLite;
using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Timer.Interval.Services
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class IntervalTimerService
    {
        private readonly EcsWorld m_World;
        private readonly EcsPool<PingIntervalLink> m_PingIntervalLinkPool;
        private readonly EcsPool<TimerCounterComponent> m_CounterPool;
        private readonly ILogger m_Logger;

        public IntervalTimerService(EcsWorld world,
            EcsPool<PingIntervalLink> pingIntervalLinkPool,
            EcsPool<TimerCounterComponent> counterPool,
            ILogger logger)
        {
            m_World = world;
            m_PingIntervalLinkPool = pingIntervalLinkPool;
            m_CounterPool = counterPool;
            m_Logger = logger.WithPrefix(nameof(IntervalTimerService));
        }

        public bool CreateIntervalTimer(in EcsPackedEntity srcPackedEntity,
            out EcsPackedEntity dstPackedEntity)
        {
            if (!srcPackedEntity.Unpack(m_World, out var srcEntity))
            {
                dstPackedEntity = default;
                m_Logger.Log(LogType.Error, $"Source entity is dead: id<{srcEntity.ToString()}>");
                return false;
            }

            if (!m_PingIntervalLinkPool.Has(srcEntity))
            {
                dstPackedEntity = default;
                m_Logger.Log(LogType.Error,
                    $"Source entity has no required component:" +
                    $" id<{srcEntity.ToString()}>, component<{nameof(PingIntervalLink)}>");
                return false;
            }

            ref var pingInterval = ref m_PingIntervalLinkPool.Get(srcEntity);
            if (pingInterval.Value <= 0f)
            {
                dstPackedEntity = default;
                m_Logger.Log(LogType.Error,
                    $"Interval cannot be less than or equal to zero: id<{srcEntity.ToString()}>");
                return false;
            }

            var dstEntity = m_World.NewEntity();
            m_CounterPool.Add(dstEntity);
            m_PingIntervalLinkPool.Copy(srcEntity, dstEntity);
            dstPackedEntity = m_World.PackEntity(dstEntity);
            return true;
        }
    }
}
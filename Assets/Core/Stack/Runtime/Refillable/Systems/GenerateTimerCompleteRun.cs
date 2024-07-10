using Core.Stack.Refillable.Services;
using Core.Stack.Refillable.Tags;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
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
    public class GenerateTimerCompleteRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsCustomInject<RefillableService> m_RefillableService;

        private readonly EcsFilterInject<
            Inc<GenerateTimerTag, IsCompleteSelfRequest, IsIntervalTag, ParentComponent>,
            Exc<IsPausedTag>
        > m_Timers;

        public void Run(IEcsSystems systems)
        {
            if (m_Timers.Value.GetEntitiesCount() < 0)
            {
                return;
            }

            foreach (var timerEntity in m_Timers.Value)
            {
                ref var parent = ref m_Timers.Pools.Inc4.Get(timerEntity);
                parent.Value.Unpack(m_World.Value, out var parentEntity);
                if (m_RefillableService.Value.IsFullStack(parentEntity))
                {
                    m_RefillableService.Value.PauseTimerEntity(timerEntity);
                    continue;
                }

                var generateCount = m_RefillableService.Value.GetGenerateCount(parentEntity);
                for (var i = 0; i < generateCount; i++)
                {
                    m_RefillableService.Value.PushObject(parentEntity,
                        m_RefillableService.Value.CreateStackObject(parentEntity));
                }

                m_RefillableService.Value.UpdateGeneratorCounter(parentEntity);

                if (m_RefillableService.Value.IsFullStack(parentEntity))
                {
                    m_RefillableService.Value.PauseTimerEntity(timerEntity);
                }
            }
        }
    }
}
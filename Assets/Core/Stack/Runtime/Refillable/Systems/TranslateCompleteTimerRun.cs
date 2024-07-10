using Core.Stack.Base.Components;
using Core.Stack.Refillable.Services;
using Core.Stack.Refillable.Tags;
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
    public class TranslateCompleteTimerRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<RefillableService> m_RefillableService;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TranslateFromToComponent> m_Translate;

        private readonly EcsFilterInject<
            Inc<TranslateTimerTag, IsIntervalTag, IsCompleteSelfRequest>,
            Exc<IsPausedTag>
        > m_Timers;

        public void Run(IEcsSystems systems)
        {
            if (m_Timers.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_Timers.Value)
            {
                ref var fromTo = ref m_Translate.Value.Get(timerEntity);
                fromTo.From.Unpack(m_World.Value, out var fromEntity);
                fromTo.To.Unpack(m_World.Value, out var toEntity);

                if (m_RefillableService.Value.IsEmptyStack(fromEntity))
                {
                    m_RefillableService.Value.KillTimerEntity(timerEntity);
                    m_RefillableService.Value.RemoveTranslateTimerReference(fromEntity);
                    continue;
                }

                if (m_RefillableService.Value.IsFullStack(toEntity))
                {
                    m_RefillableService.Value.KillTimerEntity(timerEntity);
                    m_RefillableService.Value.RemoveTranslateTimerReference(fromEntity);
                    continue;
                }

                var objectPacked = m_RefillableService.Value.PopObject(fromEntity);
                objectPacked.Unpack(m_World.Value, out var objectEntity);
                m_RefillableService.Value.TranslateStackToPlayer(objectEntity, fromEntity, toEntity);
                m_RefillableService.Value.PushObject(toEntity, objectPacked);
                m_RefillableService.Value.UpdateGeneratorCounter(fromEntity);

                if (m_RefillableService.Value.HasSpotInStack(fromEntity))
                {
                    var generateTimerEntity = m_RefillableService.Value.GetGenerateTimer(fromEntity);
                    m_RefillableService.Value.UnpauseTimerEntity(generateTimerEntity);
                }
            }
        }
    }
}
using Core.Stack.Base.Components;
using Core.Stack.Void.Services;
using Core.Stack.Void.Tags;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
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
    public class CompleteTranslateTimerRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<VoidService> m_VoidService;
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

                var objectPacked = m_VoidService.Value.PopObject(toEntity);
                m_VoidService.Value.DestroyObject(objectPacked);

                if (m_VoidService.Value.IsEmptyStack(toEntity))
                {
                    m_VoidService.Value.KillTimerEntity(timerEntity);
                    m_VoidService.Value.RemoveTranslateTimerReference(fromEntity);
                }
            }
        }
    }
}
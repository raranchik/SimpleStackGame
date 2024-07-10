using Core.Stack.Refillable.Links;
using Core.Stack.Refillable.Tags;
using Core.TextUpdater.Requests;
using Core.Timer.Components;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Refillable.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateTimerTickRun : IEcsRunSystem
    {
        private readonly EcsPoolInject<UpdateTMPRequest> m_UpdateText;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<TimerTextLink> m_TimerTextPool;
        private readonly EcsPoolInject<UpdateTMPRequest> m_UpdateTMPPool;
        private readonly EcsPoolInject<ParentComponent> m_Parent;
        private readonly EcsPoolInject<ElapsedComponent> m_Elapsed;
        private readonly EcsPoolInject<DurationComponent> m_Duration;

        private readonly EcsFilterInject<
            Inc<GenerateTimerTag, IsTimerTag, ElapsedComponent, DurationComponent>,
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
                ref var parent = ref m_Parent.Value.Get(timerEntity);
                parent.Value.Unpack(m_World.Value, out var parentEntity);
                ref var timerText = ref m_TimerTextPool.Value.Get(parentEntity);
                ref var elapsed = ref m_Elapsed.Value.Get(timerEntity);
                ref var duration = ref m_Duration.Value.Get(timerEntity);
                var value = Mathf.Clamp(duration.Value - elapsed.Value, 0f, duration.Value);
                var text = value.ToString("0.#");
                ref var updateTMP = ref m_UpdateTMPPool.Value.Add(m_World.Value.NewEntity());
                updateTMP.TMP = timerText.Value;
                updateTMP.Value = text;
            }
        }
    }
}
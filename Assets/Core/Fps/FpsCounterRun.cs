using Core.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Fps
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class FpsCounterRun : IEcsRunSystem
    {
        private const float UpdateRate = 0.25f;

        private readonly EcsCustomInject<FpsCounter> m_Counter;
        private readonly EcsCustomInject<TimeService> m_TimeService;
        private float m_Timer;

        public void Run(IEcsSystems systems)
        {
            if (m_TimeService.Value.UnscaledTime > m_Timer)
            {
                var fps = Mathf.RoundToInt(1f / m_TimeService.Value.UnscaledDeltaTime);
                m_Counter.Value.SetText(fps.ToString());
                m_Timer = m_TimeService.Value.UnscaledTime + UpdateRate;
            }
        }
    }
}
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace TimeListener
{
    public class TimeRun : IEcsRunSystem
    {
        private readonly EcsCustomInject<TimeService> m_TimeServiceInject = default;

        public void Run(IEcsSystems systems)
        {
            var timeService = m_TimeServiceInject.Value;
            timeService.Time = Time.time;
            timeService.DeltaTime = Time.deltaTime;
            timeService.UnscaledTime = Time.unscaledTime;
            timeService.UnscaledDeltaTime = Time.unscaledDeltaTime;
        }
    }
}
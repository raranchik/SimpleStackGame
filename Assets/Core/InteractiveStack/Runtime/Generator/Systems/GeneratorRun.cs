using Core.InteractiveStack.Generator.Components;
using Core.InteractiveStack.Stack.Components;
using Core.TimeManagement.Timer.Components;
using Core.TimeManagement.Timer.Interval.SelfRequests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Generator.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GeneratorRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<GeneratorIntervalTimerComponent, SelfPingRequest>,
            Exc<TimerIsDisabledComponent>
        > m_GeneratorTimerFilterInject;

        public void Run(IEcsSystems systems)
        {
            var generatorFilter = m_GeneratorTimerFilterInject.Value;
            if (generatorFilter.GetEntitiesCount() <= 0)
            {
                return;
            }
        }
    }
}
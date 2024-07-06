using Core.InteractiveStack.Generator.Components;
using Core.InteractiveStack.Generator.Links;
using Core.MonoConverter;
using Core.TimeManagement.Timer.Services;
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
    public class GeneratorInit : IEcsInitSystem
    {
        private readonly MonoLinker[] m_GeneratorMonoLinkers;
        private readonly TimerService m_TimerService;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<GenerateIntervalTimerLink> m_IntervalPool;

        public GeneratorInit(MonoLinker[] generatorMonoLinkers, TimerService timerService)
        {
            m_GeneratorMonoLinkers = generatorMonoLinkers;
            m_TimerService = timerService;
        }

        public void Init(IEcsSystems systems)
        {
            foreach (var generatorMonoLinker in m_GeneratorMonoLinkers)
            {
                var generatorEntity = m_World.Value.NewEntity();
                generatorMonoLinker.LinkTo(m_World.Value.PackEntityWithWorld(generatorEntity));

                ref var interval = ref m_IntervalPool.Value.Get(generatorEntity);
                m_TimerService.DeclareTimer()
                    .WithInterval(interval.Value)
                    .WithSource(m_World.Value.PackEntity(generatorEntity))
                    .WithMarker<GenerateSelfPingRequestComponent>()
                    .Submit();
            }
        }
    }
}
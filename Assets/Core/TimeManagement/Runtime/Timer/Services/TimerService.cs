using Core.TimeManagement.Timer.Builders;
using Core.TimeManagement.Timer.Components;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TimeManagement.Timer.Services
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimerService
    {
        private readonly EcsWorld m_World;

        public TimerService(EcsWorld world)
        {
            m_World = world;
        }

        public TimerBuilder DeclareTimer()
        {
            var newEntity = m_World.NewEntity();
            var elapsedPool = m_World.GetPool<ElapsedTimeComponent>();
            elapsedPool.Add(newEntity);
            var sourcePool = m_World.GetPool<SourceComponent>();
            sourcePool.Add(newEntity);
            var builder = new TimerBuilder(m_World, newEntity);
            return builder;
        }
    }
}
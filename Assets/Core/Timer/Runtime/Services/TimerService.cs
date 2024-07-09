using Core.Timer.Builders;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Timer.Services
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
            var timer = m_World.NewEntity();
            var builder = new TimerBuilder(m_World, timer);
            return builder;
        }
    }
}
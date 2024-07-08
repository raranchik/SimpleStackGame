using Core.Timer.Components;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Timer.Builders
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TimerBuilder
    {
        private readonly EcsWorld m_World;
        private readonly int m_NewEntity;

        public TimerBuilder(EcsWorld world, int newEntity)
        {
            m_World = world;
            m_NewEntity = newEntity;
        }

        public TimerBuilder WithInterval(float value)
        {
            var durationComponentPool = m_World.GetPool<DurationComponent>();
            ref var durationComponent = ref durationComponentPool.Add(m_NewEntity);
            durationComponent.Value = value;
            var isIntervalTimerComponent = m_World.GetPool<IsIntervalTimerComponent>();
            isIntervalTimerComponent.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder WithMarker<T>() where T : struct
        {
            var pool = m_World.GetPool<T>();
            pool.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder AsDisabled()
        {
            var pool = m_World.GetPool<IsDisabledComponent>();
            pool.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder WithSource(EcsPackedEntity value)
        {
            var pool = m_World.GetPool<SourceComponent>();
            ref var component = ref pool.Get(m_NewEntity);
            component.Value = value;
            return this;
        }

        public EcsPackedEntity Submit()
        {
            return m_World.PackEntity(m_NewEntity);
        }

        public void Cancel()
        {
            m_World.DelEntity(m_NewEntity);
        }
    }
}
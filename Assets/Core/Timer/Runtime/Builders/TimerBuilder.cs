using Core.Timer.Components;
using Core.Timer.Tags;
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

        public TimerBuilder AsInterval()
        {
            var pool = m_World.GetPool<IsIntervalTag>();
            pool.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder WithDuration(float value)
        {
            var pool = m_World.GetPool<DurationComponent>();
            ref var duration = ref pool.Add(m_NewEntity);
            duration.Value = value;
            return this;
        }

        public TimerBuilder WithTag<T>() where T : struct
        {
            var pool = m_World.GetPool<T>();
            pool.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder AsDisabled()
        {
            var pool = m_World.GetPool<IsDisabledTag>();
            pool.Add(m_NewEntity);
            return this;
        }

        public TimerBuilder WithSource(in EcsPackedEntity value)
        {
            var pool = m_World.GetPool<SourceComponent>();
            ref var component = ref pool.Add(m_NewEntity);
            component.Value = value;
            return this;
        }

        public EcsPackedEntity Submit()
        {
            var elapsed = m_World.GetPool<ElapsedComponent>();
            elapsed.Add(m_NewEntity);
            return m_World.PackEntity(m_NewEntity);
        }

        public void Cancel()
        {
            m_World.DelEntity(m_NewEntity);
        }
    }
}
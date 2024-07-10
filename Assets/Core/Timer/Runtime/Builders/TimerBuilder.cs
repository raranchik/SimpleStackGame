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
            m_World.GetPool<ElapsedComponent>()
                .Add(m_NewEntity);
            m_World.GetPool<IsTimerTag>()
                .Add(m_NewEntity);
            m_World.GetPool<DurationComponent>()
                .Add(m_NewEntity);
        }

        public TimerBuilder AsInterval()
        {
            WithTag<IsIntervalTag>();
            return this;
        }

        public TimerBuilder WithDuration(float value)
        {
            ref var component = ref m_World.GetPool<DurationComponent>()
                .Get(m_NewEntity);
            component.Value = value;
            return this;
        }

        public TimerBuilder WithTag<T>() where T : struct
        {
            var pool = m_World.GetPool<T>();
            if (!pool.Has(m_NewEntity))
            {
                pool.Add(m_NewEntity);
            }

            return this;
        }

        public TimerBuilder AsDisabled()
        {
            WithTag<IsPausedTag>();
            return this;
        }

        public TimerBuilder WithParent(in EcsPackedEntity value)
        {
            ref var component = ref m_World.GetPool<ParentComponent>()
                .Add(m_NewEntity);
            component.Value = value;
            return this;
        }

        public EcsPackedEntity SubmitPacked()
        {
            return m_World.PackEntity(m_NewEntity);
        }

        public int SubmitUnpacked()
        {
            return m_NewEntity;
        }

        public void Cancel()
        {
            m_World.DelEntity(m_NewEntity);
        }
    }
}
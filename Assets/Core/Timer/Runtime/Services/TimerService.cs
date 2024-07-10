using Core.Timer.Builders;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
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
        private readonly EcsPool<KillSelfRequest> m_Kill;
        private readonly EcsPool<PauseSelfRequest> m_Pause;
        private readonly EcsPool<UnpauseSelfRequest> m_Unpause;
        private readonly EcsPool<ParentComponent> m_Parent;
        private readonly EcsPool<IsPausedTag> m_IsPaused;

        public TimerService(EcsWorld world)
        {
            m_World = world;
            m_Kill = m_World.GetPool<KillSelfRequest>();
            m_Pause = m_World.GetPool<PauseSelfRequest>();
            m_Unpause = m_World.GetPool<UnpauseSelfRequest>();
            m_Parent = m_World.GetPool<ParentComponent>();
            m_IsPaused = m_World.GetPool<IsPausedTag>();
        }

        public TimerBuilder DeclareTimer()
        {
            var timer = m_World.NewEntity();
            var builder = new TimerBuilder(m_World, timer);
            return builder;
        }

        public EcsPackedEntity GetParent(in EcsPackedEntity packed)
        {
            packed.Unpack(m_World, out var entity);
            return GetParent(entity);
        }

        private EcsPackedEntity GetParent(in int entity)
        {
            return m_Parent.Get(entity).Value;
        }

        public void Kill(in EcsPackedEntity packed)
        {
            packed.Unpack(m_World, out var entity);
            Kill(entity);
        }

        public void Kill(in int entity)
        {
            if (!m_Kill.Has(entity))
            {
                m_Kill.Add(entity);
            }
        }

        public void Pause(in EcsPackedEntity packed)
        {
            packed.Unpack(m_World, out var entity);
            Pause(entity);
        }

        public void Pause(in int entity)
        {
            if (!m_IsPaused.Has(entity) && !m_Pause.Has(entity))
            {
                m_Pause.Add(entity);
            }
        }

        public void Unpause(in EcsPackedEntity packed)
        {
            packed.Unpack(m_World, out var entity);
            Unpause(entity);
        }

        public void Unpause(in int entity)
        {
            if (m_IsPaused.Has(entity) && !m_Unpause.Has(entity))
            {
                m_Unpause.Add(entity);
            }
        }
    }
}
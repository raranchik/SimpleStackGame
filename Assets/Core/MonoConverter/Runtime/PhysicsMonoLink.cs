using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public abstract class PhysicsMonoLink<TMarker, TPhysicsRequest> : PhysicsMonoLinkBase
        where TMarker : struct
        where TPhysicsRequest : struct
    {
        protected EcsPool<TMarker> m_MarkerPool;
        protected EcsPool<TPhysicsRequest> m_PhysicsRequestPool;
        protected EcsWorld m_World;
        protected EcsPackedEntity m_Value;

        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
            if (!packedEntityWithWorld.Unpack(out var world, out var entity))
            {
                return;
            }

            m_World = world;
            m_MarkerPool = m_World.GetPool<TMarker>();
            m_PhysicsRequestPool = m_World.GetPool<TPhysicsRequest>();
            m_Value = m_World.PackEntity(entity);
        }
    }
}
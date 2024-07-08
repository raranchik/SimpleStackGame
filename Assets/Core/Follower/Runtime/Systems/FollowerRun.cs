using Core.Follower.Links;
using Core.Follower.Requests;
using Core.MonoConverter.Links;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Follower.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class FollowerRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<FollowerFollowRequest>> m_FollowFilter;
        private readonly EcsPoolInject<FollowerTargetLink> m_TargetPool;
        private readonly EcsPoolInject<FollowerOffsetLink> m_OffsetPool;
        private readonly EcsPoolInject<TransformLink> m_TransformPool;

        public void Run(IEcsSystems systems)
        {
            if (m_FollowFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var followRequestEntity in m_FollowFilter.Value)
            {
                ref var followRequest = ref m_FollowFilter.Pools.Inc1.Get(followRequestEntity);
                if (!followRequest.Value.Unpack(m_World.Value, out var followerEntity))
                {
                    m_World.Value.DelEntity(followRequestEntity);
                    continue;
                }

                ref var target = ref m_TargetPool.Value.Get(followerEntity);
                ref var offset = ref m_OffsetPool.Value.Get(followerEntity);
                ref var transform = ref m_TransformPool.Value.Get(followerEntity);

                var position = target.Value.position + offset.Value;
                transform.Value.position = position;

                m_World.Value.DelEntity(followRequestEntity);
            }
        }
    }
}
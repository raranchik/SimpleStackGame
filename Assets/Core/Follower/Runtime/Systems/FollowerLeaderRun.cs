using Core.Follower.Links;
using Core.Follower.SelfRequests;
using Core.MonoConverter.Links;
using Core.Movement.Move.Request;
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
    public class FollowerLeaderRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TransformLink, FollowerTargetLink, FollowerOffsetLink>>
            m_FollowerFilter;

        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<MoveRequest>> m_MoveRequestFilter;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPool;
        private readonly EcsPoolInject<FollowerTargetLink> m_TargetPool;
        private readonly EcsPoolInject<FollowerFollowSelfRequest> m_FollowerSelfRequestPool;

        public void Run(IEcsSystems systems)
        {
            if (m_MoveRequestFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            if (m_FollowerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var moveRequestEntity in m_MoveRequestFilter.Value)
            {
                ref var moveRequest = ref m_MoveRequestFilter.Pools.Inc1.Get(moveRequestEntity);
                ref var packedEntity = ref moveRequest.Value;
                if (!packedEntity.Unpack(m_World.Value, out var moveEntity))
                {
                    continue;
                }

                ref var transformLink = ref m_TransformLinkPool.Value.Get(moveEntity);
                foreach (var followerEntity in m_FollowerFilter.Value)
                {
                    ref var targetLink = ref m_TargetPool.Value.Get(followerEntity);
                    if (targetLink.Value != transformLink.Value)
                    {
                        continue;
                    }

                    m_FollowerSelfRequestPool.Value.Add(followerEntity);
                }
            }
        }
    }
}
using Core.MonoConverter.Links;
using Core.Movement.Move;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Core.Follower
{
    public class FollowerLeaderRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<TransformLink, FollowerTargetLink, FollowerOffsetLink>>
            m_FollowerFilterInject;

        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsFilterInject<Inc<MoveRequest>> m_MoveRequestFilterInject;
        private readonly EcsPoolInject<MoveRequest> m_MoveRequestPoolInject;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPool;
        private readonly EcsPoolInject<FollowerTargetLink> m_TargetPoolInject;
        private readonly EcsPoolInject<FollowerFollowSelfRequest> m_FollowerSelfRequestPoolInject;

        public void Run(IEcsSystems systems)
        {
            var moveRequestFilter = m_MoveRequestFilterInject.Value;
            if (moveRequestFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var followerFilter = m_FollowerFilterInject.Value;
            if (followerFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = m_WorldInject.Value;
            var transformLinkPool = m_TransformLinkPool.Value;
            var moveRequestPool = m_MoveRequestPoolInject.Value;
            var targetPool = m_TargetPoolInject.Value;
            var followerSelfRequestPool = m_FollowerSelfRequestPoolInject.Value;
            foreach (var moveRequestEntity in moveRequestFilter)
            {
                ref var moveRequest = ref moveRequestPool.Get(moveRequestEntity);
                ref var packedEntity = ref moveRequest.PackedEntity;
                if (!packedEntity.Unpack(world, out var moveEntity))
                {
                    continue;
                }

                ref var transformLink = ref transformLinkPool.Get(moveEntity);
                foreach (var followerEntity in followerFilter)
                {
                    ref var targetLink = ref targetPool.Get(followerEntity);
                    if (targetLink.TargetTransform != transformLink.Transform)
                    {
                        continue;
                    }

                    followerSelfRequestPool.Add(followerEntity);
                }
            }
        }
    }
}
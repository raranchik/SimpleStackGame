using Core.MonoConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Core.Follower
{
    public class FollowerRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<FollowerFollowSelfRequest, FollowerTargetLink, FollowerOffsetLink, TransformLink>
        > m_FollowerFilterInject;

        public void Run(IEcsSystems systems)
        {
            var followerFilter = m_FollowerFilterInject.Value;
            if (followerFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var pools = m_FollowerFilterInject.Pools;
            var followSelfRequestPool = pools.Inc1;
            var targetLinkPool = pools.Inc2;
            var offsetLinkPool = pools.Inc3;
            var transformLinkPool = pools.Inc4;
            foreach (var followerEntity in followerFilter)
            {
                ref var targetLink = ref targetLinkPool.Get(followerEntity);
                ref var offsetLink = ref offsetLinkPool.Get(followerEntity);
                ref var transformLink = ref transformLinkPool.Get(followerEntity);

                var position = targetLink.Value.position + offsetLink.Value;
                transformLink.Value.position = position;

                followSelfRequestPool.Del(followerEntity);
            }
        }
    }
}
using Core.Follower.Links;
using Core.Follower.SelfRequests;
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
        private readonly EcsFilterInject<
            Inc<FollowerFollowSelfRequest, FollowerTargetLink, FollowerOffsetLink, TransformLink>
        > m_FollowerFilter;

        public void Run(IEcsSystems systems)
        {
            if (m_FollowerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var followerEntity in m_FollowerFilter.Value)
            {
                ref var targetLink = ref m_FollowerFilter.Pools.Inc2.Get(followerEntity);
                ref var offsetLink = ref m_FollowerFilter.Pools.Inc3.Get(followerEntity);
                ref var transformLink = ref m_FollowerFilter.Pools.Inc4.Get(followerEntity);

                var position = targetLink.Value.position + offsetLink.Value;
                transformLink.Value.position = position;

                m_FollowerFilter.Pools.Inc1.Del(followerEntity);
            }
        }
    }
}
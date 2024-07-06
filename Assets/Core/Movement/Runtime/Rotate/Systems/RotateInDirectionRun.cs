using Core.DevicesInput.Requests;
using Core.MonoConverter.Links;
using Core.Movement.Rotate.Links;
using Core.TimeManagement.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Movement.Rotate.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class RotateInDirectionRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<RotateInDirectionRequest> m_RotateInDirRequestPool;
        private readonly EcsFilterInject<Inc<RotateInDirectionRequest>> m_RotateInDirRequestFilter;
        private readonly EcsFilterInject<Inc<TransformLink, RotateMaxSpeedLink>> m_RotateLinkFilter;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPool;
        private readonly EcsPoolInject<RotateMaxSpeedLink> m_RotateLinkPool;
        private readonly EcsCustomInject<TimeListener> m_TimeService;

        public void Run(IEcsSystems systems)
        {
            var rotateInDirRequestFilter = m_RotateInDirRequestFilter.Value;
            if (rotateInDirRequestFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var rotateLinkFilter = m_RotateLinkFilter.Value;
            if (rotateLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = m_World.Value;
            var transformLinkPool = m_TransformLinkPool.Value;
            var rotateInDirRequestPool = m_RotateInDirRequestPool.Value;
            var timeService = m_TimeService.Value;
            var rotateLinkPool = m_RotateLinkPool.Value;
            foreach (var rotateLinkEntity in rotateLinkFilter)
            {
                ref var transformLink = ref transformLinkPool.Get(rotateLinkEntity);
                var transform = transformLink.Value;
                ref var rotateLink = ref rotateLinkPool.Get(rotateLinkEntity);
                foreach (var rotateInDirRequestEntity in rotateInDirRequestFilter)
                {
                    ref var rotateInDirRequest = ref rotateInDirRequestPool.Get(rotateInDirRequestEntity);
                    ref var direction = ref rotateInDirRequest.Direction;
                    var toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    var rotation = Quaternion.RotateTowards(transform.rotation, toRotation,
                        rotateLink.Value * timeService.DeltaTime);
                    transform.rotation = rotation;

                    world.DelEntity(rotateInDirRequestEntity);
                }
            }
        }
    }
}
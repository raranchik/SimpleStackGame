using Core.DevicesInput;
using Core.MonoConverter;
using Core.TimeManagement.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Core.Movement.Rotate
{
    public class RotateInDirectionRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsPoolInject<RotateInDirectionRequest> m_RotateInDirRequestPoolInject;
        private readonly EcsFilterInject<Inc<RotateInDirectionRequest>> m_RotateInDirRequestFilterInject;
        private readonly EcsFilterInject<Inc<TransformLink, RotateMaxSpeedLink>> m_RotateLinkFilterInject;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPoolInject;
        private readonly EcsPoolInject<RotateMaxSpeedLink> m_RotateLinkPoolInject;
        private readonly EcsCustomInject<TimeListener> m_TimeServiceInject;

        public void Run(IEcsSystems systems)
        {
            var rotateInDirRequestFilter = m_RotateInDirRequestFilterInject.Value;
            if (rotateInDirRequestFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var rotateLinkFilter = m_RotateLinkFilterInject.Value;
            if (rotateLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = m_WorldInject.Value;
            var transformLinkPool = m_TransformLinkPoolInject.Value;
            var rotateInDirRequestPool = m_RotateInDirRequestPoolInject.Value;
            var timeService = m_TimeServiceInject.Value;
            var rotateLinkPool = m_RotateLinkPoolInject.Value;
            foreach (var rotateLinkEntity in rotateLinkFilter)
            {
                ref var transformLink = ref transformLinkPool.Get(rotateLinkEntity);
                var transform = transformLink.Value;
                ref var rotateLink = ref rotateLinkPool.Get(rotateLinkEntity);
                foreach (var rotateInDirRequestEntity in rotateInDirRequestFilter)
                {
                    ref var rotateInDirRequest = ref rotateInDirRequestPool.Get(rotateInDirRequestEntity);
                    ref var direction = ref rotateInDirRequest.Value;
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
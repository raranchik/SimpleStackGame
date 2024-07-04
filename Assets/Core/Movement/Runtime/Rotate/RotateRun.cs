using Core.JoystickInput;
using Core.Movement.Move;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MonoConverter.Links;
using TimeListener;
using UnityEngine;

namespace Core.Movement.Rotate
{
    public class RotateRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsPoolInject<JoystickMotionRequest> m_MotionRequestPoolInject;
        private readonly EcsFilterInject<Inc<JoystickMotionRequest>> m_MotionRequestFilterInject;
        private readonly EcsFilterInject<Inc<TransformLink, MoveLink, RotateLink>> m_PlayerLinkFilterInject;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPoolInject;
        private readonly EcsPoolInject<RotateLink> m_RotationLinkPoolInject;
        private readonly EcsCustomInject<TimeService> m_TimeServiceInject;

        public void Run(IEcsSystems systems)
        {
            var motionRequestFilter = m_MotionRequestFilterInject.Value;
            var count = motionRequestFilter.GetEntitiesCount();
            if (count <= 0 || count > 1)
            {
                return;
            }

            var playerLinkFilter = m_PlayerLinkFilterInject.Value;
            count = playerLinkFilter.GetEntitiesCount();
            if (count <= 0 || count > 1)
            {
                return;
            }

            var playerLinkEntity = playerLinkFilter.GetRawEntities()[0];
            var transformLinkPool = m_TransformLinkPoolInject.Value;
            ref var transformLink = ref transformLinkPool.Get(playerLinkEntity);
            var transform = transformLink.Transform;

            var motionRequestEntity = motionRequestFilter.GetRawEntities()[0];
            var motionRequestPool = m_MotionRequestPoolInject.Value;
            ref var motionRequest = ref motionRequestPool.Get(motionRequestEntity);
            var direction = motionRequest.Direction;

            var timeService = m_TimeServiceInject.Value;

            var rotationLinkPool = m_RotationLinkPoolInject.Value;
            ref var rotationLink = ref rotationLinkPool.Get(playerLinkEntity);
            var toRotation = Quaternion.LookRotation(direction, Vector3.up);
            var rotation = Quaternion.RotateTowards(transform.rotation, toRotation,
                rotationLink.m_Speed * timeService.DeltaTime);
            transform.rotation = rotation;

            var world = m_WorldInject.Value;
            world.DelEntity(motionRequestEntity);
        }
    }
}
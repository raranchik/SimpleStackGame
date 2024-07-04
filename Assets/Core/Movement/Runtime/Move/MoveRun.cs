using Core.JoystickInput;
using Core.Movement.Rotate;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MonoConverter.Links;
using TimeListener;
using UnityEngine;

namespace Core.Movement.Move
{
    public class MoveRun : IEcsRunSystem
    {
        private readonly EcsPoolInject<JoystickMotionRequest> m_MotionRequestPoolInject;
        private readonly EcsFilterInject<Inc<JoystickMotionRequest>> m_MotionRequestFilterInject;
        private readonly EcsFilterInject<Inc<TransformLink, MoveLink, RotateLink>> m_PlayerLinkFilterInject;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPoolInject;
        private readonly EcsPoolInject<MoveLink> m_MovementLinkPoolInject;
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

            var movementLinkPool = m_MovementLinkPoolInject.Value;
            ref var movementLink = ref movementLinkPool.Get(playerLinkEntity);
            var movementSpeed = direction * movementLink.m_Speed * timeService.DeltaTime;
            transform.Translate(movementSpeed, Space.World);
        }
    }
}
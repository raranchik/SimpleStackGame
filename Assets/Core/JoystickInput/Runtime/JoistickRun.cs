using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Core.JoystickInput
{
    public class JoystickRun : IEcsRunSystem
    {
        private const float InputThresholdSqr = 0.01f;

        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsFilterInject<Inc<JoystickLink>> m_JoystickLinkFilterInject;
        private readonly EcsPoolInject<JoystickLink> m_JoystickLinkPoolInject;
        private readonly EcsPoolInject<JoystickMotionRequest> m_MotionRequestPoolInject;

        public void Run(IEcsSystems systems)
        {
            var joystickLinkFilter = m_JoystickLinkFilterInject.Value;
            var count = joystickLinkFilter.GetEntitiesCount();
            if (count <= 0 || count > 1)
            {
                return;
            }

            var world = m_WorldInject.Value;

            var joystickLinkEntity = joystickLinkFilter.GetRawEntities()[0];
            var joystickLinkPool = m_JoystickLinkPoolInject.Value;
            ref var joystickLink = ref joystickLinkPool.Get(joystickLinkEntity);
            var joystick = joystickLink.m_Joystick;

            var direction2 = joystick.Direction;
            if (!(direction2.sqrMagnitude >= InputThresholdSqr))
            {
                return;
            }

            direction2.Normalize();
            var direction3 = new Vector3(direction2.x, 0f, direction2.y);

            var motionRequestPool = m_MotionRequestPoolInject.Value;
            var requestEntity = world.NewEntity();
            ref var motionRequest = ref motionRequestPool
                .Add(requestEntity);
            motionRequest.Direction = direction3;
        }
    }
}
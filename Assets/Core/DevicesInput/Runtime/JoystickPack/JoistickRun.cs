using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Core.DevicesInput.JoystickPack
{
    public class JoystickRun : IEcsRunSystem
    {
        private const float InputThresholdSqr = 0.01f;

        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsFilterInject<Inc<JoystickLink>> m_JoystickLinkFilterInject;
        private readonly EcsPoolInject<JoystickLink> m_JoystickLinkPoolInject;
        private readonly EcsPoolInject<MoveInDirectionRequest> m_MoveInDirectionPoolInject;
        private readonly EcsPoolInject<RotateInDirectionRequest> m_RotateInDirectionPoolInject;

        public void Run(IEcsSystems systems)
        {
            var joystickLinkFilter = m_JoystickLinkFilterInject.Value;
            if (joystickLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = m_WorldInject.Value;
            var joystickLinkPool = m_JoystickLinkPoolInject.Value;
            foreach (var joystickLinkEntity in joystickLinkFilter)
            {
                ref var joystickLink = ref joystickLinkPool.Get(joystickLinkEntity);
                var joystick = joystickLink.Value;
                var direction2 = joystick.Direction;
                if (!(direction2.sqrMagnitude >= InputThresholdSqr))
                {
                    return;
                }

                var moveInDirPool = m_MoveInDirectionPoolInject.Value;
                var rotateInDirPool = m_RotateInDirectionPoolInject.Value;

                direction2.Normalize();
                var direction3 = new Vector3(direction2.x, 0f, direction2.y);

                var moveRequestEntity = world.NewEntity();
                ref var moveRequest = ref moveInDirPool.Add(moveRequestEntity);
                moveRequest.Value = direction3;

                var rotateRequestEntity = world.NewEntity();
                ref var rotateRequest = ref rotateInDirPool.Add(rotateRequestEntity);
                rotateRequest.Value = direction3;
            }
        }
    }
}
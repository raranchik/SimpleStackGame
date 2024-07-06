using Core.DevicesInput.JoystickPack.Links;
using Core.DevicesInput.Requests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.DevicesInput.JoystickPack.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class JoystickRun : IEcsRunSystem
    {
        private const float InputThresholdSqr = 0.01f;

        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<JoystickLink>> m_JoystickLinkFilter;
        private readonly EcsPoolInject<JoystickLink> m_JoystickLinkPool;
        private readonly EcsPoolInject<MoveInDirectionRequest> m_MoveInDirectionPool;
        private readonly EcsPoolInject<RotateInDirectionRequest> m_RotateInDirectionPool;

        public void Run(IEcsSystems systems)
        {
            var joystickLinkFilter = m_JoystickLinkFilter.Value;
            if (joystickLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var world = m_World.Value;
            var joystickLinkPool = m_JoystickLinkPool.Value;
            foreach (var joystickLinkEntity in joystickLinkFilter)
            {
                ref var joystickLink = ref joystickLinkPool.Get(joystickLinkEntity);
                var joystick = joystickLink.Value;
                var direction2 = joystick.Direction;
                if (!(direction2.sqrMagnitude >= InputThresholdSqr))
                {
                    return;
                }

                var moveInDirPool = m_MoveInDirectionPool.Value;
                var rotateInDirPool = m_RotateInDirectionPool.Value;

                direction2.Normalize();
                var direction3 = new Vector3(direction2.x, 0f, direction2.y);

                var moveRequestEntity = world.NewEntity();
                ref var moveRequest = ref moveInDirPool.Add(moveRequestEntity);
                moveRequest.Direction = direction3;

                var rotateRequestEntity = world.NewEntity();
                ref var rotateRequest = ref rotateInDirPool.Add(rotateRequestEntity);
                rotateRequest.Direction = direction3;
            }
        }
    }
}
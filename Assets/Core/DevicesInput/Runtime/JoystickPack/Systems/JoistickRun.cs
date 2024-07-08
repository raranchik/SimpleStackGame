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
        private readonly EcsFilterInject<Inc<JoystickLink>> m_JoystickFilter;

        private readonly EcsPoolInject<IsStartMoveInDirectionRequest> m_IsStartMoveInDirPool;
        private readonly EcsPoolInject<MoveInDirectionRequest> m_MoveInDirPool;
        private readonly EcsPoolInject<IsEndMoveInDirectionRequest> m_IsEndMoveInDirPool;

        private readonly EcsPoolInject<IsStartRotateInDirection> m_IsStartRotateInDirPool;
        private readonly EcsPoolInject<RotateInDirectionRequest> m_RotateInDirPool;

        private bool m_IsHandleInput;

        public void Run(IEcsSystems systems)
        {
            if (m_JoystickFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var joystickEntity in m_JoystickFilter.Value)
            {
                ref var joystick = ref m_JoystickFilter.Pools.Inc1.Get(joystickEntity);
                var direction2 = joystick.Value.Direction;
                if (direction2.sqrMagnitude < InputThresholdSqr)
                {
                    if (m_IsHandleInput)
                    {
                        m_IsHandleInput = false;
                        CreateEndMoveInDirectionRequest();
                    }

                    return;
                }

                direction2.Normalize();
                var direction3 = new Vector3(direction2.x, 0f, direction2.y);

                if (!m_IsHandleInput)
                {
                    m_IsHandleInput = true;
                    CreateStartMoveInDirectionRequest(direction3);
                    CreateStartRotateInDirectionRequest(direction3);
                }
                else
                {
                    m_IsHandleInput = true;
                    CreateMoveInDirectionRequest(direction3);
                    CreateRotateInDirectionRequest(direction3);
                }
            }
        }

        private void CreateStartMoveInDirectionRequest(in Vector3 direction)
        {
            var entity = m_World.Value.NewEntity();
            ref var request = ref m_IsStartMoveInDirPool.Value.Add(entity);
            request.Value = direction;
        }

        private void CreateMoveInDirectionRequest(in Vector3 direction)
        {
            var entity = m_World.Value.NewEntity();
            ref var request = ref m_MoveInDirPool.Value.Add(entity);
            request.Value = direction;
        }

        private void CreateEndMoveInDirectionRequest()
        {
            var entity = m_World.Value.NewEntity();
            m_IsEndMoveInDirPool.Value.Add(entity);
        }

        private void CreateStartRotateInDirectionRequest(in Vector3 direction)
        {
            var entity = m_World.Value.NewEntity();
            ref var request = ref m_IsStartRotateInDirPool.Value.Add(entity);
            request.Value = direction;
        }

        private void CreateRotateInDirectionRequest(in Vector3 direction)
        {
            var entity = m_World.Value.NewEntity();
            ref var request = ref m_RotateInDirPool.Value.Add(entity);
            request.Value = direction;
        }
    }
}
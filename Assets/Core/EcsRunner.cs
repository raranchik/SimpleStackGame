using System.Collections.Generic;
using System.Linq;
using Core.DevicesInput.JoystickPack;
using Core.Follower;
using Core.MonoConverter;
using Core.Movement.Move;
using Core.Movement.Rotate;
using Core.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TimeListener;
using UnityEngine;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

namespace Core
{
    public class EcsRunner : MonoBehaviour
    {
        [SerializeField] private MonoLinker m_JoystickLinker;
        [SerializeField] private MonoLinker m_PlayerLinker;
        [SerializeField] private MonoLinker m_CameraLinker;

        private EcsWorld m_World;
        private IEcsSystems m_UpdateSystems;
        private IEcsSystems m_LateUpdateSystems;
        private IEcsSystems m_InitSystems;

        private void Awake()
        {
            m_World = new EcsWorld();

            m_InitSystems = CreateInitSystems();
            m_InitSystems.Init();

            m_UpdateSystems = CreateUpdateSystems();
            m_UpdateSystems.Init();

            m_LateUpdateSystems = CreateLateUpdateSystems();
            m_LateUpdateSystems.Init();
        }

        private void Update()
        {
            m_UpdateSystems?.Run();
        }

        private void LateUpdate()
        {
            m_LateUpdateSystems?.Run();
        }

        private void OnDestroy()
        {
            m_InitSystems?.Destroy();
            m_InitSystems = null;

            m_UpdateSystems?.Destroy();
            m_UpdateSystems = null;

            m_LateUpdateSystems?.Destroy();
            m_LateUpdateSystems = null;

            m_World?.Destroy();
            m_World = null;
        }

        private IEcsSystems CreateInitSystems()
        {
            return new EcsSystems(m_World)
                .Add(new JoystickInit(m_JoystickLinker))
                .Add(new PlayerInit(m_PlayerLinker))
                .Add(new FollowerInit(m_CameraLinker))
#if UNITY_EDITOR
                .Add(new EcsSystemsDebugSystem())
                .Add(new EcsWorldDebugSystem())
#endif
                .Inject();
        }

        private IEcsSystems CreateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new TimeRun())
                .Add(new JoystickRun())
                .Add(new MoveInDirectionRun())
                .Add(new RotateInDirectionRun())
                .Add(new FollowerLeaderRun())
                .Inject(CreateUpdateInjectParams());
        }

        private IEcsSystems CreateLateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new FollowerRun())
                .Inject();
        }

        private object[] CreateUpdateInjectParams()
        {
            return new HashSet<object>
                {
                    new TimeService(),
                }
                .ToArray();
        }
    }
}
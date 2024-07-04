using System;
using System.Collections.Generic;
using System.Linq;
using Core.Follower;
using Core.JoystickInput;
using Core.Movement.Move;
using Core.Movement.Rotate;
using Core.Player;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MonoConverter;
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

        private EcsWorld m_World;
        private IEcsSystems m_UpdateSystems;
        private IEcsSystems m_LateUpdateSystems;

        private void Awake()
        {
            m_World = new EcsWorld();

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
            m_UpdateSystems?.Destroy();
            m_UpdateSystems = null;

            m_LateUpdateSystems?.Destroy();
            m_LateUpdateSystems = null;

            m_World?.Destroy();
            m_World = null;
        }

        private IEcsSystems CreateLateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new FollowerRun())
#if UNITY_EDITOR
                .Add(new EcsSystemsDebugSystem())
                .Add(new EcsWorldDebugSystem())
#endif
                .Inject();
        }

        private IEcsSystems CreateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new JoystickInit(m_JoystickLinker))
                .Add(new PlayerInit(m_PlayerLinker))
                .Add(new TimeRun())
                .Add(new JoystickRun())
                .Add(new MoveRun())
                .Add(new RotateRun())
#if UNITY_EDITOR
                .Add(new EcsSystemsDebugSystem())
                .Add(new EcsWorldDebugSystem())
#endif
                .Inject(CreateUpdateInjectParams());
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
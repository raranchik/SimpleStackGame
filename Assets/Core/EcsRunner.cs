using System.Collections.Generic;
using System.Linq;
using Core.DevicesInput.JoystickPack.Systems;
using Core.Follower.Systems;
using Core.MonoConverter;
using Core.Movement.Move.Systems;
using Core.Movement.Rotate.Systems;
using Core.Player.Systems;
using Core.Time;
using Core.Timer.Services;
using Core.Timer.Systems;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

namespace Core
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class EcsRunner : MonoBehaviour
    {
        [SerializeField] private MonoLinker m_JoystickLinker;
        [SerializeField] private MonoLinker m_PlayerLinker;
        [SerializeField] private MonoLinker m_CameraLinker;
        [SerializeField] private MonoLinker[] m_Generators;
        [SerializeField] private MonoLinker m_ObjectPrefab;

        private EcsWorld m_World;

        private IEcsSystems m_UpdateSystems;
        private IEcsSystems m_LateUpdateSystems;
        private IEcsSystems m_InitSystems;
        private IEcsSystems m_FixedUpdateSystems;
#if UNITY_EDITOR
        private IEcsSystems m_LeoEcsDebugSystems;
#endif

        private TimeService m_TimeService;
        private TimerService m_TimerService;

        private void Awake()
        {
            m_TimeService = new TimeService();
            m_TimerService = new TimerService(m_World);

            m_World = new EcsWorld();

            m_InitSystems = CreateInitSystems();
            m_InitSystems.Init();

            m_UpdateSystems = CreateUpdateSystems();
            m_UpdateSystems.Init();

            m_FixedUpdateSystems = CreateFixedUpdateSystems();
            m_FixedUpdateSystems.Init();

            m_LateUpdateSystems = CreateLateUpdateSystems();
            m_LateUpdateSystems.Init();
#if UNITY_EDITOR
            m_LeoEcsDebugSystems = CreateLeoEcsDebugSystems();
            m_LeoEcsDebugSystems.Init();
#endif
        }

        private void Update()
        {
            m_UpdateSystems?.Run();
#if UNITY_EDITOR
            m_LeoEcsDebugSystems?.Run();
#endif
        }

        private void FixedUpdate()
        {
            m_FixedUpdateSystems?.Run();
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

            m_FixedUpdateSystems?.Destroy();
            m_FixedUpdateSystems = null;

            m_LateUpdateSystems?.Destroy();
            m_LateUpdateSystems = null;

            m_World?.Destroy();
            m_World = null;
#if UNITY_EDITOR
            m_LeoEcsDebugSystems?.Destroy();
            m_LeoEcsDebugSystems = null;
#endif
        }

        private IEcsSystems CreateInitSystems()
        {
            return new EcsSystems(m_World)
                .Add(new JoystickInit(m_JoystickLinker))
                .Add(new PlayerAvatarInit(m_PlayerLinker))
                .Add(new FollowerInit(m_CameraLinker))
                // .Add(new GeneratorInit(m_Generators, m_TimerService))
                .Inject();
        }

        private IEcsSystems CreateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new TimeServiceRun())
                .Add(new IntervalTimerRun(Debug.unityLogger))
                // .Add(new GeneratorIntervalTimerCompleteSelfRequestHandlerRun(Debug.unityLogger))
                // .Add(new GenerateSelfRequestHandlerRun(Debug.unityLogger,
                //     new EntityOutPool(new EntityOutFactory(m_World, m_ObjectPrefab), 5)))
                .Add(new JoystickRun())
                .Inject(CreateUpdateInjectParams());
        }

        private IEcsSystems CreateFixedUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new EndMoveInDirectionPlayerAvatarRun())
                .Add(new StartMoveInDirectionPlayerAvatarRun())
                .Add(new MoveInDirectionPlayerAvatarRun())
                .Add(new StartRotateInDirectionPlayerAvatarRun())
                .Add(new RotateInDirectionPlayerAvatarRun())
                .Add(new FollowerLeaderRun())
                .Inject(CreateFixedUpdateInjectParams());
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
                    m_TimeService,
                }
                .ToArray();
        }

        private object[] CreateFixedUpdateInjectParams()
        {
            return new HashSet<object>
                {
                    m_TimeService,
                }
                .ToArray();
        }
#if UNITY_EDITOR
        private IEcsSystems CreateLeoEcsDebugSystems()
        {
            return new EcsSystems(m_World)
                .Add(new EcsSystemsDebugSystem())
                .Add(new EcsWorldDebugSystem())
                .Inject();
        }
#endif
    }
}
using System.Collections.Generic;
using System.Linq;
using Core.DevicesInput.JoystickPack.Systems;
using Core.Follower.Systems;
using Core.InteractiveStack.Generator.Systems;
using Core.MonoConverter;
using Core.MonoConverter.Factory;
using Core.MonoConverter.Pool;
using Core.Movement.Move.Systems;
using Core.Movement.Rotate.Systems;
using Core.Player;
using Core.TimeManagement.Time;
using Core.TimeManagement.Timer.Services;
using Core.TimeManagement.Timer.Systems;
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
        private TimeListener m_TimeListener;
        private TimerService m_TimerService;

        private void Awake()
        {
            m_TimeListener = new TimeListener();
            m_World = new EcsWorld();
            m_TimerService = new TimerService(m_World);

            m_InitSystems = CreateInitSystems();
            m_InitSystems.Init();

            m_UpdateSystems = CreateUpdateSystems();
            m_UpdateSystems.Init();

            m_FixedUpdateSystems = CreateFixedUpdateSystems();
            m_FixedUpdateSystems.Init();

            m_LateUpdateSystems = CreateLateUpdateSystems();
            m_LateUpdateSystems.Init();
        }

        private void Update()
        {
            m_UpdateSystems?.Run();
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
        }

        private IEcsSystems CreateInitSystems()
        {
            return new EcsSystems(m_World)
                .Add(new JoystickInit(m_JoystickLinker))
                .Add(new PlayerInit(m_PlayerLinker))
                .Add(new FollowerInit(m_CameraLinker))
                .Add(new GeneratorInit(m_Generators, m_TimerService))
#if UNITY_EDITOR
                .Add(new EcsSystemsDebugSystem())
                .Add(new EcsWorldDebugSystem())
#endif
                .Inject();
        }

        private IEcsSystems CreateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new TimeListenerRun())
                .Add(new IntervalTimerRun(Debug.unityLogger))
                .Add(new GenerateSelfPingRequestHandlerRun(Debug.unityLogger))
                .Add(new GenerateSelfRequestHandlerRun(Debug.unityLogger,
                    new EntityOutPool(m_World, new EntityOutFactory(m_World, m_ObjectPrefab), 5)))
                .Add(new JoystickRun())
                .Inject(CreateUpdateInjectParams());
        }

        private IEcsSystems CreateFixedUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new MoveInDirectionRun())
                .Add(new RotateInDirectionRun())
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
                    m_TimeListener,
                }
                .ToArray();
        }

        private object[] CreateFixedUpdateInjectParams()
        {
            return new HashSet<object>
                {
                    m_TimeListener,
                }
                .ToArray();
        }
    }
}
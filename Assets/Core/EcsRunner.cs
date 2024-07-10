using System.Collections.Generic;
using System.Linq;
using Core.Base;
using Core.DevicesInput.JoystickPack.Systems;
using Core.EcsMapper;
using Core.Follower.Systems;
using Core.Logger;
using Core.MonoConverter;
using Core.MonoConverter.Factory;
using Core.MonoConverter.Links;
using Core.MonoConverter.Pool;
using Core.Movement.Move.Systems;
using Core.Movement.Rotate.Systems;
using Core.Player.Systems;
using Core.Stack.Base.Services;
using Core.Stack.Refillable.Services;
using Core.Stack.Refillable.Systems;
using Core.Stack.Void.Services;
using Core.Stack.Void.Systems;
using Core.TextUpdater.Systems;
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
        [SerializeField] private MonoLinker m_VoidLinker;
        [SerializeField] private MonoLinker m_JoystickLinker;
        [SerializeField] private MonoLinker m_PlayerLinker;
        [SerializeField] private MonoLinker m_CameraLinker;
        [SerializeField] private MonoLinker[] m_Generators;
        [SerializeField] private MonoLinker m_StackObjectPrefab;
        [SerializeField] private List<StackObject> m_StackObjects;

        private EcsWorld m_World;

        private IEcsSystems m_UpdateSystems;
        private IEcsSystems m_LateUpdateSystems;
        private IEcsSystems m_InitSystems;
        private IEcsSystems m_FixedUpdateSystems;
#if UNITY_EDITOR
        private IEcsSystems m_LeoEcsDebugSystems;
#endif

        private IFactory<EcsPackedEntityWithWorld> m_StackObjectFactory;
        private IPool<EcsPackedEntityWithWorld> m_StackObjectPool;
        private LeoEcsLiteEntityMap m_EntityMap;
        private TimeService m_TimeService;
        private TimerService m_TimerService;
        private StackObjectsMeshMap m_MeshMap;
        private RefillableService m_RefillableService;
        private VoidService m_VoidService;

        private void Awake()
        {
            m_World = new EcsWorld();

            m_TimeService = new TimeService();
            m_TimerService = new TimerService(m_World);
            m_StackObjectFactory = new EntityFactory(m_World, m_StackObjectPrefab, OnCreateStackObject);
            m_StackObjectPool = new EntityPool(Debug.unityLogger.WithPrefix($"StackObjectPool"), m_StackObjectFactory);
            m_EntityMap = new LeoEcsLiteEntityMap(m_World, Debug.unityLogger);
            m_MeshMap = new StackObjectsMeshMap(m_StackObjects);
            m_RefillableService = new RefillableService(m_World, m_TimerService, m_StackObjectPool, m_MeshMap);
            m_VoidService = new VoidService(m_World, m_TimerService, m_StackObjectPool);

            m_VoidLinker.LinkTo(m_World.PackEntityWithWorld(m_World.NewEntity()));

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
                .Add(new GeneratorInit(m_Generators, m_TimerService))
                .Inject(CreateInitInjectParams());
        }

        private IEcsSystems CreateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new TimeServiceRun())
                .Add(new JoystickRun())
                .Add(new KillRun())
                .Add(new OnPauseGenerateTimerRun())
                .Add(new PauseRun())
                .Add(new OnUnpauseGenerateTimerRun())
                .Add(new UnpauseRun())
                .Add(new IntervalTimerCompleteRun())
                .Add(new TickRun())
                .Add(new GenerateTimerTickRun())
                .Add(new GenerateTimerCompleteRun())
                .Add(new TranslateCompleteTimerRun())
                .Add(new CompleteTranslateTimerRun())
                .Add(new UpdateTMPRun())
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
                .Add(new TriggerExitRefillableRun())
                .Add(new TriggerStayRefillableRun())
                .Add(new TriggerExitVoidRun())
                .Add(new TriggerStayVoidRun())
                .Add(new FollowerLeaderRun())
                .Inject(CreateFixedUpdateInjectParams());
        }

        private IEcsSystems CreateLateUpdateSystems()
        {
            return new EcsSystems(m_World)
                .Add(new FollowerRun())
                .Inject();
        }

        private object[] CreateInitInjectParams()
        {
            return new HashSet<object>
                {
                    m_EntityMap,
                    m_MeshMap,
                }
                .ToArray();
        }

        private object[] CreateUpdateInjectParams()
        {
            return new HashSet<object>
                {
                    m_TimeService,
                    m_MeshMap,
                    m_RefillableService,
                    m_VoidService,
                }
                .ToArray();
        }

        private object[] CreateFixedUpdateInjectParams()
        {
            return new HashSet<object>
                {
                    m_TimeService,
                    m_EntityMap,
                    m_TimerService,
                    m_RefillableService,
                    m_VoidService,
                }
                .ToArray();
        }

        private void OnCreateStackObject(int entity)
        {
            var gameObjectPool = m_World.GetPool<GameObjectLink>();
            ref var go = ref gameObjectPool.Get(entity);
            go.Value.SetActive(false);
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
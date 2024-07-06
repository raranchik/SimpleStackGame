using Core.Base.Runtime;
using Core.InteractiveStack.Generator.Components;
using Core.InteractiveStack.Generator.SelfRequests;
using Core.InteractiveStack.Stack.Components;
using Core.InteractiveStack.Stack.Requests;
using Core.Logger;
using Core.MonoConverter.Links;
using Core.TimeManagement.Timer.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Generator.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateSelfRequestHandlerRun : IEcsRunSystem
    {
        private const int MinGenerateCount = 1;
        private const int MaxGenerateCount = 1;

        private readonly ILogger m_Logger;
        private IOutPool<EcsPackedEntityWithWorld> m_ObjectPool;
        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsPoolInject<StackPushRequest> m_PushRequestPool;
        private readonly EcsPoolInject<TimerIsDisabledComponent> m_IsDisabledTimerPool;
        private readonly EcsPoolInject<GenerateSelfPingRequestTimerComponent> m_TimerComponentPool;
        private readonly EcsPoolInject<MeshLink> m_MeshPool;
        private readonly EcsPoolInject<MeshFilterLink> m_MeshFilterPool;

        private readonly EcsFilterInject<
            Inc<StackComponent, StackMaxCountLink, GenerateSelfRequest>
        > m_GenerateRequestFilter;

        public GenerateSelfRequestHandlerRun(ILogger logger, IOutPool<EcsPackedEntityWithWorld> objectPool)
        {
            m_Logger = logger.WithPrefix(nameof(GenerateSelfRequestHandlerRun));
            m_ObjectPool = objectPool;
        }

        public void Run(IEcsSystems systems)
        {
            if (m_GenerateRequestFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var generateRequestEntity in m_GenerateRequestFilter.Value)
            {
                ref var stack = ref m_GenerateRequestFilter.Pools.Inc1.Get(generateRequestEntity);
                ref var stackMax = ref m_GenerateRequestFilter.Pools.Inc2.Get(generateRequestEntity);
                if (stack.Value.Count >= stackMax.Value)
                {
                    if (m_TimerComponentPool.Value.Has(generateRequestEntity))
                    {
                        ref var timer = ref m_TimerComponentPool.Value.Get(generateRequestEntity);
                        timer.Value.Unpack(m_WorldInject.Value, out var timerEntity);
                        m_IsDisabledTimerPool.Value.Add(timerEntity);
                        m_GenerateRequestFilter.Pools.Inc3.Del(generateRequestEntity);
                        m_Logger.Log(LogType.Error, $"Stack already full. I disable timer for you");
                        continue;
                    }

                    m_GenerateRequestFilter.Pools.Inc3.Del(generateRequestEntity);
                    m_Logger.Log(LogType.Error, $"Stack already full");
                    continue;
                }

                var remainingCount = stackMax.Value - stack.Value.Count;
                var generateCount = Mathf.Clamp(remainingCount, MinGenerateCount, MaxGenerateCount);
                for (var i = 0; i < generateCount; i++)
                {
                    m_ObjectPool.Pop(out var objectPackedEntity);
                    if (!objectPackedEntity.Unpack(out var world, out var objectEntity))
                    {
                        m_Logger.Log(LogType.Error, $"Dead entity in pool");
                        break;
                    }

                    ref var mesh = ref m_MeshPool.Value.Get(generateRequestEntity);
                    ref var meshFilter = ref m_MeshFilterPool.Value.Get(objectEntity);
                    meshFilter.Value.mesh = mesh.Value;

                    var pushRequestEntity = m_WorldInject.Value.NewEntity();
                    ref var pushRequest = ref m_PushRequestPool.Value.Add(pushRequestEntity);
                    pushRequest.Destination = m_WorldInject.Value.PackEntity(generateRequestEntity);
                    pushRequest.Object = m_WorldInject.Value.PackEntity(objectEntity);
                }

                m_GenerateRequestFilter.Pools.Inc3.Del(generateRequestEntity);
            }
        }
    }
}
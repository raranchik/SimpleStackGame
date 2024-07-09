using Core.Base;
using Core.Container.Components;
using Core.Container.Links;
using Core.Counter.Runtime.Requests;
using Core.Generator.Systems;
using Core.Logger;
using Core.MonoConverter.Links;
using Core.RefillableStack.Components;
using Core.RefillableStack.Links;
using Core.RefillableStack.Tags;
using Core.Stack.Components;
using Core.Stack.Links;
using Core.Timer.Components;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.RefillableStack.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class StackObjectGeneratorRun : GeneratorBase<ObjectParentStackComponent>
    {
        private const int MinGenerateCount = 1;

        private readonly ILogger m_Logger;
        private readonly IPool<EcsPackedEntityWithWorld> m_ObjectPool;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<StackComponent> m_StackPool;
        private readonly EcsPoolInject<StackMaxCountLink> m_StackMaxPool;
        private readonly EcsPoolInject<IsDisabledTag> m_IsDisabledTimerPool;
        private readonly EcsPoolInject<CountPerGenerationLink> m_CountPerGenerationPool;
        private readonly EcsPoolInject<ObjectParentStackComponent> m_ObjectComponentPool;
        private readonly EcsPoolInject<MeshLink> m_MeshPool;
        private readonly EcsPoolInject<MeshFilterLink> m_MeshFilterPool;
        private readonly EcsPoolInject<TransformLink> m_TransformPool;
        private readonly EcsPoolInject<GridEmptyPositionsComponent> m_EmptyPositionsPool;
        private readonly EcsPoolInject<GridCellScaleLink> m_CellScalePool;
        private readonly EcsPoolInject<GameObjectLink> m_GameObjectPool;
        private readonly EcsPoolInject<ContainerRootLink> m_ContainerRootPool;
        private readonly EcsPoolInject<CounterTextLink> m_CounterLink;
        private readonly EcsPoolInject<DisableSelfRequest> m_DisableSelfRequestPool;
        private readonly EcsPoolInject<GridObjectPositionComponent> m_GridObjectPositionPool;
        private readonly EcsPoolInject<UpdateTMPRequest> m_UpdateTextPool;

        private readonly EcsFilterInject<
            Inc<RefillableStackIntervalTimerTag, IsCompleteSelfRequest, IsIntervalTag,
                SourceComponent>,
            Exc<IsDisabledTag>
        > m_TimerFilter;

        public StackObjectGeneratorRun(ILogger logger, IPool<EcsPackedEntityWithWorld> objectPool)
        {
            m_Logger = logger.WithPrefix(nameof(StackObjectGeneratorRun));
            m_ObjectPool = objectPool;
        }

        public override void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() < 0)
            {
                return;
            }

            foreach (var timerEntity in m_TimerFilter.Value)
            {
                ref var source = ref m_TimerFilter.Pools.Inc4.Get(timerEntity);
                if (!source.Value.Unpack(m_World.Value, out var sourceEntity))
                {
                    m_World.Value.DelEntity(timerEntity);
                    m_Logger.Log(LogType.Error, $"Source entity is dead. I kill timer for you");
                    continue;
                }

                ref var stack = ref m_StackPool.Value.Get(sourceEntity);
                ref var stackMax = ref m_StackMaxPool.Value.Get(sourceEntity);
                if (stack.Value.Count >= stackMax.Value)
                {
                    m_TimerFilter.Pools.Inc2.Del(timerEntity);
                    m_IsDisabledTimerPool.Value.Add(timerEntity);
                    m_DisableSelfRequestPool.Value.Add(timerEntity);
                    m_Logger.Log(LogType.Error, $"Stack already full. I disable timer for you");
                    continue;
                }

                ref var countPerGeneration = ref m_CountPerGenerationPool.Value.Get(sourceEntity);
                var remainingCount = stackMax.Value - stack.Value.Count;
                var generateCount = Mathf.Clamp(remainingCount, MinGenerateCount, countPerGeneration.Value);
                for (var i = 0; i < generateCount; i++)
                {
                    stack.Value.Push(CreateStackObject(sourceEntity));
                }

                ref var counter = ref m_CounterLink.Value.Get(sourceEntity);
                var counterText = $"{stack.Value.Count.ToString()}/{stackMax.Value.ToString()}";
                var updateTextEntity = m_World.Value.NewEntity();
                ref var updateText = ref m_UpdateTextPool.Value.Add(updateTextEntity);
                updateText.TMP = counter.Value;
                updateText.Value = counterText;

                if (stack.Value.Count >= stackMax.Value)
                {
                    m_DisableSelfRequestPool.Value.Add(timerEntity);
                }

                m_TimerFilter.Pools.Inc2.Del(timerEntity);
            }
        }

        private EcsPackedEntity CreateStackObject(in int source)
        {
            var packed = m_ObjectPool.Pop();
            packed.Unpack(out var world, out var entity);

            ref var stackObject = ref CreateObjectComponent(entity);
            stackObject.Value = m_World.Value.PackEntity(source);

            ref var mesh = ref m_MeshPool.Value.Get(source);
            ref var meshFilter = ref m_MeshFilterPool.Value.Get(entity);
            meshFilter.Value.sharedMesh = mesh.Value;

            ref var transform = ref m_TransformPool.Value.Get(entity);
            var boundsSize = mesh.Value.bounds.size;
            ref var cellScale = ref m_CellScalePool.Value.Get(source);
            var maxSize = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z);
            var scale = cellScale.Value / maxSize;
            transform.Value.localScale = Vector3.one * scale;

            ref var emptyPositions = ref m_EmptyPositionsPool.Value.Get(source);
            var gridPosition = emptyPositions.Value.Dequeue();
            ref var gridObjectPosition = ref m_GridObjectPositionPool.Value.Add(entity);
            gridObjectPosition.Value = gridPosition;

            ref var containerRoot = ref m_ContainerRootPool.Value.Get(source);
            var position = containerRoot.Value.position + (Vector3)gridPosition * cellScale.Value;
            transform.Value.position = position;

            ref var gameObject = ref m_GameObjectPool.Value.Get(entity);
            gameObject.Value.SetActive(true);

            return world.PackEntity(entity);
        }

        public override ref ObjectParentStackComponent CreateObjectComponent(in int entity)
        {
            return ref m_ObjectComponentPool.Value.Add(entity);
        }
    }
}
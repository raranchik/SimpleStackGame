using Core.Container.Components;
using Core.Container.Links;
using Core.Counter.Runtime.Requests;
using Core.Logger;
using Core.MonoConverter.Links;
using Core.Player.Components;
using Core.Player.Tags;
using Core.RefillableStack.Components;
using Core.RefillableStack.Links;
using Core.Stack.Components;
using Core.Stack.Links;
using Core.Timer.SelfRequests;
using Core.Timer.Tags;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Player.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TranslateFromToPlayerCompleteTimerRequestRun : IEcsRunSystem
    {
        private readonly ILogger m_Logger;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<GridObjectPositionComponent> m_GridObjectPositionPool;
        private readonly EcsPoolInject<Grid3SizeLink> m_Size3Pool;
        private readonly EcsPoolInject<GridCellScaleLink> m_CellScalePool;
        private readonly EcsPoolInject<StackComponent> m_StackPool;
        private readonly EcsPoolInject<TransformLink> m_TransformPool;
        private readonly EcsPoolInject<StackMaxCountLink> m_StackMaxPool;
        private readonly EcsPoolInject<ContainerRootLink> m_ContainerRootPool;
        private readonly EcsPoolInject<GridEmptyPositionsComponent> m_EmptyPositionsPool;
        private readonly EcsPoolInject<IsDisabledTag> m_IsDisabledPool;
        private readonly EcsPoolInject<MeshFilterLink> m_MeshFilterPool;
        private readonly EcsPoolInject<ObjectParentStackComponent> m_ObjectParentPool;
        private readonly EcsPoolInject<UpdateTMPRequest> m_UpdateTMPPool;
        private readonly EcsPoolInject<CounterTextLink> m_CounterPool;
        private readonly EcsPoolInject<RefillableStackIntervalTimerComponent> m_GenerateTimerPool;
        private readonly EcsPoolInject<ActivateSelfRequest> m_ActivatePool;

        private readonly EcsFilterInject<
            Inc<TranslateFromStack2ToStack3PlayerTimerTag, TranslateFromToComponent, IsIntervalTag,
                IsCompleteSelfRequest>,
            Exc<IsDisabledTag, DisableSelfRequest, DestroySelfRequest>
        > m_TimerFilter;

        public TranslateFromToPlayerCompleteTimerRequestRun(ILogger logger)
        {
            m_Logger = logger.WithPrefix(nameof(TranslateFromToPlayerCompleteTimerRequestRun));
        }

        public void Run(IEcsSystems systems)
        {
            if (m_TimerFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var timerEntity in m_TimerFilter.Value)
            {
                ref var fromTo = ref m_TimerFilter.Pools.Inc2.Get(timerEntity);
                fromTo.From.Unpack(m_World.Value, out var refillableStackEntity);
                fromTo.To.Unpack(m_World.Value, out var playerEntity);

                ref var fromStack = ref m_StackPool.Value.Get(refillableStackEntity);
                var objectPacked = fromStack.Value.Pop();
                if (!objectPacked.Unpack(m_World.Value, out var objectEntity))
                {
                    m_Logger.Log(LogType.Error, $"Stack object is dead. I kill timer for you");
                    m_World.Value.DelEntity(timerEntity);
                    m_GenerateTimerPool.Value.Del(playerEntity);
                    continue;
                }

                TranslateStackObject(objectEntity, playerEntity, refillableStackEntity);

                ref var parent = ref m_ObjectParentPool.Value.Get(objectEntity);
                parent.Value = fromTo.To;

                ref var toStack = ref m_StackPool.Value.Get(playerEntity);
                toStack.Value.Push(objectPacked);
                ref var toStackMax = ref m_StackMaxPool.Value.Get(playerEntity);
                if (toStack.Value.Count >= toStackMax.Value)
                {
                    m_World.Value.DelEntity(timerEntity);
                    m_GenerateTimerPool.Value.Del(playerEntity);
                }
                else
                {
                    m_TimerFilter.Pools.Inc4.Del(timerEntity);
                }

                ref var fromStackMax = ref m_StackMaxPool.Value.Get(refillableStackEntity);
                ref var counter = ref m_CounterPool.Value.Get(refillableStackEntity);
                var counterText = $"{fromStack.Value.Count.ToString()}/{fromStackMax.Value.ToString()}";
                var updateTextEntity = m_World.Value.NewEntity();
                ref var updateText = ref m_UpdateTMPPool.Value.Add(updateTextEntity);
                updateText.TMP = counter.Value;
                updateText.Value = counterText;

                ref var generateTimer = ref m_GenerateTimerPool.Value.Get(refillableStackEntity);
                if (generateTimer.Value.Unpack(m_World.Value, out var generateTimerEntity))
                {
                    if (fromStack.Value.Count < fromStackMax.Value && m_IsDisabledPool.Value.Has(generateTimerEntity))
                    {
                        m_ActivatePool.Value.Add(generateTimerEntity);
                    }
                }
            }
        }

        private void TranslateStackObject(in int objectEntity, in int playerEntity, in int refillableStackEntity)
        {
            ref var transform = ref m_TransformPool.Value.Get(objectEntity);

            ref var meshFilter = ref m_MeshFilterPool.Value.Get(objectEntity);
            var mesh = meshFilter.Value.sharedMesh;

            var boundsSize = mesh.bounds.size;
            ref var cellScale = ref m_CellScalePool.Value.Get(playerEntity);
            var maxSize = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z);
            var scale = cellScale.Value / maxSize;
            transform.Value.localScale = Vector3.one * scale;

            ref var fromEmptyPositions = ref m_EmptyPositionsPool.Value.Get(refillableStackEntity);
            ref var toEmptyPositions = ref m_EmptyPositionsPool.Value.Get(playerEntity);
            var gridPosition = toEmptyPositions.Value.Dequeue();
            ref var gridObjectPosition = ref m_GridObjectPositionPool.Value.Get(objectEntity);
            fromEmptyPositions.Value.Enqueue(gridObjectPosition.Value);
            gridObjectPosition.Value = gridPosition;

            ref var containerRoot = ref m_ContainerRootPool.Value.Get(playerEntity);
            transform.Value.SetParent(containerRoot.Value);

            var position = containerRoot.Value.position + (Vector3)gridPosition * cellScale.Value;
            transform.Value.position = position;
        }
    }
}
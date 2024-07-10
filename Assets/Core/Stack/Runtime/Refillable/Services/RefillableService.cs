using Core.Base;
using Core.Container.Components;
using Core.Container.Links;
using Core.MonoConverter.Links;
using Core.Stack.Base.Components;
using Core.Stack.Base.Links;
using Core.Stack.Base.Services;
using Core.Stack.Refillable.Components;
using Core.Stack.Refillable.Links;
using Core.TextUpdater.Requests;
using Core.Timer.Services;
using Leopotam.EcsLite;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Refillable.Services
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class RefillableService
    {
        private const int MinGenerateCount = 1;

        private readonly EcsWorld m_World;
        private readonly TimerService m_TimerService;
        private readonly IPool<EcsPackedEntityWithWorld> m_ObjectPool;
        private readonly StackObjectsMeshMap m_MeshMap;

        private readonly EcsPool<StackComponent> m_Stack;
        private readonly EcsPool<StackMaxCountLink> m_StackMax;
        private readonly EcsPool<TranslateTimerReferenceComponent> m_TranslateTimerReference;
        private readonly EcsPool<GenerateTimerReferenceComponent> m_GenerateTimerReference;
        private readonly EcsPool<TranslateTimerDurationLink> m_TranslateDuration;
        private readonly EcsPool<TranslateFromToComponent> m_TranslateFromTo;
        private readonly EcsPool<GenerateCountLink> m_GenerateCount;
        private readonly EcsPool<ObjectParentReferenceComponent> m_ObjectParent;
        private readonly EcsPool<StackObjectIdLink> m_Id;
        private readonly EcsPool<MeshFilterLink> m_MeshFilter;
        private readonly EcsPool<TransformLink> m_Transform;
        private readonly EcsPool<GridCellScaleLink> m_CellScale;
        private readonly EcsPool<GridEmptyPositionsComponent> m_EmptyPositions;
        private readonly EcsPool<GridObjectPositionComponent> m_GridPosition;
        private readonly EcsPool<ContainerRootLink> m_ContainerRoot;
        private readonly EcsPool<GameObjectLink> m_GameObject;
        private readonly EcsPool<CounterTextLink> m_GeneratorCounter;
        private readonly EcsPool<UpdateTMPRequest> m_UpdateText;

        public RefillableService(EcsWorld world, TimerService timerService, IPool<EcsPackedEntityWithWorld> objectPool,
            StackObjectsMeshMap meshMap)
        {
            m_World = world;
            m_TimerService = timerService;
            m_ObjectPool = objectPool;
            m_MeshMap = meshMap;

            m_Stack = m_World.GetPool<StackComponent>();
            m_StackMax = m_World.GetPool<StackMaxCountLink>();
            m_TranslateTimerReference = m_World.GetPool<TranslateTimerReferenceComponent>();
            m_GenerateTimerReference = m_World.GetPool<GenerateTimerReferenceComponent>();
            m_TranslateDuration = m_World.GetPool<TranslateTimerDurationLink>();
            m_TranslateFromTo = m_World.GetPool<TranslateFromToComponent>();
            m_GenerateCount = m_World.GetPool<GenerateCountLink>();
            m_ObjectParent = m_World.GetPool<ObjectParentReferenceComponent>();
            m_Id = m_World.GetPool<StackObjectIdLink>();
            m_MeshFilter = m_World.GetPool<MeshFilterLink>();
            m_Transform = m_World.GetPool<TransformLink>();
            m_CellScale = m_World.GetPool<GridCellScaleLink>();
            m_EmptyPositions = m_World.GetPool<GridEmptyPositionsComponent>();
            m_GridPosition = m_World.GetPool<GridObjectPositionComponent>();
            m_ContainerRoot = m_World.GetPool<ContainerRootLink>();
            m_GameObject = m_World.GetPool<GameObjectLink>();
            m_GeneratorCounter = m_World.GetPool<CounterTextLink>();
            m_UpdateText = m_World.GetPool<UpdateTMPRequest>();
        }

        public bool IsFullStack(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            ref var stackMax = ref m_StackMax.Get(entity);
            return stack.Value.Count >= stackMax.Value;
        }

        public bool IsEmptyStack(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            return stack.Value.Count <= 0;
        }

        public bool HasSpotInStack(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            ref var stackMax = ref m_StackMax.Get(entity);
            return stack.Value.Count < stackMax.Value;
        }

        public int GetTranslateTimer(in int entity)
        {
            ref var timerReference = ref m_TranslateTimerReference.Get(entity);
            timerReference.Value.Unpack(m_World, out var timerEntity);
            return timerEntity;
        }

        public int GetGenerateTimer(in int entity)
        {
            ref var timerReference = ref m_GenerateTimerReference.Get(entity);
            timerReference.Value.Unpack(m_World, out var timerEntity);
            return timerEntity;
        }

        public void RemoveTranslateTimerReference(in int entity)
        {
            m_TranslateTimerReference.Del(entity);
        }

        public bool HasTranslateTimerReference(in int entity)
        {
            return m_TranslateTimerReference.Has(entity);
        }

        public float GetTranslateTimerDuration(in int entity)
        {
            return m_TranslateDuration.Get(entity).Value;
        }

        public int CreateTimerEntityWithParent<TTag>(in float duration, in EcsPackedEntity parent)
            where TTag : struct
        {
            return m_TimerService.DeclareTimer()
                .AsInterval()
                .WithDuration(duration)
                .WithParent(parent)
                .WithTag<TTag>()
                .SubmitUnpacked();
        }

        public void KillTimerEntity(in int entity)
        {
            m_TimerService.Kill(entity);
        }

        public void PauseTimerEntity(in int entity)
        {
            m_TimerService.Pause(entity);
        }

        public void UnpauseTimerEntity(in int entity)
        {
            m_TimerService.Unpause(entity);
        }

        public void AddTranslateFromToComponent(in int entity, in EcsPackedEntity from, in EcsPackedEntity to)
        {
            ref var component = ref m_TranslateFromTo.Add(entity);
            component.From = from;
            component.To = to;
        }

        public void AddTranslateTimerReferenceComponent(in int entity, EcsPackedEntity timerPacked)
        {
            ref var component = ref m_TranslateTimerReference.Add(entity);
            component.Value = timerPacked;
        }

        public void RemoveTranslateTimerReferenceComponent(in int entity)
        {
            m_TranslateTimerReference.Del(entity);
        }

        public int GetGenerateCount(in int entity)
        {
            ref var generateCount = ref m_GenerateCount.Get(entity);
            ref var stack = ref m_Stack.Get(entity);
            ref var stackMax = ref m_StackMax.Get(entity);
            var remaining = stackMax.Value - stack.Value.Count;
            return Mathf.Clamp(remaining, MinGenerateCount, generateCount.Value);
        }

        public EcsPackedEntity CreateStackObject(in int parentEntity)
        {
            var objectPacked = m_ObjectPool.Pop();
            objectPacked.Unpack(out var world, out var objectEntity);

            ref var id = ref m_Id.Get(parentEntity);
            var mesh = m_MeshMap.GetMesh(id.Value);
            ref var meshFilter = ref m_MeshFilter.Get(objectEntity);
            meshFilter.Value.sharedMesh = mesh;

            ref var transform = ref m_Transform.Get(objectEntity);
            ref var cellScale = ref m_CellScale.Get(parentEntity);
            var boundsSize = mesh.bounds.size;
            var maxSize = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z);
            var scale = cellScale.Value / maxSize;
            transform.Value.localScale = Vector3.one * scale;

            ref var emptyPositions = ref m_EmptyPositions.Get(parentEntity);
            var gridPosition = emptyPositions.Value.Pop();
            ref var gridObjectPosition = ref m_GridPosition.Add(objectEntity);
            gridObjectPosition.Value = gridPosition;

            ref var containerRoot = ref m_ContainerRoot.Get(parentEntity);
            var position = containerRoot.Value.position + (Vector3)gridPosition * cellScale.Value;
            transform.Value.position = position;

            ref var gameObject = ref m_GameObject.Get(objectEntity);
            gameObject.Value.SetActive(true);

            AddObjectParent(objectEntity, m_World.PackEntity(parentEntity));

            return world.PackEntity(objectEntity);
        }

        public void TranslateStackToPlayer(in int objectEntity, in int fromEntity, in int toEntity)
        {
            ref var transform = ref m_Transform.Get(objectEntity);

            ref var meshFilter = ref m_MeshFilter.Get(objectEntity);
            var mesh = meshFilter.Value.sharedMesh;

            ref var cellScale = ref m_CellScale.Get(toEntity);
            var bounds = mesh.bounds;
            var boundsSize = bounds.size;
            var maxSize = Mathf.Max(boundsSize.x, boundsSize.y, boundsSize.z);
            var scale = cellScale.Value / maxSize;
            transform.Value.localScale = Vector3.one * scale;

            ref var fromEmptyPositions = ref m_EmptyPositions.Get(fromEntity);
            ref var toEmptyPositions = ref m_EmptyPositions.Get(toEntity);
            var gridPosition = toEmptyPositions.Value.Pop();
            ref var gridObjectPosition = ref m_GridPosition.Get(objectEntity);
            fromEmptyPositions.Value.Push(gridObjectPosition.Value);
            gridObjectPosition.Value = gridPosition;

            ref var containerRoot = ref m_ContainerRoot.Get(toEntity);
            transform.Value.SetParent(containerRoot.Value);

            ChangeObjectParent(objectEntity, m_World.PackEntity(toEntity));

            var position = containerRoot.Value.position;
            ref var stack = ref m_Stack.Get(toEntity);
            if (stack.Value.Count <= 0)
            {
                transform.Value.position = position;
                return;
            }

            var halfHeight = boundsSize.y * scale * 0.5f;

            var lastPacked = stack.Value.Peek();
            lastPacked.Unpack(m_World, out var last);
            meshFilter = ref m_MeshFilter.Get(last);
            mesh = meshFilter.Value.sharedMesh;

            var lastBounds = mesh.bounds;
            ref var lastTransform = ref m_Transform.Get(last);
            var lastScale = lastTransform.Value.lossyScale.x;
            var lastHalfHeight = lastBounds.size.y * lastScale * 0.5f;

            position = lastTransform.Value.position;
            position.y += lastHalfHeight + halfHeight;
            transform.Value.position = position;
        }

        public EcsPackedEntity PopObject(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            return stack.Value.Pop();
        }

        public void PushObject(in int entity, in EcsPackedEntity objectPacked)
        {
            ref var stack = ref m_Stack.Get(entity);
            stack.Value.Push(objectPacked);
        }

        public void AddObjectParent(in int entity, in EcsPackedEntity parent)
        {
            ref var objectParent = ref m_ObjectParent.Add(entity);
            objectParent.Value = parent;
        }

        public void ChangeObjectParent(in int entity, in EcsPackedEntity parent)
        {
            ref var objectParent = ref m_ObjectParent.Get(entity);
            objectParent.Value = parent;
        }

        public void UpdateGeneratorCounter(in int entity)
        {
            ref var counter = ref m_GeneratorCounter.Get(entity);
            ref var stack = ref m_Stack.Get(entity);
            ref var stackMax = ref m_StackMax.Get(entity);
            var text = $"{stack.Value.Count.ToString()}/{stackMax.Value.ToString()}";
            ref var updateText = ref m_UpdateText.Add(m_World.NewEntity());
            updateText.TMP = counter.Value;
            updateText.Value = text;
        }
    }
}
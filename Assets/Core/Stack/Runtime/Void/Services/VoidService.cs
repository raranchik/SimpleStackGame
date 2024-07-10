using Core.Base;
using Core.Container.Components;
using Core.MonoConverter.Links;
using Core.Stack.Base.Components;
using Core.Stack.Void.Components;
using Core.Stack.Void.Links;
using Core.Stack.Void.Tags;
using Core.Timer.Services;
using Leopotam.EcsLite;

namespace Core.Stack.Void.Services
{
    public class VoidService
    {
        private readonly TimerService m_TimerService;
        private readonly EcsWorld m_World;
        private readonly IPool<EcsPackedEntityWithWorld> m_ObjectPool;
        private readonly EcsPool<StackComponent> m_Stack;
        private readonly EcsPool<TranslateTimerReferenceComponent> m_TranslateTimerReference;
        private readonly EcsPool<TranslateDurationLink> m_Duration;
        private readonly EcsPool<TranslateFromToComponent> m_TranslateFromTo;
        private readonly EcsPool<GameObjectLink> m_GameObject;
        private readonly EcsPool<TransformLink> m_Transform;
        private readonly EcsPool<ObjectParentReferenceComponent> m_ObjectParent;
        private readonly EcsPool<GridEmptyPositionsComponent> m_EmptyPositions;
        private readonly EcsPool<GridObjectPositionComponent> m_GridPosition;

        public VoidService(EcsWorld world, TimerService timerService, IPool<EcsPackedEntityWithWorld> objectPool)
        {
            m_ObjectPool = objectPool;
            m_World = world;
            m_TimerService = timerService;
            m_Stack = m_World.GetPool<StackComponent>();
            m_TranslateTimerReference = m_World.GetPool<TranslateTimerReferenceComponent>();
            m_Duration = m_World.GetPool<TranslateDurationLink>();
            m_TranslateFromTo = m_World.GetPool<TranslateFromToComponent>();
            m_GameObject = m_World.GetPool<GameObjectLink>();
            m_Transform = m_World.GetPool<TransformLink>();
            m_ObjectParent = m_World.GetPool<ObjectParentReferenceComponent>();
            m_EmptyPositions = m_World.GetPool<GridEmptyPositionsComponent>();
            m_GridPosition = m_World.GetPool<GridObjectPositionComponent>();
        }

        public EcsPackedEntity PopObject(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            return stack.Value.Pop();
        }

        public void DestroyObject(in EcsPackedEntity packed)
        {
            packed.Unpack(m_World, out var entity);

            ref var gameObject = ref m_GameObject.Get(entity);
            gameObject.Value.SetActive(false);

            ref var transform = ref m_Transform.Get(entity);
            transform.Value.SetParent(null);

            ref var parent = ref m_ObjectParent.Get(entity);
            parent.Value.Unpack(m_World, out var parentEntity);
            ref var emptyPositions = ref m_EmptyPositions.Get(parentEntity);

            ref var gridPosition = ref m_GridPosition.Get(entity);
            emptyPositions.Value.Push(gridPosition.Value);

            m_GridPosition.Del(entity);
            m_ObjectParent.Del(entity);
            m_ObjectPool.Push(m_World.PackEntityWithWorld(entity));
        }

        public bool IsEmptyStack(in int entity)
        {
            ref var stack = ref m_Stack.Get(entity);
            return stack.Value.Count <= 0;
        }

        public int GetTimerEntity(in int entity)
        {
            ref var timerReference = ref m_TranslateTimerReference.Get(entity);
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
            return m_Duration.Get(entity).Value;
        }

        public int CreateTimerEntityWithParent(in float duration, in EcsPackedEntity parent)
        {
            return m_TimerService.DeclareTimer()
                .AsInterval()
                .WithDuration(duration)
                .WithParent(parent)
                .WithTag<TranslateTimerTag>()
                .SubmitUnpacked();
        }

        public void KillTimerEntity(in int entity)
        {
            m_TimerService.Kill(entity);
        }

        public void AddTranslateFromToComponent(in int entity, in EcsPackedEntity from, in EcsPackedEntity to)
        {
            ref var component = ref m_TranslateFromTo.Add(entity);
            component.From = from;
            component.To = to;
        }

        public void AddTimerReferenceComponent(in int entity, EcsPackedEntity timerPacked)
        {
            ref var component = ref m_TranslateTimerReference.Add(entity);
            component.Value = timerPacked;
        }
    }
}
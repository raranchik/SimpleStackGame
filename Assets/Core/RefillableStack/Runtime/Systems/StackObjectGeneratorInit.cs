using System.Collections.Generic;
using Core.Container.Components;
using Core.Container.Links;
using Core.MonoConverter;
using Core.MonoConverter.Links;
using Core.RefillableStack.Components;
using Core.RefillableStack.Links;
using Core.RefillableStack.Tags;
using Core.Stack.Components;
using Core.Stack.Links;
using Core.Timer.Services;
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
    public class StackObjectGeneratorInit : IEcsInitSystem
    {
        private readonly MonoLinker[] m_Generators;
        private readonly TimerService m_TimerService;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<GenerationIntervalLink> m_IntervalPool;
        private readonly EcsPoolInject<GridEmptyPositionsComponent> m_EmptyPositionsPool;
        private readonly EcsPoolInject<Grid2SizeLink> m_Grid2SizePool;
        private readonly EcsPoolInject<StackComponent> m_StackPool;
        private readonly EcsPoolInject<MeshLink> m_MeshPool;
        private readonly EcsPoolInject<PreviewLink> m_PreviewPool;
        private readonly EcsPoolInject<CounterTextLink> m_CounterPool;
        private readonly EcsPoolInject<StackMaxCountLink> m_StackMaxPool;
        private readonly EcsPoolInject<RefillableStackIntervalTimerComponent> m_TimerPool;

        public StackObjectGeneratorInit(MonoLinker[] generators, TimerService timerService)
        {
            m_Generators = generators;
            m_TimerService = timerService;
        }

        public void Init(IEcsSystems systems)
        {
            foreach (var monoLinker in m_Generators)
            {
                var entity = m_World.Value.NewEntity();
                InitializeMonoLinker(monoLinker, m_World.Value.PackEntityWithWorld(entity));
                InitializePreview(entity);
                InitializeStack(entity);
                InitializeContainer(entity);
                InitializeCounter(entity);
                InitializeTimer(entity);
            }
        }

        private void InitializeMonoLinker(MonoLinker monoLinker, in EcsPackedEntityWithWorld packed)
        {
            monoLinker.LinkTo(packed);
        }

        private void InitializeTimer(in int entity)
        {
            ref var timer = ref m_TimerPool.Value.Add(entity);
            ref var interval = ref m_IntervalPool.Value.Get(entity);
            timer.Value = m_TimerService.DeclareTimer()
                .WithDuration(interval.Value)
                .WithSource(m_World.Value.PackEntity(entity))
                .WithTag<RefillableStackIntervalTimerTag>()
                .AsInterval()
                .Submit();
        }

        private void InitializeContainer(in int entity)
        {
            ref var emptyPositions = ref m_EmptyPositionsPool.Value.Add(entity);
            emptyPositions.Value = new Queue<Vector3Int>();
            ref var grid2Size = ref m_Grid2SizePool.Value.Get(entity);
            for (var x = 0; x < grid2Size.Value.x; x++)
            {
                for (var z = 0; z < grid2Size.Value.y; z++)
                {
                    emptyPositions.Value.Enqueue(new Vector3Int(x, 0, z));
                }
            }
        }

        private void InitializeStack(in int entity)
        {
            ref var stack = ref m_StackPool.Value.Add(entity);
            stack.Value = new Stack<EcsPackedEntity>();
        }

        private void InitializePreview(in int entity)
        {
            ref var preview = ref m_PreviewPool.Value.Get(entity);
            ref var mesh = ref m_MeshPool.Value.Get(entity);
            preview.Value.sharedMesh = mesh.Value;
        }

        private void InitializeCounter(in int entity)
        {
            ref var stackMax = ref m_StackMaxPool.Value.Get(entity);
            ref var counter = ref m_CounterPool.Value.Get(entity);
            var counterText = $"{0.ToString()}/{stackMax.Value.ToString()}";
            counter.Value.text = counterText;
        }
    }
}
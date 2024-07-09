using System.Collections.Generic;
using System.Linq;
using Core.Base;
using Leopotam.EcsLite;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter.Pool
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class EntityPool : IPool<EcsPackedEntityWithWorld>
    {
        private readonly ILogger m_Logger;
        private readonly IFactory<EcsPackedEntityWithWorld> m_Factory;
        private readonly int m_ExpandSize;
        private Stack<EcsPackedEntityWithWorld> m_Stack;

        public EntityPool(ILogger logger, IFactory<EcsPackedEntityWithWorld> factory, int initSize = 5,
            int expandSize = 5)
        {
            m_Logger = logger;
            m_Factory = factory;
            m_ExpandSize = expandSize;
            m_Stack = new Stack<EcsPackedEntityWithWorld>(initSize);
            Expand(initSize);
        }

        public EcsPackedEntityWithWorld Pop()
        {
            CleanUpDeadEntities();
            if (m_Stack.Count > 0)
            {
                return m_Stack.Pop();
            }

            Expand(m_ExpandSize);
            return m_Stack.Pop();
        }

        public void Push(in EcsPackedEntityWithWorld value)
        {
            m_Stack.Push(value);
        }

        private void Expand(int size)
        {
            for (var i = 0; i < size; i++)
            {
                m_Stack.Push(m_Factory.Create());
            }
        }

        private void CleanUpDeadEntities()
        {
            if (m_Stack.Count <= 0)
            {
                return;
            }

            var packedEntities = m_Stack.ToList();
            var deadEntitiesCount = packedEntities.RemoveAll(IsDeadEntity);
            m_Stack = new Stack<EcsPackedEntityWithWorld>(packedEntities);

            if (deadEntitiesCount > 0)
            {
                m_Logger.Log(LogType.Error, $"Dead entities count: count<{deadEntitiesCount.ToString()}>");
            }
        }

        private bool IsDeadEntity(EcsPackedEntityWithWorld packed)
        {
            return !packed.Unpack(out var world, out var entity);
        }
    }
}
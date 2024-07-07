using System.Collections.Generic;
using Core.Base.Runtime;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter.Pool
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class EntityOutPool : IOutPool<EcsPackedEntityWithWorld>
    {
        private readonly IOutFactory<EcsPackedEntityWithWorld> m_Factory;
        private readonly int m_ExpandSize;
        private readonly Stack<EcsPackedEntityWithWorld> m_Stack;

        public EntityOutPool(IOutFactory<EcsPackedEntityWithWorld> factory, int initSize,
            int expandSize = 5)
        {
            m_Factory = factory;
            m_ExpandSize = expandSize;
            m_Stack = new Stack<EcsPackedEntityWithWorld>(initSize);
            Expand(initSize);
        }

        public bool Pop(out EcsPackedEntityWithWorld result)
        {
            if (m_Stack.Count > 0)
            {
                result = m_Stack.Pop();
                return true;
            }

            Expand(m_ExpandSize);
            result = m_Stack.Pop();
            return true;
        }

        public void Push(in EcsPackedEntityWithWorld value)
        {
            m_Stack.Push(value);
        }

        private void Expand(int size)
        {
            for (var i = 0; i < size; i++)
            {
                m_Factory.Create(out var result);
                m_Stack.Push(result);
            }
        }
    }
}
using System;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Base.Services
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    [Serializable]
    public class StackObject
    {
        [SerializeField] private int m_Id;
        [SerializeField] private Mesh m_Mesh;

        public int Id => m_Id;
        public Mesh Mesh => m_Mesh;
    }
}
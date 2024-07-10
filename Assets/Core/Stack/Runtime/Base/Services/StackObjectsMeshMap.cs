using System.Collections.Generic;
using System.Linq;
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
    public class StackObjectsMeshMap
    {
        public Dictionary<int, Mesh> m_Map;

        public StackObjectsMeshMap(List<StackObject> objects)
        {
            m_Map = objects.ToDictionary(x => x.Id, x => x.Mesh);
        }

        public Mesh GetMesh(int id)
        {
            return m_Map[id];
        }
    }
}
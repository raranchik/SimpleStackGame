using System;
using TMPro;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Counter.Runtime.Requests
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    [Serializable]
    public struct UpdateTMPRequest
    {
        public TextMeshPro TMP;
        public string Value;
    }
}
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class MonoLinker : MonoLinkBase
    {
        private bool m_IsInitialized;
        private Dictionary<string, MonoLinkBase> m_Links;
        private EcsPackedEntityWithWorld m_PackedEntityWithWorld;

        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
            Init();
            foreach (var (_, link) in m_Links)
            {
                link.LinkTo(packedEntityWithWorld);
            }
        }

        private void Init()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_IsInitialized = true;
            m_Links = new Dictionary<string, MonoLinkBase>();
            var links = GetComponents<MonoLinkBase>().Where(x => x is not MonoLinker);
            foreach (var link in links)
            {
                var type = link.GetType();
                var baseType = type.BaseType;
                if (baseType == null || !baseType.IsGenericType)
                {
                    continue;
                }

                var genericArgs = baseType.GenericTypeArguments;
                if (genericArgs.Length == 0 || genericArgs.Length > 1)
                {
                    continue;
                }

                var genericType = genericArgs[0];
                m_Links.Add(genericType.Name, link);
            }
        }
    }
}
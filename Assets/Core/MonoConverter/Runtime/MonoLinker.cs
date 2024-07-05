using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;

namespace Core.MonoConverter
{
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

        public MonoLink<T> GetLink<T>() where T : struct
        {
            Init();
            var type = typeof(T);
            return m_Links[type.Name] as MonoLink<T>;
        }

        public bool IsLinkExist<T>() where T : struct
        {
            var type = typeof(T);
            return IsLinkExist(type);
        }

        public bool IsLinkExist(Type type)
        {
            Init();
            return m_Links.ContainsKey(type.Name);
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
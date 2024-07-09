using System.Collections.Generic;
using System.Linq;
using Core.Logger;
using Leopotam.EcsLite;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.EcsMapper
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class LeoEcsLiteEntityMapper
    {
        private readonly EcsWorld m_World;
        private readonly ILogger m_Logger;
        private Dictionary<GameObject, EcsPackedEntity> m_Map = new Dictionary<GameObject, EcsPackedEntity>();

        public LeoEcsLiteEntityMapper(EcsWorld world, ILogger logger)
        {
            m_World = world;
            m_Logger = logger.WithPrefix(nameof(LeoEcsLiteEntityMapper));
        }

        public bool Register(GameObject gameObject, in EcsPackedEntity packedEntity)
        {
            if (gameObject == null)
            {
                m_Logger.Log(LogType.Error,
                    $"Game object is null");
                return false;
            }

            if (!packedEntity.Unpack(m_World, out var entity))
            {
                m_Logger.Log(LogType.Error,
                    $"Dead entity linked to game object: GameObject<{gameObject.name}>");
                return false;
            }

            if (m_Map.ContainsKey(gameObject))
            {
                m_Logger.Log(LogType.Error,
                    $"Unregistered game object: GameObject<{gameObject.name}>");
                return false;
            }

            m_Map.Add(gameObject, packedEntity);
            return true;
        }

        public bool Unregister(GameObject gameObject, out EcsPackedEntity packedEntity)
        {
            if (gameObject == null)
            {
                m_Logger.Log(LogType.Error,
                    $"Game object is null");
                packedEntity = default;
                return false;
            }

            return m_Map.Remove(gameObject, out packedEntity);
        }

        public bool GetEntity(GameObject gameObject, out EcsPackedEntity packedEntity)
        {
            CleanUpDeadEntities();

            if (gameObject == null)
            {
                m_Logger.Log(LogType.Error,
                    $"Game object is null");
                packedEntity = default;
                return false;
            }

            if (!m_Map.TryGetValue(gameObject, out packedEntity))
            {
                m_Logger.Log(LogType.Error,
                    $"Map does not contain game object: GameObject<{gameObject.name}>");
                return false;
            }

            return true;
        }

        private void CleanUpDeadEntities()
        {
            if (m_Map.Count <= 0)
            {
                return;
            }

            var packedEntities = m_Map.ToList();
            var deadEntitiesCount = packedEntities.RemoveAll(IsDeadEntity);
            m_Map = packedEntities.ToDictionary(pair => pair.Key, pair => pair.Value);

            if (deadEntitiesCount > 0)
            {
                m_Logger.Log(LogType.Error, $"Dead entities count: count<{deadEntitiesCount.ToString()}>");
            }
        }

        private bool IsDeadEntity(KeyValuePair<GameObject, EcsPackedEntity> pair)
        {
            return !pair.Value.Unpack(m_World, out var entity);
        }
    }
}
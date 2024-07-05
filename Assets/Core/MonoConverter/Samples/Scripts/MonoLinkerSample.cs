using Core.Logger;
using Core.MonoConverter;
using Leopotam.EcsLite;
using UnityEngine;

namespace MonoConverter.Samples
{
    public class MonoLinkerSample : MonoBehaviour
    {
        [SerializeField] private MonoLinker m_Linker;

        private void Awake()
        {
            var logger = Debug
                .unityLogger
                .WithPrefix(typeof(MonoLinkerSample).ToPrefix());

            m_Linker.LinkTo(new EcsPackedEntityWithWorld());
            logger.Log(m_Linker.IsLinkExist<T1>());
            logger.Log(m_Linker.IsLinkExist<T2>());
            logger.Log(m_Linker.IsLinkExist<T3>());
            logger.Log(m_Linker.IsLinkExist<T4>());
        }
    }
}
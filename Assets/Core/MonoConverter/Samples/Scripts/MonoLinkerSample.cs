using Leopotam.EcsLite;
using UnityEngine;

namespace Core.MonoConverter
{
    public class MonoLinkerSample : MonoBehaviour
    {
        [SerializeField] private MonoLinker m_Linker;

        private void Awake()
        {
            m_Linker.LinkTo(new EcsPackedEntityWithWorld());
        }
    }
}
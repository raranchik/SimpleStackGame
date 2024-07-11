using TMPro;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Fps
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Tmp;

        public void SetText(string text)
        {
            m_Tmp.text = text;
        }
    }
}
using Core.Container.Links;
using Core.MonoConverter;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Container.Mono
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class Grid2SizeMonoLink : MonoLink<Grid2SizeLink>
    {
// #if UNITY_EDITOR
//         [SerializeField] private Color m_GridColor = Color.red;
//         [SerializeField] private float m_SphereRadius;
//
//         private void OnValidate()
//         {
//             m_SphereRadius = Mathf.Clamp(m_SphereRadius, 0.1f, m_Value.CellScale);
//         }
//
//         private void OnDrawGizmos()
//         {
//             var size = m_Value.Size;
//             if (size.x <= 0 || size.y <= 0)
//             {
//                 return;
//             }
//
//             Gizmos.color = m_GridColor;
//             var cellScale = new Vector3(m_Value.CellScale, 0f, m_Value.CellScale);
//             var thisTransform = GetComponent<Transform>();
//             var sphereRadius = m_Value.CellScale * m_SphereRadius;
//             for (var x = 0; x < size.x; x++)
//             {
//                 for (var z = 0; z < size.y; z++)
//                 {
//                     var cellPosition = new Vector3(x, 0f, z) * m_Value.CellScale + thisTransform.position;
//                     Gizmos.DrawWireCube(cellPosition, cellScale);
//                     Gizmos.DrawSphere(cellPosition, sphereRadius);
//                 }
//             }
//         }
// #endif
    }
}
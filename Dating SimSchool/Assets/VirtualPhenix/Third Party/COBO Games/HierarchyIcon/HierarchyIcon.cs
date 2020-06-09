using UnityEngine;

namespace Helpers
{
    public class HierarchyIcon : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public Texture2D icon;
        [HideInInspector] public string tooltip;
        [HideInInspector] public int position = 0;
        public bool showIcon = false;
#endif
    }
}
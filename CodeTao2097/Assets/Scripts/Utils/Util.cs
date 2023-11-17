using System.Collections.Generic;
using UnityEngine;

namespace CodeTao
{
    public static class Util
    {
        public static bool IsTagIncluded(string tag, List<ETag> tags)
        {
            foreach (var t in tags)
            {
                if (tag == t.ToString())
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetComponentInSiblings<T>(Component component) where T : Component
        {
            Transform transform = component.transform;
            if (transform.parent == null)
            {
                return null;
            }

            Transform parent = transform.parent;
            foreach (Transform sibling in parent)
            {
                if (sibling != transform)
                {
                    T comp = sibling.GetComponent<T>();
                    if (comp != null)
                    {
                        return comp;
                    }
                }
            }

            return null;
        }

        public static string GetTagFromParent(Component component)
        {
            return component.transform.parent.tag;
        }
    }
}
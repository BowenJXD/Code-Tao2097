using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// unity layers
    /// </summary>
    public enum ELayer
    {
        Null = 0,
        Enemy = 7,
        Player = 8,
        Collectable = 9,
        Interactable = 10,
        Building = 11,
    }

    public static class LayerUtil
    {
        public static void IncludeLayer(this Rigidbody2D rb, ELayer layer)
        {
            int layerMask = 1 << LayerMask.NameToLayer(layer.ToString());
            rb.includeLayers |= layerMask;
            rb.excludeLayers &= ~(layerMask);
        }
        
        public static void ExcludeLayer(this Rigidbody2D rb, ELayer layer)
        {
            int layerMask = 1 << LayerMask.NameToLayer(layer.ToString());
            rb.excludeLayers |= layerMask;
            rb.includeLayers &= ~(layerMask);
        }
        
        public static void IncludeLayer(this Collider2D col, ELayer layer)
        {
            int layerMask = 1 << LayerMask.NameToLayer(layer.ToString());
            col.includeLayers |= layerMask;
            col.excludeLayers &= ~(layerMask);
        }
        
        public static void ExcludeLayer(this Collider2D col, ELayer layer)
        {
            int layerMask = 1 << LayerMask.NameToLayer(layer.ToString());
            col.excludeLayers |= layerMask;
            col.includeLayers &= ~(layerMask);
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

namespace Panty
{
    public class CanDragItem : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private Vector2 tempOffset;

        [SerializeField] private GameObject target;

        void IDragHandler.OnDrag(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Left && e.pointerPressRaycast.gameObject == target)
            {
                transform.position = e.position - tempOffset;
            }
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Left)
            {
                tempOffset = e.position - (Vector2)transform.position;
            }
        }
    }
}
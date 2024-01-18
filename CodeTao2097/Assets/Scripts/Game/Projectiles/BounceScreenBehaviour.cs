using System;
using System.Collections;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    public class BounceScreenBehaviour : UnitBehaviour
    {
        private Rigidbody2D rb;
        Camera mainCamera;
        private float triggerInterval = 0.1f;
        private bool isInCD;

        private void Start()
        {
            rb = Unit.GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            isInCD = false;
        }

        void OnBecameInvisible()
        {
            if (isInCD || !gameObject.activeSelf)
            {
                return;
            }
            Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector2 result = new Vector2(Mathf.Sign(rb.velocity.x), Mathf.Sign(rb.velocity.y));

            result.x = (screenPos.x < 0) ? 1 : (screenPos.x > Screen.width) ? -1 : result.x;
            result.y = (screenPos.y < 0) ? 1 : (screenPos.y > Screen.height) ? -1 : result.y;

            rb.velocity = new Vector2(result.x * Mathf.Abs(rb.velocity.x), result.y * Mathf.Abs(rb.velocity.y));

            float angle = Util.GetAngleFromVector(rb.velocity);
            Unit.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            isInCD = true;
            ActionKit.Delay(triggerInterval, () => isInCD = false).Start(this);
        }

        private void OnDisable()
        {
            isInCD = false;
        }
    }
}
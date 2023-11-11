using System.Collections;
using UnityEngine;

namespace QFramework
{
    public class ItemSelector : MonoBehaviour
    {
        private Transform targetCell;
        [SerializeField]
        private float mSpeed = 3;
        private bool mCanStartMove = true;
        public bool CanStartMove => mCanStartMove;
        public void SetTarget(Transform target)
        {
            if (mCanStartMove)
            {
                mCanStartMove = false;
                targetCell = target;
                StartCoroutine(ChangeState());
            }
        }
        private IEnumerator ChangeState()
        {
            yield return new WaitForSeconds(0.5f);
            mCanStartMove = true;
        }
        public void InitSize(Vector2 size)
        {
            (transform as RectTransform).sizeDelta = size;
        }
        public void ResetPos()
        {
            (transform as RectTransform).anchoredPosition = Vector2.zero;
        }
        private void Update()
        {
            if (targetCell == null) return;
            var nextDelta = Time.deltaTime * mSpeed;

            if ((transform.position - targetCell.position).sqrMagnitude <= nextDelta + 0.1f)
            {
                transform.position = targetCell.position;                
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, targetCell.position, nextDelta);
            }
        }
    }
}
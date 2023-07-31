
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public GameObject _chipForm;
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }

        public IEnumerator MoveFromTo(Vector3 startPosition, Vector3 endPosition, float time)
        {

            float currentTime = 0f;
            while (currentTime < time)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPosition;
            this.ResetMaterial();
        }
    }
}
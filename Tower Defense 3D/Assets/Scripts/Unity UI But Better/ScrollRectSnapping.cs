using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectSnapping : ScrollRect
{
    private readonly float _scrollTime = 0.3f;
    private readonly float _dragThreshold = 10f;

    private Coroutine _activeCoroutine = null;
    private Vector3 _clickPosition;

    private enum ScrollDirection
    {
        Left,
        Right
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        _clickPosition = eventData.position;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(_clickPosition.x - eventData.position.x) >= _dragThreshold)
        {
            if (_clickPosition.x > eventData.position.x)
                ScrollRight();
            else
                ScrollLeft();
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        _clickPosition = eventData.position;
    }

    public void ScrollRight()
    {
        Scroll(ScrollDirection.Right);
    }

    public void ScrollLeft()
    {
        Scroll(ScrollDirection.Left);
    }

    private void Scroll(ScrollDirection scrollDirection)
    {
        var scrollDelta = content.sizeDelta.x / content.childCount;

        var finalPosition = content.localPosition;

        if (scrollDirection == ScrollDirection.Right)
            scrollDelta = -scrollDelta;

        finalPosition.x += scrollDelta;

        if (_activeCoroutine == null)
        {
            _activeCoroutine = StartCoroutine(ScrollObject(content.localPosition, finalPosition));
        }
    }

    IEnumerator ScrollObject(Vector3 startingPosition, Vector3 finalPosition)
    {
        float currTime = 0f;

        while (currTime < _scrollTime)
        {
            currTime += Time.deltaTime;

            content.localPosition = Vector3.Lerp(startingPosition, finalPosition, currTime / _scrollTime);

            yield return new WaitForEndOfFrame();
        }
        
        _activeCoroutine = null;

        yield return null;
    }
}

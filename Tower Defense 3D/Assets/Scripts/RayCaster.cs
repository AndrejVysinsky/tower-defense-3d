﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RayCaster
{
    public static bool RayCastNearestUIObject(out RaycastResult raycastResult)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);

        raycastResults = raycastResults.OrderBy(x => x.distance).ToList();

        if (raycastResults.Count > 0)
        {
            raycastResult = raycastResults[0];
            return true;
        }
        else
        {
            raycastResult = new RaycastResult();
            return false;
        }
    }

    public static bool RayCastNearestGameObject(out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitInfos = Physics.RaycastAll(ray).ToList();
        
        hitInfos = hitInfos.OrderBy(x => x.distance).ToList();

        if (hitInfos.Count > 0)
        {
            hitInfo = hitInfos[0];
            return true;
        }
        else
        {
            hitInfo = new RaycastHit();
            return false;
        }
    }
}
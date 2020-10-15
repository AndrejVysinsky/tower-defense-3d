using UnityEngine;

public class Path : MonoBehaviour
{
    private PortalStart _portalStart;
    private PortalEnd _portalEnd;

    public PortalStart PortalStart 
    { 
        get 
        {
            if (_portalStart == null)
            {
                _portalStart = GameObject.FindGameObjectWithTag("Start").GetComponent<PortalStart>();
            }
            return _portalStart;
        }
        set
        {
            _portalStart = value;
        }
    }

    public PortalEnd PortalEnd
    {
        get
        {
            if (_portalEnd == null)
            {
                _portalEnd = GameObject.FindGameObjectWithTag("End").GetComponent<PortalEnd>();
            }
            return _portalEnd;
        }
        set
        {
            _portalEnd = value;
        }
    }
}

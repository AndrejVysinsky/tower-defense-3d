using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGridObjectRemoved
{
    /// <summary>
    /// After this call, Destroy(gameObject) is called
    /// </summary>
    void OnGridObjectRemoved();
}

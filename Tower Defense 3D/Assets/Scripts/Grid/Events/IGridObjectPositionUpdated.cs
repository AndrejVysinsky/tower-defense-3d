using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IGridObjectPositionUpdated
{
    Vector3 OnGridObjectPositionUpdated(Vector3 position);
}

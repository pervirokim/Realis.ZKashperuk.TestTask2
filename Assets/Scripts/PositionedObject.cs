using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionedObject : MonoBehaviour
{
    public Vector2 Position { get; private set; }

    public virtual void SetPosition(Vector2 position) => Position = position;

}

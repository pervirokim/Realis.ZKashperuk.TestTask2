using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : PositionedObject
{
    private Brick _brickInCell;

    public override void SetPosition(Vector2 position)
    {
        base.SetPosition(position);
        gameObject.name = $"Cell X:{position.x} Y:{position.y}";
    }

    public void PutBrick(Brick brick)
    {
        _brickInCell = brick;
        brick.SetPosition(Position);
    }

    public void RemoveBrick() => _brickInCell = null;

    public Brick GetBrick() => _brickInCell;
    public bool HasBrick() => _brickInCell is not null;

}

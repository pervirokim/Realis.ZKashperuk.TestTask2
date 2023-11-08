using System;
using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    private Vector2 _firstPressPos;
    private Vector2 _secondPressPos;
    private Vector2 _currentSwipe;
    private float _offset = 0.5f;

    public static Action<Vector2> onSwipe;

    void Update()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                _firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                _secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                _currentSwipe = new Vector3(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

                //normalize the 2d vector
                _currentSwipe.Normalize();

                if (_currentSwipe.y > 0 && _currentSwipe.x > -_offset && _currentSwipe.x < _offset)
                    onSwipe.Invoke(Vector2.up);
                if (_currentSwipe.y < 0 && _currentSwipe.x > -_offset && _currentSwipe.x < _offset)
                    onSwipe.Invoke(Vector2.down);
                if (_currentSwipe.x < 0 && _currentSwipe.y > -_offset && _currentSwipe.y < _offset)
                    onSwipe.Invoke(Vector2.left);
                if (_currentSwipe.x > 0 && _currentSwipe.y > -_offset && _currentSwipe.y < _offset)
                    onSwipe.Invoke(Vector2.right);
            }
        }
    }
}

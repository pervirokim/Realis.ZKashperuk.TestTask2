using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickColorsService : IBrickColorsService
{
    public int startValue = 2;
    public int bricksMergeCount = 11;

    public Dictionary<int, Color> _colors = new Dictionary<int, Color>();

    public BrickColorsService()
    {
        Reset();
    }

    public void Reset()
    {
        _colors= new Dictionary<int, Color>();
        int currentValue = startValue;
        for (int i = 0; i < bricksMergeCount; i++)
        {
            if (i != 0)
                currentValue *= 2;

            _colors.Add(currentValue, Random.ColorHSV());
        }
    }
    public Color GetColorByValue(int value) => _colors[value];

}

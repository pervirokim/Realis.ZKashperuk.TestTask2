using UnityEngine;

public interface IBrickColorsService 
{
    public Color GetColorByValue(int value);
    public void Reset();
}

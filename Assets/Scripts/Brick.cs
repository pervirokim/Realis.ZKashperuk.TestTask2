using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class Brick : PositionedObject
{
    public Color Color { get; private set; }
    public int Value { get; private set; }

    public void Merge(Brick brick) => _brickToMerge = brick;
    public Brick GetMergeBlock() => _brickToMerge;

    private Brick _brickToMerge;
    private TextMeshProUGUI _textMeshProUGUI;
    private SpriteRenderer _spriteRenderer;
    private IBrickColorsService _colorService;

    [Inject]
    public void Construct(IBrickColorsService colorService)
    {
        _colorService = colorService;
        Init(2);
    }

    public void Init(int value)
    {
        _textMeshProUGUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        Value = value;
        _textMeshProUGUI.text = Value.ToString();
        gameObject.name = $"Brick val:{value}";

        Color = _colorService.GetColorByValue(value);
        _spriteRenderer.color = Color;
    }

}

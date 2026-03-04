using UnityEngine.UIElements;
using UnityEngine;

[UxmlElement]
public partial class SDCNumericField : FloatField
{
    public delegate void OutOfRangeEvent();
    public event OutOfRangeEvent JustBecameOutOfRange;

    public delegate void InsideRangeEvent();
    public event InsideRangeEvent JustEnteredRange;

    // properties
    [UxmlAttribute]
    public float minValue = 0f;

    [UxmlAttribute]
    public float maxValue = 100f;

    [UxmlAttribute]
    public bool allowNegatives = true;

    [UxmlAttribute]
    public bool allowFloatingPoint = true;

    public SDCNumericField() : base()
    {
        // verify what characters are allowed
        this.Q<TextElement>().RegisterCallback<KeyDownEvent>(evt =>
        {
            // minus check
            if (allowNegatives && evt.character == '-' && string.IsNullOrEmpty(this.text))
            {
                return;
            }

            // floating point check
            if (allowFloatingPoint && evt.character == '.' && !this.text.Contains('.'))
            {
                return;
            }

            if (!char.IsDigit(evt.character) && evt.keyCode != KeyCode.Backspace)
            {
                evt.StopPropagation();
            }
        }, TrickleDown.TrickleDown);

        // if out of range, error color
        this.RegisterCallback<ChangeEvent<float>>(evt =>
        {
            float value = evt.newValue;
            if (value < minValue || value > maxValue)
            {
                if (!this.ClassListContains("base-field-error"))
                {
                    this.AddToClassList("base-field-error");
                    JustBecameOutOfRange?.Invoke();
                }
            }
            else
            {
                if (this.ClassListContains("base-field-error"))
                {
                    this.RemoveFromClassList("base-field-error");
                    JustEnteredRange?.Invoke();
                }
            }
        });
    }
}
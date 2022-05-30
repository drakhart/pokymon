using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Color BarColor
    {
        get
        {
            var localScale = transform.localScale.x;

            if (localScale <= 0.15)
            {
                return new Color32(0xDE, 0x52, 0x3A, 0xFF);
            }
            else if (localScale <= 0.5f)
            {
                return new Color32(0xE4, 0x92, 0x27, 0xFF);
            }
            else
            {
                return new Color32(0x45, 0xA0, 0x4E, 0xFF);
            }
        }
    }

    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// Update the bar length given a normalized value
    /// </summary>
    /// <param name="normalizedValue">Value normalized between 0 and 1</param>
    public void SetLength(float normalizedValue)
    {
        StartCoroutine(AnimateNewLength(normalizedValue));
    }

    private IEnumerator AnimateNewLength(float normalizedValue)
    {
        float currentValue = transform.localScale.x;
        float updateQuantity = currentValue - normalizedValue;

        while (currentValue - normalizedValue > Mathf.Epsilon)
        {
            currentValue -= updateQuantity * Time.deltaTime;
            transform.localScale = new Vector3(currentValue, 1);
            _image.color = BarColor;

            yield return null;
        }

        transform.localScale = new Vector3(normalizedValue, 1);
        _image.color = BarColor;
    }
}

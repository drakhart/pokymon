using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bar : MonoBehaviour
{
    [SerializeField] private bool _hasFixedColor;

    private Image _image;

    public Color GetBarColor(float finalScale)
    {
        if (finalScale <= 0.15)
        {
            return new Color32(0xDE, 0x52, 0x3A, 0xFF);
        }
        else if (finalScale <= 0.5f)
        {
            return new Color32(0xE4, 0x92, 0x27, 0xFF);
        }
        else
        {
            return new Color32(0x45, 0xA0, 0x4E, 0xFF);
        }
    }

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetScale(float finalScale)
    {
        transform.localScale = new Vector3(finalScale, 1);

        if (!_hasFixedColor)
        {
            _image.color = GetBarColor(finalScale);
        }
    }

    public YieldInstruction SetScaleAnimated(float finalScale)
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScaleX(finalScale, 1f));

        if (!_hasFixedColor)
        {
            seq.Join(GetComponent<Image>().DOColor(GetBarColor(finalScale), 1f));
        }

        return seq.Play().WaitForCompletion();
    }
}

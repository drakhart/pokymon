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
            return ColorManager.SharedInstance.BarLow;
        }
        else if (finalScale <= 0.5f)
        {
            return ColorManager.SharedInstance.BarMedium;
        }
        else
        {
            return ColorManager.SharedInstance.BarHigh;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bar : MonoBehaviour
{
    [SerializeField] private bool _hasFixedColor = false;
    [SerializeField] private float _highThreshold = 0.5f;
    [SerializeField] private float _lowThreshold = 0.20f;

    private Image _image;

    private Color BarColor(float finalScale)
    {
        if (finalScale <= _lowThreshold)
        {
            return ColorManager.SharedInstance.LowBar;
        }
        else if (finalScale > _highThreshold)
        {
            return ColorManager.SharedInstance.HighBar;
        }
        else
        {
            return ColorManager.SharedInstance.MediumBar;
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
            _image.color = BarColor(finalScale);
        }
    }

    public YieldInstruction SetScaleAnimated(float finalScale)
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScaleX(finalScale, 1f));

        if (!_hasFixedColor)
        {
            seq.Join(GetComponent<Image>().DOColor(BarColor(finalScale), 1f));
        }

        return seq.Play().WaitForCompletion();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator
{
    private SpriteRenderer _renderer;
    private List<Sprite> _frameList;
    private float _frameRate;

    private int _currentFrame;
    private float _timer;

    public CustomAnimator(SpriteRenderer renderer, List<Sprite> frameList, float frameRate = 0.0625f)
    {
        _renderer = renderer;
        _frameList = frameList;
        _frameRate = frameRate;
    }

    public void Start() {
        _currentFrame = 0;
        _timer = 0;

        Render();
    }

    public void Update() {
        _timer += Time.deltaTime;

        if (_timer > _frameRate)
        {
            _currentFrame = (_currentFrame + 1) % _frameList.Count;
            _timer -= _frameRate;

            Render();
        }
    }

    public void Render() {
        _renderer.sprite = _frameList[_currentFrame];
    }
}

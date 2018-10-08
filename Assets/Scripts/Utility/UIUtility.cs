using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtility : MonoSingleton<UIUtility>
{
    // TODO: Rotation, scale
    #region General - MoveToPosition, SetGroupPositions

    /// <summary>
    /// Move singular rect transform to target position
    /// </summary>
    /// <param name="_rTransform"></param>
    /// <param name="_target"></param>
    /// <param name="_travelTime"></param>
    /// <returns></returns>
    public IEnumerator MoveTransformOverTime(RectTransform _rTransform, Vector2 _target, float _travelTime)
	{
        float elapsedTime = 0.0f;
        Vector3 startPosition = _rTransform.anchoredPosition;

        while (elapsedTime < _travelTime)
		{
            _rTransform.anchoredPosition = Vector3.Lerp(startPosition, _target, (elapsedTime / _travelTime));
            elapsedTime += Time.deltaTime;
			yield return null;
		}

        _rTransform.anchoredPosition = _target;
        yield return null;
    }

    public IEnumerator MoveTransformOverTimeSmoothStep(RectTransform _rTransform, Vector2 _target, float _travelTime)
    {
        float elapsedTime = 0.0f;
        Vector2 startPosition = _rTransform.anchoredPosition;

        while (elapsedTime < _travelTime)
        {
            _rTransform.anchoredPosition = new Vector2(
                Mathf.SmoothStep(startPosition.x, _target.x, (elapsedTime / _travelTime)),
                Mathf.SmoothStep(startPosition.y, _target.y, (elapsedTime / _travelTime)));

            //_rTransform.anchoredPosition = Vector3.Slerp(startPosition, _target, (elapsedTime / _travelTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rTransform.anchoredPosition = _target;
        yield return null;
    }

    /// <summary>
    /// Move singular rect transform to target position
    /// </summary>
    /// <param name="_rTransform"></param>
    /// <param name="_target"></param>
    /// <param name="_speed"></param>
    /// <returns></returns>
    public IEnumerator MoveTransformBySpeed(RectTransform _rTransform, Vector2 _target, float _speed)
	{
        float distance = Vector3.Distance(_rTransform.anchoredPosition, _target);
        float travelTime = distance / _speed;

        yield return this.StartCoroutine(this.MoveTransformOverTime(_rTransform, _target, travelTime));
    }

    /// <summary>
    /// Move list of rect transforms to target positions
    /// </summary>
    /// <param name="_rTransforms"></param>
    /// <param name="_targets"></param>
    /// <param name="_travelTime"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator MoveTransformsOverTime(List<RectTransform> _rTransforms, List<Vector2> _targets, float _travelTime, bool _isSimultaneous)
	{
		if(_isSimultaneous)
		{
			for(int i = 0; i < _rTransforms.Count; i++)
			{
				if(i < _rTransforms.Count -1)
				{
					this.StartCoroutine(this.MoveTransformOverTime(_rTransforms[i], _targets[i], _travelTime));
				}
				else
				{
					yield return this.StartCoroutine(this.MoveTransformOverTime(_rTransforms[i], _targets[i], _travelTime));
				}
			}
		}
		else
		{
			for(int i = 0; i < _rTransforms.Count; i++)
			{
				yield return this.StartCoroutine(this.MoveTransformOverTime(_rTransforms[i], _targets[i], _travelTime));
			}
		}
	}

    /// <summary>
    /// Move list of rect transforms to target positions
    /// </summary>
    /// <param name="_rTransforms"></param>
    /// <param name="_targets"></param>
    /// <param name="_speed"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator MoveTransformsBySpeed(List<RectTransform> _rTransforms, List<Vector2> _targets, float _speed, bool _isSimultaneous)
	{
		if(_isSimultaneous)
		{
			for(int i = 0; i < _rTransforms.Count; i++)
			{
				if(i < _rTransforms.Count -1)
				{
					this.StartCoroutine(this.MoveTransformBySpeed(_rTransforms[i], _targets[i], _speed));
				}
				else
				{
					yield return this.StartCoroutine(this.MoveTransformBySpeed(_rTransforms[i], _targets[i], _speed));
				}
			}
		}
		else
		{
			for(int i = 0; i < _rTransforms.Count; i++)
			{
				yield return this.StartCoroutine(this.MoveTransformBySpeed(_rTransforms[i], _targets[i], _speed));
			}
		}
	}

    /// <summary>
    /// Set list of rect transforms' positions
    /// </summary>
    /// <param name="_rTransforms"></param>
    /// <param name="_targets"></param>
    public void SetTransformListPositions(List<RectTransform> _rTransforms, Vector2[] _targets)
	{
		for(int i = 0; i < _rTransforms.Count; i++)
		{
			_rTransforms[i].anchoredPosition = _targets[i];
		}
	}


    public IEnumerator ScaleOverTime(RectTransform _rTransform, float _targetScale, float _scaleTime, bool _isSmoothStep)
    {
        float elapsedTime = 0.0f;

        if (_isSmoothStep)
        {
            float startScale = _rTransform.localScale.x;
            float currentScale = 0.0f;

            while (elapsedTime < _scaleTime)
            {
                if (_rTransform)
                {
                    currentScale = Mathf.SmoothStep(startScale, _targetScale, (elapsedTime / _scaleTime));
                    _rTransform.localScale = new Vector3(currentScale, currentScale, 1.0f);
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            if (_rTransform)
                _rTransform.localScale = new Vector3(currentScale, currentScale, 1.0f);
        }
        else
        {
            Vector3 startScale = _rTransform.localScale;
            Vector3 targetScale = new Vector3(_targetScale, _targetScale, 1.0f);

            while (elapsedTime < _scaleTime)
            {
                if (_rTransform)
                {
                    _rTransform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / _scaleTime));
                }
                
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            if (_rTransform)
                _rTransform.localScale = targetScale;
        }

        yield return null;
    }

    public IEnumerator ScaleListOverTime(List<RectTransform> _rTransforms, float _targetScale, float _scaleTime, bool _isSmoothStep, bool _isSimultaneous)
    {
        if (_isSimultaneous)
        {
            for (int i = 0; i < _rTransforms.Count; i++)
            {
                if (i < _rTransforms.Count - 1)
                {
                    this.StartCoroutine(this.ScaleOverTime(_rTransforms[i], _targetScale, _scaleTime, _isSmoothStep));
                }
                else
                {
                    yield return this.StartCoroutine(this.ScaleOverTime(_rTransforms[i], _targetScale, _scaleTime, _isSmoothStep));
                }
            }
        }
        else
        {
            for (int i = 0; i < _rTransforms.Count; i++)
            {
                yield return this.StartCoroutine(this.ScaleOverTime(_rTransforms[i], _targetScale, _scaleTime, _isSmoothStep));
            }
        }

        yield return null;
    }

    // GUTTERTRASH
    private IEnumerator RotateTransformOverTime(RectTransform _rTransform, float _angle, float _rotationTime, bool _isRotatingRight)
    {
        float elapsedTime = 0.0f;
        Quaternion startRotation = _rTransform.rotation;
        Quaternion targetRotation = startRotation;
        targetRotation.eulerAngles = _isRotatingRight ? 
            new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + _angle) : 
            new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z - _angle);

        while (elapsedTime < _rotationTime)
        {
            _rTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, (elapsedTime / _rotationTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _rTransform.rotation = targetRotation;
        yield return null;
    }

    #endregion

    #region Text

    /// <summary>
    /// Fade singular text component's alpha to target alpha
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_targetAlpha"></param>
    /// <returns></returns>
    public IEnumerator FadeOverTime(Text _text, float _fadeTime, float _targetAlpha)
	{
        float elapsedTime = 0.0f;
        float startAlpha = _text.color.a;
        float currentAlpha = startAlpha;

        while (elapsedTime < _fadeTime)
        {
            currentAlpha = Mathf.Lerp(startAlpha, _targetAlpha, (elapsedTime / _fadeTime));
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, currentAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _targetAlpha);
        yield return null;
	}

    /// <summary>
    /// Fade singular text component's alpha to target alpha
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_fadeSpeed"></param>
    /// <param name="_targetAlpha"></param>
    /// <returns></returns>
    public IEnumerator FadeBySpeed(Text _text, float _fadeSpeed, float _targetAlpha)
	{
		float alpha = _text.color.a;
        float distance = alpha > _targetAlpha ? alpha - _targetAlpha : _targetAlpha - alpha;
        float fadeTime = distance / _fadeSpeed;

        yield return this.StartCoroutine(this.FadeOverTime(_text, fadeTime, _targetAlpha));

        yield return null;
	}

    /// <summary>
    /// Fade list of text components' alpha to target alpha
    /// </summary>
    /// <param name="_textList"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator FadeListOverTime(List<Text> _textList, float _fadeTime, float _targetAlpha,	bool _isSimultaneous)
	{
		if(_isSimultaneous)
		{
			for(int i = 0; i < _textList.Count; i++)
			{
				if(i < _textList.Count - 1)
				{
					this.StartCoroutine(this.FadeOverTime(_textList[i], _fadeTime, _targetAlpha));
				}
				else
				{
					yield return this.StartCoroutine(this.FadeOverTime(_textList[i], _fadeTime, _targetAlpha));
				}
			}
		}
		else
		{
			for(int i = 0; i < _textList.Count; i++)
			{
				yield return this.StartCoroutine(this.FadeOverTime(_textList[i], _fadeTime, _targetAlpha));
			}
		}
	}

    /// <summary>
    /// Fade list of text components' alpha to target alpha
    /// </summary>
    /// <param name="_textList"></param>
    /// <param name="_fadeSpeed"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator FadeListBySpeed(List<Text> _textList, float _fadeSpeed, float _targetAlpha, bool _isSimultaneous)
	{
		if(_isSimultaneous)
		{
			for(int i = 0; i < _textList.Count; i++)
			{
				if(i < _textList.Count -1)
				{
					this.StartCoroutine(this.FadeBySpeed(_textList[i], _fadeSpeed, _targetAlpha));
				}
				else
				{
					yield return this.StartCoroutine(this.FadeBySpeed(_textList[i], _fadeSpeed, _targetAlpha));
				}
			}
		}
		else
		{
			for(int i = 0; i < _textList.Count; i++)
			{
				yield return this.StartCoroutine(this.FadeBySpeed(_textList[i], _fadeSpeed, _targetAlpha));
			}
		}

		yield return null;
	}

    /// <summary>
    /// Shift singular text component's colour to target colour
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_shiftTime"></param>
    /// <param name="_targetColour"></param>
    /// <returns></returns>
    public IEnumerator ShiftColour(Text _text, float _shiftTime, Color _targetColour)
    {
		Color startCol = _text.color;
		float elapsedTime = 0.0f;

		while(elapsedTime < _shiftTime)
		{
            // For fading and colour shifting at the same time
            startCol = new Color(startCol.r, startCol.g, startCol.b, _text.color.a);
            _targetColour = new Color(_targetColour.r, _targetColour.g, _targetColour.b, _text.color.a);

            _text.color = Color.Lerp(startCol, _targetColour, elapsedTime / _shiftTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

        _text.color = _targetColour;
        yield return null;
	}

    //  TODO: test
    /// <summary>
    /// Shift list of text components' colour to target colour
    /// </summary>
    /// <param name="_textList"></param>
    /// <param name="_targetColour"></param>
    /// <param name="_shiftTime"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator ShiftListColours(List<Text> _textList, Color _targetColour, float _shiftTime, bool _isSimultaneous)
	{
		for(int i = 0; i < _textList.Count; i++)
		{
			if(_isSimultaneous)
			{
				if(i < _textList.Count - 1)
				{
					this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour));
				}
				else
				{
					yield return this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour));
				}
			}
			else
			{
				this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour));
			}
		}
		yield return null;
	}

    /// <summary>
    /// Shift list of text components' colour to target colour
    /// </summary>
    /// <param name="_textList"></param>
    /// <param name="_targetColour"></param>
    /// <param name="_shiftTime"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator ShiftListColours(List<Text> _textList, List<Color> _targetColour, float _shiftTime, bool _isSimultaneous)
    {
        for (int i = 0; i < _textList.Count; i++)
        {
            if (_isSimultaneous)
            {
                if (i < _textList.Count - 1)
                {
                    this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour[i]));
                }
                else
                {
                    yield return this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour[i]));
                }
            }
            else
            {
                this.StartCoroutine(this.ShiftColour(_textList[i], _shiftTime, _targetColour[i]));
            }
        }
        yield return null;
    }

    #endregion

    #region Images

    /// <summary>
    /// Fade singular image component's alpha to target alpha
    /// </summary>
    /// <param name="_image"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_targetAlpha"></param>
    /// <returns></returns>
    public IEnumerator FadeOverTime(Image _image, float _fadeTime, float _targetAlpha)
    {
        float elapsedTime = 0.0f;
        float startAlpha = _image.color.a;
        float currentAlpha = startAlpha;

        while (elapsedTime < _fadeTime)
        {
            currentAlpha = Mathf.Lerp(startAlpha, _targetAlpha, (elapsedTime / _fadeTime));
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, currentAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _targetAlpha);
        yield return null;
    }

    /// <summary>
    /// Fade singular image component's alpha to target alpha
    /// </summary>
    /// <param name="_image"></param>
    /// <param name="_fadeSpeed"></param>
    /// <param name="_targetAlpha"></param>
    /// <returns></returns>
    public IEnumerator FadeBySpeed(Image _image, float _fadeSpeed, float _targetAlpha)
    {
		float alpha = _image.color.a;
        float distance = alpha > _targetAlpha ? alpha - _targetAlpha : _targetAlpha - alpha;
        float fadeTime = distance / _fadeSpeed;

        yield return this.StartCoroutine(this.FadeOverTime(_image, fadeTime, _targetAlpha));
	}

    /// <summary>
    /// Fade list of image components' alpha to target alpha
    /// </summary>
    /// <param name="_images"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
	public IEnumerator FadeListOverTime(List<Image> _images, float _fadeTime, float _targetAlpha, bool _isSimultaneous)
    {
		for(int i = 0; i < _images.Count; i++)
		{
			if(_isSimultaneous)
			{
				if(i < _images.Count - 1)
				{
					this.StartCoroutine(this.FadeOverTime(_images[i], _fadeTime, _targetAlpha));
				}
				else
				{
					yield return this.StartCoroutine(this.FadeOverTime(_images[i], _fadeTime, _targetAlpha));
				}
			}
			else
			{
				yield return this.StartCoroutine(this.FadeOverTime(_images[i], _fadeTime, _targetAlpha));
			}
		}
		yield return null;
	}

    /// <summary>
    /// Fade list of text components' alpha to target alpha
    /// </summary>
    /// <param name="_images"></param>
    /// <param name="_isFadingIn"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_fadeSpeed"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
	public IEnumerator FadeListBySpeed(List<Image> _images, float _fadeSpeed, float _targetAlpha, bool _isSimultaneous)
    {
		for(int i = 0; i < _images.Count; i++)
		{
			if(_isSimultaneous)
			{
				if(i < _images.Count - 1)
				{
					this.StartCoroutine(this.FadeBySpeed(_images[i], _fadeSpeed, _targetAlpha));
				}
				else
				{
					yield return this.StartCoroutine(this.FadeBySpeed(_images[i], _fadeSpeed, _targetAlpha));
				}
			}
			else
			{
				yield return this.StartCoroutine(this.FadeBySpeed(_images[i], _fadeSpeed, _targetAlpha));
			}
		}
		yield return null;
	}

    /// <summary>
    /// Shift singular image component's colour to target colour
    /// </summary>
    /// <param name="_image"></param>
    /// <param name="_shiftTime"></param>
    /// <param name="_targetColour"></param>
    /// <returns></returns>
    public IEnumerator ShiftColour(Image _image, float _shiftTime, Color _targetColour)
    {
		Color startCol = _image.color;
		float elapsedTime = 0.0f;

		while(elapsedTime < _shiftTime)
		{
            // For fading and colour shifting at the same time
            startCol = new Color(startCol.r, startCol.g, startCol.b, _image.color.a);
            _targetColour = new Color(_targetColour.r, _targetColour.g, _targetColour.b, _image.color.a);

            _image.color = Color.Lerp(startCol, _targetColour, elapsedTime / _shiftTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

        _image.color = _targetColour;
        yield return null;
	}

    //  TODO: test
    /// <summary>
    /// Shift list of image components' colour to target colour
    /// </summary>
    /// <param name="_images"></param>
    /// <param name="_shiftTime"></param>
    /// <param name="_targetColour"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator ShiftListColours(List<Image> _images, float _shiftTime, Color _targetColour, bool _isSimultaneous)
    {
		for(int i = 0; i < _images.Count; i++)
		{
			if(_isSimultaneous)
			{
				if(i < _images.Count - 1)
				{
					this.StartCoroutine(this.ShiftColour(_images[i], _shiftTime, _targetColour));
				}
				else
				{
					yield return this.StartCoroutine(this.ShiftColour(_images[i], _shiftTime, _targetColour));
				}
			}
			else
			{
				this.StartCoroutine(this.ShiftColour(_images[i], _shiftTime, _targetColour));
			}
		}
		yield return null;
	}

    #endregion

    // TODO: test
    #region Containers (Buttons, etc)
        
    /// <summary>
    /// Fade all text & image components found within a container
    /// </summary>
    /// <param name="_rTransform"></param>
    /// <param name="_fadeTime"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    public IEnumerator FadeOverTime(RectTransform _rTransform, float _fadeTime, float _targetAlpha, bool _isSimultaneous)
    {
        List<Text> textList = new List<Text>();
        List<Image> imageList = new List<Image>();

        // Find out the type of component the parent has & add it to the list
        if (_rTransform.GetComponent<Text>())
        {
            textList.Add(_rTransform.GetComponent<Text>());
        }
        if (_rTransform.GetComponent<Image>())
        {
            imageList.Add(_rTransform.GetComponent<Image>());
        }

        // Store references to any child object containing a txt/img components
        textList.AddRange(_rTransform.GetComponentsInChildren<Text>());
        imageList.AddRange(_rTransform.GetComponentsInChildren<Image>());

        if (_isSimultaneous)
        {
            if (textList.Count > imageList.Count)
            {
                this.StartCoroutine(this.FadeListOverTime(imageList, _fadeTime, _targetAlpha, _isSimultaneous));
                yield return this.StartCoroutine(this.FadeListOverTime(textList, _fadeTime, _targetAlpha, _isSimultaneous));
            }
            else
            {
                this.StartCoroutine(this.FadeListOverTime(textList, _fadeTime, _targetAlpha, _isSimultaneous));
                yield return this.StartCoroutine(this.FadeListOverTime(imageList, _fadeTime, _targetAlpha, _isSimultaneous));
            }
        }
        else
        {
            // TODO: Text always fades first then image....
            yield return this.StartCoroutine(this.FadeListOverTime(textList, _fadeTime, _targetAlpha, _isSimultaneous));
            yield return this.StartCoroutine(this.FadeListOverTime(imageList, _fadeTime, _targetAlpha, _isSimultaneous));
        }

        yield return null;
    }

    /// <summary>
    /// Fade all text & image components found within a container
    /// </summary>
    /// <param name="_rTransform"></param>
    /// <param name="_fadeSpeed"></param>
    /// <param name="_targetAlpha"></param>
    /// <param name="_isSimultaneous"></param>
    /// <returns></returns>
    private IEnumerator FadeContainerBySpeed(RectTransform _rTransform, float _fadeSpeed, float _targetAlpha, bool _isSimultaneous)
    {
		List<Text> textList = new List<Text>();
		List<Image> imageList = new List<Image>();

		// Find out the type of component the parent has & add it to the list
		if(_rTransform.GetComponent<Text>())
		{
			textList.Add(_rTransform.GetComponent<Text>());
		}
        if (_rTransform.GetComponent<Image>())
		{
			imageList.Add(_rTransform.GetComponent<Image>());
		}

		// Store references to any child object containing a txt/img components
		textList.AddRange(_rTransform.GetComponentsInChildren<Text>());
		imageList.AddRange(_rTransform.GetComponentsInChildren<Image>());

        if (_isSimultaneous)
        {
            if (textList.Count > imageList.Count)
            {
                this.StartCoroutine(this.FadeListBySpeed(imageList, _fadeSpeed, _targetAlpha, _isSimultaneous));
                yield return this.StartCoroutine(this.FadeListBySpeed(textList, _fadeSpeed, _targetAlpha, _isSimultaneous));
            }
            else
            {
                this.StartCoroutine(this.FadeListBySpeed(textList, _fadeSpeed, _targetAlpha, _isSimultaneous));
                yield return this.StartCoroutine(this.FadeListBySpeed(imageList, _fadeSpeed, _targetAlpha, _isSimultaneous));
            }
        }
        else
        {
            // TODO: Text always fades first then image....
            yield return this.StartCoroutine(this.FadeListBySpeed(textList, _fadeSpeed, _targetAlpha, _isSimultaneous));
            yield return this.StartCoroutine(this.FadeListBySpeed(imageList, _fadeSpeed, _targetAlpha, _isSimultaneous));
        }

		yield return null;
	}

    #endregion

    public override IEnumerator Initialize()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Run()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Stop()
    {
        throw new NotImplementedException();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlay : SingletonBehaviour<GameOverlay>
{
    [Header("Game Objects")]
    public Text Text;
    public CanvasGroup OverlayGroup;
    public Image OverlayBackground;

    [Header("Networking UI")]
    public GameObject MasterClientUI;
    public GameObject NormalClientUI;
    public Image MasterGameOverScreen;
    public Text MasterGameOverPlayerText;

    [Header("Countdown UI")]
    public Text CountdownText;

    [Space()]
    [Header("Animation")]
    public float FadeInTime;
    public float FadeOutTime;

    private float _fadeTime, _startFadeTime;
    private float _startAlpha;
    private float _targetAlpha;

    protected override void setInstance()
    {
        instance = this;
        _init();
    }

    private void _init()
    {
        SetText(string.Empty);
        OverlayGroup.alpha = 0f;

        if(MasterGameOverScreen != null)
        {
            MasterGameOverScreen.gameObject.SetActive(false);
        }        
    }

    public void SetNormalClientUI()
    {
        if (NormalClientUI != null)
        {
            NormalClientUI.SetActive(true);
        }
        if (MasterClientUI != null)
        {
            MasterClientUI.SetActive(false);
        }
    }

    public void SetMasterClientUI()
    {
        if (MasterClientUI != null)
        {
            MasterClientUI.SetActive(true);
        }
        if (NormalClientUI != null)
        {
            NormalClientUI.SetActive(false);
        }
    }

    public void SetCountdwonText(int num)
    {
        if (CountdownText == null) return;

        if(num == 0)
        {
            CountdownText.text = "SHAKE 'EM!";
        }
        else
        {
            CountdownText.text = num.ToString();
        }
    }

    public void SetMasterGameOver(string winningPlayer)
    {
        if (MasterGameOverScreen != null)
        {
            MasterGameOverScreen.gameObject.SetActive(true);
        }
        if(MasterGameOverPlayerText != null)
        {
            MasterGameOverPlayerText.text = winningPlayer + " popped first!";
        }
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void FadeIn(float delay = 0f, Color? color = null)
    {
        if(color != null && color.HasValue)
            _setColor(color.Value);

        _startFadeTime = FadeInTime;
        _fadeTime = _startFadeTime + delay;
        _startAlpha = OverlayGroup.alpha;
        _targetAlpha = 1f;
    }

    public void FadeOut(float delay = 0f, Color? color = null)
    {
        if(color != null && color.HasValue)
            _setColor(color.Value);

        _startFadeTime = FadeOutTime;
        _fadeTime = _startFadeTime + delay;
        _startAlpha = OverlayGroup.alpha;
        _targetAlpha = 0f;
    }

    private void _setColor(Color c)
    {
        OverlayBackground.color = c;
    }

    private void Update()
    {
        if (_fadeTime > 0)
            _updateFade();
    }

    private void _updateFade()
    {
        _fadeTime -= Time.deltaTime;

        // handle delay
        if (_fadeTime > _startFadeTime)
            return;

        OverlayGroup.alpha = Mathf.Lerp(
            _startAlpha, 
            _targetAlpha, 
            1f - _fadeTime / _startFadeTime);
    }
}
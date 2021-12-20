using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MorseTranslator : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private MorseSoundGenerator _soundGenerator;
    [SerializeField] private TextMeshProUGUI _phraseComponent;
#pragma warning restore CS0649

    private enum CountMode
    {
        NONE,
        SIGN,
        PAUSE,
    }
    
    private Dictionary<char, String> morse = new Dictionary<char, String>()
    {
        {'A' , ".-"},
        {'B' , "-..."},
        {'C' , "-.-."},
        {'D' , "-.."},
        {'E' , "."},
        {'F' , "..-."},
        {'G' , "--."},
        {'H' , "...."},
        {'I' , ".."},
        {'J' , ".---"},
        {'K' , "-.-"},
        {'L' , ".-.."},
        {'M' , "--"},
        {'N' , "-."},
        {'O' , "---"},
        {'P' , ".--."},
        {'Q' , "--.-"},
        {'R' , ".-."},
        {'S' , "..."},
        {'T' , "-"},
        {'U' , "..-"},
        {'V' , "...-"},
        {'W' , ".--"},
        {'X' , "-..-"},
        {'Y' , "-.--"},
        {'Z' , "--.."},
        {'0' , "-----"},
        {'1' , ".----"},
        {'2' , "..---"},
        {'3' , "...--"},
        {'4' , "....-"},
        {'5' , "....."},
        {'6' , "-...."},
        {'7' , "--..."},
        {'8' , "---.."},
        {'9' , "----."},
    };
    
    private float _signTimer;
    private float _pauseTimer;
    
    private float _dotThreshold = 0.2f;
    private float _lineThreshold = 0.5f;
    private float _letterPause = 0.2f;
    private float _wordPause = 0.5f;

    private string _outputPhrase;

    private CountMode _countMode;
    
    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        _outputPhrase = String.Empty;
        _countMode = CountMode.NONE;
        _signTimer = _pauseTimer = 0.0f;
        Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCounting();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCounting();
        }

        switch (_countMode)
        {
            case CountMode.NONE:
                return;
            case CountMode.SIGN:
                _signTimer += Time.deltaTime;
                break;
            case CountMode.PAUSE:
                _pauseTimer += Time.deltaTime;
                break;
        }
    }

    private void StartCounting()
    {
        _countMode = CountMode.SIGN;
        _signTimer = 0.0f;
        _soundGenerator.StartBeep();

        ParsePauseTimer();
    }

    private void StopCounting()
    {
        _countMode = CountMode.PAUSE;
        _pauseTimer = 0.0f;
        _soundGenerator.StopBeep();
        
        ParseSignTimer();
    }

    private void ParsePauseTimer()
    {
        if (_pauseTimer >= _letterPause && _pauseTimer <= _wordPause)
        {
            AddSign(" ");
        }
        else if (_pauseTimer > _wordPause)
        {
            AddSign(" / ");
        }
    }

    private void ParseSignTimer()
    {
        if (_signTimer <= _dotThreshold)
        {
            AddSign(".");
        }
        else if (_signTimer <= _lineThreshold)
        {
            AddSign("-");
        }
    }

    private void AddSign(string sign)
    {
        _outputPhrase += sign;
        Refresh();
    }

    private void Refresh()
    {
        _phraseComponent.text = _outputPhrase;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MorseDecoder : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private TextMeshProUGUI _morsePhraseView;
    [SerializeField] private TextMeshProUGUI _translatedPhraseView;
#pragma warning restore CS0649

    private enum CountMode
    {
        NONE,
        SIGN,
        PAUSE,
    }

    private readonly Dictionary<String, char> _morseLetters = new Dictionary<String, char>
    {
        {".-", 'A'},
        {"-...", 'B'},
        {"-.-.", 'C'},
        {"-..", 'D'},
        {".", 'E'},
        {"..-.", 'F'},
        {"--.", 'G'},
        {"....", 'H'},
        {"..", 'I'},
        {".---", 'J'},
        {"-.-", 'K'},
        {".-..", 'L'},
        {"--", 'M'},
        {"-.", 'N'},
        {"---", 'O'},
        {".--.", 'P'},
        {"--.-", 'Q'},
        {".-.", 'R'},
        {"...", 'S'},
        {"-", 'T'},
        {"..-", 'U'},
        {"...-", 'V'},
        {".--", 'W'},
        {"-..-", 'X'},
        {"-.--", 'Y'},
        {"--..", 'Z'},
    };

    private float _signTimer;
    private float _pauseTimer;

    private float _dotTime = 0.2f;
    private float _pauseTime = 0.3f;

    private string _currentMorsePhrase;
    private string _currentTranslatedPhrase;

    private CountMode _countMode;

    private void Start()
    {
        Reset();
    }

    public string GetCurrentCommand()
    {
        return _currentTranslatedPhrase.Trim();
    }

    public void Reset()
    {
        _countMode = CountMode.NONE;

        _currentMorsePhrase = String.Empty;
        _currentTranslatedPhrase = String.Empty;

        _signTimer = 0.0f;
        _pauseTimer = 0.0f;

        Refresh();
    }

    private void Update()
    {
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

    public void StartCounting()
    {
        _countMode = CountMode.SIGN;
        _signTimer = 0.0f;

        ParsePauseTimer();
    }

    public void StopCounting()
    {
        _countMode = CountMode.PAUSE;
        _pauseTimer = 0.0f;

        ParseSignTimer();
    }

    public string GetCodeForCommand(string command)
    {
        string code = string.Empty;

        foreach(char c in command)
        {
            KeyValuePair<string,char> pair = _morseLetters.FirstOrDefault(x => x.Value == c);
            code += $"{pair.Key}  ";
        }

        return code.Remove(code.Length - 2, 2);
    }

    private void ParseSignTimer()
    {
        AddSign(_signTimer <= _dotTime ? "." : "-");
    }

    private void ParsePauseTimer()
    {
        if (_pauseTimer >= _pauseTime)
        {
            AddSign(" ");
        }
    }

    private void AddSign(string sign)
    {
        _currentMorsePhrase += sign;
        TranslatePhrase(_currentMorsePhrase);
        Refresh();
    }

    private void TranslatePhrase(string phrase)
    {
        string translatedPhrase = String.Empty;
        string[] split = phrase.Split(' ');

        foreach (string morseLetter in split)
        {
            if (_morseLetters.TryGetValue(morseLetter, out char letter))
            {
                translatedPhrase += letter;
            }
        }

        _currentTranslatedPhrase = translatedPhrase;
    }

    private void Refresh()
    {
        _morsePhraseView.text = TranslateToCustomFont(_currentMorsePhrase);
        _translatedPhraseView.text = _currentTranslatedPhrase;
    }

    private string TranslateToCustomFont(string phrase)
    {
        string value = string.Empty;

        foreach (char c in phrase)
        {
            if (c == '.')
            {
                value += "<sprite=\"Code\" index=0>";
            }
            else if (c == '-')
            {
                value += "<sprite=\"Code\" index=1>";
            }
            else if (c == ' ')
            {
                value += "  ";
            }
        }

        return value;
    }
}
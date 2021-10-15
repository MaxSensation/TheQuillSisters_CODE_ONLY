// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.ScriptableObjectVariables;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] 
    private ScriptObjVar<bool> isOn = default;
    [SerializeField] 
    private bool hasWordDelay = default;
    [SerializeField] 
    private float delayBetweenEachWord = default;
    [SerializeField] 
    private float paddingTop = default;
    [SerializeField] 
    public float paddingBottom = default;
    [SerializeField] 
    public float paddingLeft = default;
    [SerializeField] 
    public float paddingRight = default;
    
    public static Action<string> onTextChanged;
    public static Action onHide;
    public static Action onShow;

    private AudioSource _audioSource;
    private GameObject _background;
    private StringTableEntry _currentSentenceEntry;
    private int _currentSentenceNr;
    private float _currentTime;
    private int _currentWordNr;
    private PlayableDirector _director;
    private Image _image;
    private bool _inDialog;
    private bool _localChange;
    private List<StringTableEntry> _sentences;
    private SignalReceiver _signalReceiver;
    private string[] _splittedUpSentence;
    private string _stringCollection;
    private StringTable _stringTable;
    private TextMeshProUGUI _textMeshPro;

    private void Reset()
    {
        _currentSentenceNr = 0;
        _currentWordNr = 0;
        _inDialog = false;
        _textMeshPro.SetText("");
        onTextChanged?.Invoke(_textMeshPro.text);
    }


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _signalReceiver = GetComponent<SignalReceiver>();
        _director = GetComponent<PlayableDirector>();
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        _textMeshPro.SetText("");
        onTextChanged?.Invoke(_textMeshPro.text);
        SubtitlesTrigger.OnDialog += OnDialog;
        SubtitlesTrigger.OnStopped += OnStopped;
        HideText();
    }

    private void Update()
    {
        if (_inDialog)
        {
            UpdateSubtitles();
        }
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocalChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocalChange;
    }

    private void OnStopped()
    {
        _director.Stop();
        HideText();
    }

    private async void OnDialog(string colName, PlayableAsset playable)
    {
        foreach (var output in playable.outputs)
        {
            if (output.outputTargetType == typeof(SignalReceiver))
            {
                _director.SetGenericBinding(output.sourceObject, _signalReceiver);
            }
            else if (output.outputTargetType == typeof(AudioSource))
            {
                _director.SetGenericBinding(output.sourceObject, _audioSource);
            }
        }
        _director.Stop();
        Reset();
        _stringCollection = colName;
        await LoadStrings();
        _inDialog = true;
        _textMeshPro.SetText("");
        if (isOn)
        {
            ShowText();
        }
        else
        {
            HideText();
        }
        GetNewStringList();
        _director.playableAsset = playable;
        _director.Play();
        if (!hasWordDelay)
        {
            AddSentence(_currentSentenceEntry.Value);
        }
    }

    private void GetNewStringList()
    {
        _sentences = _stringTable.Values.ToList();
        _currentSentenceEntry = _sentences[_currentSentenceNr];
        SplitUpSentence();
    }

    private void UpdateSubtitles()
    {
        if (hasWordDelay && _currentTime >= delayBetweenEachWord && _currentWordNr < _splittedUpSentence.Length)
        {
            AddWord(_splittedUpSentence[_currentWordNr]);
            _currentTime = 0;
        }
        if (_currentSentenceNr >= _sentences.Count)
        {
            Complete();
        }
        _currentTime += Time.deltaTime;
    }

    private void AddSentence(string value)
    {
        _textMeshPro.SetText(value);
        onTextChanged?.Invoke(_textMeshPro.text);
    }

    private void Complete()
    {
        _currentSentenceNr = 0;
        _currentWordNr = 0;
        _inDialog = false;
        _textMeshPro.SetText("");
        onTextChanged?.Invoke(_textMeshPro.text);
    }

    private void AddWord(string word)
    {
        _currentWordNr++;
        _textMeshPro.SetText(_textMeshPro.text + " " + word);
        onTextChanged?.Invoke(_textMeshPro.text);
    }

    private void SplitUpSentence()
    {
        _splittedUpSentence = _currentSentenceEntry.Value.Split(' ');
    }

    public void NextSentence()
    {
        _currentSentenceNr++;
        if (_currentSentenceNr < _sentences.Count)
        {
            _textMeshPro.SetText("");
            onTextChanged?.Invoke(_textMeshPro.text);
            if (isOn)
                ShowText();
            else
                HideText();
            _currentSentenceEntry = _sentences[_currentSentenceNr];
            SplitUpSentence();
            _currentWordNr = 0;
        }

        if (!hasWordDelay)
        {
            AddSentence(_currentSentenceEntry.Value);
        }
    }

    public void HideText()
    {
        onHide?.Invoke();
        _textMeshPro.enabled = false;
    }

    private void ShowText()
    {
        onShow?.Invoke();
        _textMeshPro.enabled = true;
    }

    private void OnLocalChange(Locale obj)
    {
        if (_stringCollection != null)
        {
            UpdateLang();
        }
    }

    private async Task LoadStrings()
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(_stringCollection);
        await loadingOperation.Task;
        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            _stringTable = loadingOperation.Result;
        }
    }

    private async void UpdateLang()
    {
        await LoadStrings();
        GetNewStringList();
        _textMeshPro.SetText("");
        onTextChanged?.Invoke(_textMeshPro.text);
    }
}
using SoulSmithEmotions;

using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using System.Collections.ObjectModel;

public class UnitTemplate 
{

    private ReadOnlyDictionary<StatType, int> _statsList;
    private ReadOnlyCollection<string> _moveSetString;
    private EmotionTag _emotion;
    private string _spriteName;
    private int _timeOnBoard;
    private int _maxMoveCount = 3;
    private string _friendlyName;

    public UnitTemplate(ReadOnlyDictionary<StatType, int> statsList,
                        ReadOnlyCollection<string> moveSetString,
                        EmotionTag emotion,
                        int timeOnBoard,
                        string spriteName,
                        string friendlyName)
    {
        _statsList = statsList;
        _moveSetString = moveSetString;
        _emotion = emotion;
        _spriteName = spriteName;
        _friendlyName = friendlyName;
        _timeOnBoard = timeOnBoard;
    }

    public ReadOnlyDictionary<StatType, int> StatsList { get { return _statsList; } }
    public ReadOnlyCollection<string> MoveSetString { get {  return _moveSetString; } }
    public EmotionTag Emotion { get { return _emotion; } }
    public string SpriteName { get { return _spriteName; } }
    public int TimeOnBoard { get { return _timeOnBoard; } }
    public string FriendlyName { get { return _friendlyName; } }
    public int MaxMoveCount {  get { return _maxMoveCount; } }
}

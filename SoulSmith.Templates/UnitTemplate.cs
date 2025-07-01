using SoulSmith.Asset;
using SoulSmith.Collections;
using SoulSmith.UnitStats;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SoulSmith.Templates;
[JsonConverter(typeof(UnitTemplateJsonConverter))]
public class UnitTemplate : IAsset
{

    private ReadOnlyDictionary<StatType, int> _statsList;
    private IReadOnlySoulSmithWeightedList<string> _moveSetString;
    private EmotionTag.EmotionTag _emotion;
    private string _spriteName;
    private int _timeOnBoard;
    private int _maxMoveCount = 3;
    private string _friendlyName;

    public UnitTemplate(IDictionary<StatType, int> statsList,
                        IReadOnlySoulSmithWeightedList<string> moveSetString,
                        EmotionTag.EmotionTag emotion,
                        int timeOnBoard,
                        string spriteName,
                        string friendlyName)
    {
        _statsList = new(statsList);
        _moveSetString = moveSetString;
        _emotion = emotion;
        _spriteName = spriteName;
        _friendlyName = friendlyName;
        _timeOnBoard = timeOnBoard;
    }

    public void Dispose() { }

    public ReadOnlyDictionary<StatType, int> StatsList { get { return _statsList; } }
    public IReadOnlySoulSmithWeightedList<string> MoveSetWeightedList { get {  return _moveSetString; } }
    public EmotionTag.EmotionTag Emotion { get { return _emotion; } }
    public string SpriteName { get { return _spriteName; } }
    public int TimeOnBoard { get { return _timeOnBoard; } }
    public string FriendlyName { get { return _friendlyName; } }
    public int MaxMoveCount {  get { return _maxMoveCount; } }
}

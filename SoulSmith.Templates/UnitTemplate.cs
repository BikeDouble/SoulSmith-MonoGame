using SoulSmith.Asset;
using SoulSmith.Collections;
using SoulSmith.UnitStats;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SoulSmith.Templates;
[JsonConverter(typeof(UnitTemplateJsonConverter))]
public class UnitTemplate : IDisposable
{

    private ReadOnlyDictionary<StatType, int> _statsList;
    private IReadOnlySoulSmithWeightedList<string> _moveSetString;
    private EmotionTags.EmotionTag _emotion;
    private string _spriteName;
    private string _spriteType;
    private int _timeOnBoard;
    private int _maxMoveCount = 3;
    private string _friendlyName;
    private float _spriteSizeMod = 1;

    public UnitTemplate(IDictionary<StatType, int> statsList,
                        IReadOnlySoulSmithWeightedList<string> moveSetString,
                        EmotionTags.EmotionTag emotion,
                        int timeOnBoard,
                        string spriteName,
                        string spriteType,
                        float spriteSizeMod,
                        string friendlyName)
    {
        _statsList = new(statsList);
        _moveSetString = moveSetString;
        _emotion = emotion;
        _spriteName = spriteName;
        _spriteType = spriteType;
        _friendlyName = friendlyName;
        _spriteSizeMod = spriteSizeMod;
        _timeOnBoard = timeOnBoard;
    }

    public void Dispose() { }

    public UnitTemplate GetModdedCopy(IDictionary<StatType, int> statslist, int? timeOnBoard = null)
    {
        var newStats = new Dictionary<StatType, int>(statslist);
        
        foreach (var stat in this.StatsList)
        {
            if (!newStats.ContainsKey(stat.Key))
            {
                newStats[stat.Key] = stat.Value;
            }
        }

        return new UnitTemplate(
            newStats,
            this.MoveSetWeightedList,
            this.Emotion,
            timeOnBoard ?? this.TimeOnBoard,
            this.SpriteName,
            this.SpriteType,
            this.SpriteSizeMod,
            this.FriendlyName
        );
    }

    public ReadOnlyDictionary<StatType, int> StatsList { get { return _statsList; } }
    public IReadOnlySoulSmithWeightedList<string> MoveSetWeightedList { get {  return _moveSetString; } }
    public EmotionTags.EmotionTag Emotion { get { return _emotion; } }
    public string SpriteName { get { return _spriteName; } }
    public string SpriteType { get { return _spriteType; } }
    public float SpriteSizeMod { get { return _spriteSizeMod; } }
    public int TimeOnBoard { get { return _timeOnBoard; } }
    public string FriendlyName { get { return _friendlyName; } }
    public int MaxMoveCount {  get { return _maxMoveCount; } }
}

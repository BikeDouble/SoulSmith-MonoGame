
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.Json;

public class DeserializedUnitTemplate
{
    public string FriendlyName { get; set; }
    public string SpriteName { get; set; }
    public int MaxHealth { get; set; }
    public int CurHealth { get; set; } = -1;
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int DecayRate { get; set; } = SoulSmithStats.StatConstants.STANDARDDECAYRATE;
    public int CurDecay { get; set; } = 0;
    public string[] MoveSet { get; set; }
    public int Emotion { get; set; }
    public int MaxMoveCount { get; set; } = 3;
    public int TimeOnBoard { get; set; } = -1;
}

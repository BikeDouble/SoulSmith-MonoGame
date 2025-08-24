using System.Text.Json.Serialization;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Effects
{
    [JsonConverter(typeof(IEffectJsonConverter))]
    public interface IEffect : IDisposable, IReadOnlyEffect
    {
        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null);
        public Visualization.EffectVisualization CreateVisualization();
        public bool HasVisualization { get; }
        public float AdditionalDelay { get; }
    }
}

using System.Text.Json.Serialization;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Effects
{
    [JsonConverter(typeof(IEffectJsonConverter))]
    public interface IEffect : IDisposable, IReadOnlyEffect
    {
        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null);
        public Visualization.EffectVisualization CreateVisualization();
        public float AdditionalDelay { get; }
    }
}

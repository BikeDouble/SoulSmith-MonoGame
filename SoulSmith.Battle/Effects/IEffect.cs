using System.Text.Json.Serialization;
using System.Text.Json;

namespace SoulSmith.Battle.Effects
{
    [JsonConverter(typeof(IEffectJsonConverter))]
    public interface IEffect : IDisposable
    {
        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null);
        public Visualization.EffectVisualization CreateVisualization();
    }
}

using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effects.Payloads
{
    public interface IMagnitudeModifiablePayload
    {
        ReadOnlyCollection<MagnitudeModifier> MagnitudeModifiers { get; }
        void AddMagnitudeModifier(MagnitudeModifier magnitudeModifier);
        int ModifiedAmount { get; }
    }
}

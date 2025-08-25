using SoulSmith.Battle.Effects.Results;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class MagnitudeModifiablePayloadBase : PayloadBase
    {
        public MagnitudeModifiablePayloadBase(IReadOnlyUnit sender, IReadOnlyUnit target, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        { }

        public ReadOnlyCollection<MagnitudeModifier> MagnitudeModifiers { get { return MagnitudeModifiersInternal.AsReadOnly(); } }

        protected readonly List<MagnitudeModifier> MagnitudeModifiersInternal = new List<MagnitudeModifier>();

        public void AddMagnitudeModifier(MagnitudeModifier magnitudeModifier)
        {
            MagnitudeModifiersInternal.Add(magnitudeModifier);
        }

        public override List<IEffectOriginator> GetModifyingObjects()
        {
            List<IEffectOriginator> modifyingObjects = base.GetModifyingObjects();

            foreach (MagnitudeModifier magnitudeModifier in MagnitudeModifiers)
            {
                if (magnitudeModifier.Originator != null && !modifyingObjects.Contains(magnitudeModifier.Originator))
                {
                    modifyingObjects.Add(magnitudeModifier.Originator);
                }
            }

            return modifyingObjects;
        }
    }
}

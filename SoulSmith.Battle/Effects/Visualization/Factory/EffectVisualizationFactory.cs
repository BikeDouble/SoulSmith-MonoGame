using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(EffectVisualizationFactoryJsonConverter))]
    public class EffectVisualizationFactory : IDisposable
    {
        private float _lifespan;
        private float _effectActivationTimer;
        private float _delay;

        public EffectVisualizationFactory(
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) 
        {
            _lifespan = lifespan;
            _effectActivationTimer = effectActivationTimer;
            _delay = delay;
        }

        public virtual EffectVisualization CreateVisualization()
        {
            return new EffectVisualization(_lifespan, _effectActivationTimer, _delay);
        }

        public float Lifespan { get { return _lifespan; } }
        public float EffectActivationTimer { get { return _effectActivationTimer; } }
        public float Delay { get { return _delay; } }

        public void Dispose()
        {
            // Implement any necessary cleanup logic here
        }
    }
}

using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using System;

namespace SoulSmith.Battle.Effects.Visualization
{
    public class ShockwaveVisualization : EffectVisualization
    {
        public readonly static Vector2 STARTSIZE = new Vector2(0, 0);
        public readonly static Vector2 FADESTARTSIZE = new Vector2((Window.DISTANCEBETWEENBACKUNITSONEITHERTEAM - 150) * 2);


        // Children
        private List<(float TimeElapsed, CanvasObject Object)> _particles;

        private readonly string _particleResourceKey;
        private IReadOnlyUnit _sender;
        private readonly int _particleCount;
        private readonly int _particleToExecute;
        private readonly float _delayBetweenParticles;
        private readonly float _fadeTime;

        // Derived constants
        private readonly float _timePerParticle;
        private readonly Vector2 _endSize; 

        public ShockwaveVisualization(string particleResourceKey,
            int particleCount,
            int particleToExecute,
            float delayBetweenParticles,
            float effectActivationTimer,
            float fadeTime,
            Color color,
            float delay = 0f) : base(CalculateLifespan(particleCount, delayBetweenParticles, effectActivationTimer, particleToExecute, fadeTime), effectActivationTimer, delay)
        {
            _particleCount = particleCount;
            _particleToExecute = particleToExecute;
            _particleResourceKey = particleResourceKey;
            _delayBetweenParticles = delayBetweenParticles;
            _fadeTime = fadeTime;
            _endSize = FADESTARTSIZE * (1 + _fadeTime / TotalEffectActivationTimer);
            _timePerParticle = CalculateTimePerParticle(_delayBetweenParticles, TotalEffectActivationTimer, _particleToExecute);
            this.SetColor(color);

            thisLastWidth = this.GetGlobalPosition().Width;
           
        }

        private static float CalculateTimePerParticle(float delayBetweenParticles, float effectActivationTimer, int particleToExecute)
        {
            return effectActivationTimer - (delayBetweenParticles * (particleToExecute - 1));
        }

        public static float CalculateLifespan(int particleCount, float delayBetweenParticles, float effectActivationTimer, int particleToExecute, float fadeTime)
        {
            float timePerParticle = CalculateTimePerParticle(delayBetweenParticles, effectActivationTimer, particleToExecute);
            return (particleCount - 1) * delayBetweenParticles + timePerParticle + fadeTime;
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0, bool mirrorSprites = false)
        {
            base.BeginVisualization(sender, target, delay, mirrorSprites);

            _particles = new List<(float, CanvasObject)>();
            _sender = sender;
            BeginNewParticle();
        }

        private void BeginNewParticle()
        {
            IDrawableResource particleResource = DrawHelpers.GetDrawableResourceInstance(_particleResourceKey);
            CanvasObject particle = new CanvasObject(null, particleResource);
            AddChild(particle);
            particle.ScaleToSetSize(STARTSIZE);

            Vector2 startPoint;
            IReadOnlyUnitSprite senderSprite = _sender.ReadOnlySprite;
            startPoint = senderSprite.GetGlobalPosition().Coordinates;
            particle.SetCoordinates(startPoint);

            if (MirrorSprites)
            {
                particle.Scale(new Vector2(-1, 1));
            }

            AddChild(particle);

            _particles.Add((0f, particle));
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            if (_particles.Count < _particleCount)
            {
                double timeToSpawnNextParticle = _particles.Count * _delayBetweenParticles;
                if (ElapsedLifespan > timeToSpawnNextParticle) BeginNewParticle();
            }

            for (int i = 0; i < _particles.Count(); i++)
            { 
                _particles[i] = ((float)(_particles[i].TimeElapsed + delta), _particles[i].Object);
                var particle = _particles[i];
                ProcessParticle(particle.TimeElapsed, particle.Object);
            }
        }

        private float thisLastWidth;

        private void ProcessParticle(float timeElapsedInParticle, CanvasObject particle)
        {

            float growProgress = (float)(timeElapsedInParticle / _timePerParticle);

            Vector2 curSize = (_endSize - STARTSIZE) * growProgress + STARTSIZE;

            particle.ScaleToSetSize(curSize);

            float fadeProgress;

            if (timeElapsedInParticle > _timePerParticle)
            {
                float timeSpentFading = timeElapsedInParticle - _timePerParticle;
                fadeProgress = timeSpentFading / _fadeTime;
                if (fadeProgress > 1)
                {
                    fadeProgress = 1;
                    particle.Hide();
                }
            }
            else fadeProgress = 0;

            float alphaMult = 1 - fadeProgress;

            Color curColor = new Color(255, 255, 255, (int)(255 * alphaMult));
            particle.SetColor(curColor);
        }
    }
}
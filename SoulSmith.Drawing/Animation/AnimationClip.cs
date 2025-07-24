using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing.Textures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Animation
{
    public class AnimationClip
    {
        private List<int> _transitionFrameIndices = null;
        private List<AnimationFrame> _frames;

        public AnimationClip(IEnumerable<AnimationFrame> sortedFrames, IEnumerable<int> transitionFrameIndices = null)
        {
            _frames = new List<AnimationFrame>(sortedFrames);
            _transitionFrameIndices = new List<int>(transitionFrameIndices);
        }

        public AnimationClip(IEnumerable<AnimationFrame> unsortedFrames, string clipDataName, IEnumerable<int> transitionFrameIndices = null)
        {
            _frames = SortFrames(unsortedFrames, clipDataName);
            _transitionFrameIndices = new List<int>(transitionFrameIndices);
        }

        /// <summary>
        /// Sorts frames of animation clip. Assumes all frames within clip are consecutive and in order in
        /// </summary>
        /// <param name="unsortedFrames"></param>
        /// <param name="clipDataName"></param>
        /// <returns></returns>
        public static List<AnimationFrame> SortFrames(IEnumerable<AnimationFrame> unsortedFrames, string clipDataName)
        {
            List<AnimationFrame> sortedFrames = new List<AnimationFrame>();

            bool foundFrames = false;

            foreach (AnimationFrame frame in unsortedFrames)
            {

                string parsedFrameName = ParseFrameDataName(frame.DataName).Name;

                if (string.Compare(parsedFrameName, clipDataName, true) == 0)
                {
                    sortedFrames.Add(frame);
                    foundFrames = true;
                }
                else if (foundFrames) // Break if we have found frames with matching name, but not anymore. Assumes frames in clip are consecutive
                {
                    break;
                }
            }

            return sortedFrames;
        }

        public static (string Name, int Index) ParseFrameDataName(string frameDataName)
        {
            int underscoreIndex = frameDataName.LastIndexOf('_');

            string name = frameDataName.Substring(0, underscoreIndex);
            string frameIdxString = frameDataName.Substring(underscoreIndex + 1);

            int frameIdx = int.Parse(frameIdxString);

            return (name, frameIdx);
        }

        public void DrawFrame(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch, IAssetWrapper<Texture2DResource> texture, double timeInClip, double animationSpeed = 1d, OriginPlacement originPlacement = OriginPlacement.Center)
        {
            AnimationFrame activeFrame = GetActiveFrame(timeInClip, animationSpeed);
            activeFrame.DrawFrame(position, color, spriteBatch, texture, originPlacement);
        }

        public bool IsTransitionReady(double timeInClip, double animationSpeed = 1d)
        {
            if (_transitionFrameIndices == null) return true;

            if (_transitionFrameIndices.Count == 0) return true;

            int activeFrameIdx = GetActiveFrameIdx(timeInClip, animationSpeed);

            return _transitionFrameIndices.Contains(activeFrameIdx);
        }

        private AnimationFrame GetActiveFrame(double timeInClip, double animationSpeed = 1d)
        {
            return _frames[GetActiveFrameIdx(timeInClip, animationSpeed)];
        }

        private int GetActiveFrameIdx(double timeInClip, double animationSpeed = 1d)
        {
            if (animationSpeed <= 0) return 0;

            if (Animation.FRAMESPERSECOND <= 0) return 0;

            double speedAdjustedTime = timeInClip * animationSpeed;

            int nonModdedFrameIdx = (int)Math.Floor(speedAdjustedTime * Animation.FRAMESPERSECOND);

            int frameIdx = nonModdedFrameIdx % _frames.Count;

            return frameIdx;
        }

        public int Height { get { return _frames[0].FrameHeight; } }
        public int Width { get { return _frames[0].FrameWidth; } }
        public Vector2 Origin { get { return _frames[0].FrameOrigin; } }
    }
}

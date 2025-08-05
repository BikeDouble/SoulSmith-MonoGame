using Microsoft.Xna.Framework;
using SoulSmith.Core;
using SoulSmith.Drawing.Zoned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Object.Canvas
{
    public class ButtonObject_GrowOnHover : ButtonObject
    {
        private bool currentlyGrown = false;
        private bool disableGrow = false;

        public ButtonObject_GrowOnHover(
        ZonedDrawableResourceInstance idleResource,
        ZonedDrawableResourceInstance hoveredResource,
        Position position = null,
        Vector2? hoverScale = null) : base(
            idleResource,
            hoveredResource,
            position)
        {
            if (hoverScale.HasValue)
            {
                HoverScale = hoverScale.Value;
            }
        }

        public ButtonObject_GrowOnHover(
        string idleResource,
        string hoveredResource,
        Position position = null,
        Vector2? hoverScale = null) : base(
            idleResource,
            hoveredResource,
            position)
        {
            if (hoverScale.HasValue)
            {
                HoverScale = hoverScale.Value;
            }
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (!disableGrow)
            {
                Scale(HoverScale);
                currentlyGrown = true;
            }
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();

            if ((!disableGrow) && currentlyGrown)
            {
                Scale(UnhoverScale);
                currentlyGrown = false;
            }   
        }

        public void EnableGrow()
        {
            disableGrow = false;
        }

        public void DisableGrow()
        {
            disableGrow = true;

            if (currentlyGrown)
            {
                Scale(UnhoverScale);
                currentlyGrown = false;
            }
        }

        protected Vector2 HoverScale = Vector2.One;
        protected Vector2 UnhoverScale { get { return Vector2.One / HoverScale; } }
    }
}

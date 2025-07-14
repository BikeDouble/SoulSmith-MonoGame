using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Input;
using SoulSmith.Shapes;

namespace SoulSmith.Object
{
    public class SoulSmithObject : IReadOnlySoulSmithObject, ISoulSmithObject, IDrawPacketGenerator, IDisposable, IProcessable
    {
        private List<SoulSmithObject> _children;

        public SoulSmithObject()
        {
            _children = new List<SoulSmithObject>();
        }

        public SoulSmithObject(IEnumerable<SoulSmithObject> children)
        {
            _children = new List<SoulSmithObject>();
            AddMultipleChildren(children);
        }

        /// <summary>
        /// Processes object and input logic every frame.
        /// </summary>
        /// <param name="delta">How much time has passed in seconds since last process.</param>
        /// <param name="inputQueue">Queue for input packet queries.</param>
        public virtual void Process(double delta)
        {
            foreach (SoulSmithObject child in _children)
            {
                child.Process(delta);
            }
        }

        public void AddMultipleChildren(IEnumerable<SoulSmithObject> children)
        {
            if (children != null)
                foreach (SoulSmithObject child in children)
                    AddChild(child);
        }

        public virtual void AddChild(SoulSmithObject child)
        {
            if (child == null)
                return;

            if (_children.Contains(child))
            {
                return;
            }

            _children.Add(child);
            child.GetParentEventHandler += ReturnParent;
        }

        public virtual void RemoveChild(SoulSmithObject child)
        {
            if (_children.Contains(child))
            {
                _children.Remove(child);
                child.GetParentEventHandler -= ReturnParent;
            }
        }

        public event EventHandler<SoulSmithObjectGetParentEventArgs> GetParentEventHandler;

        public SoulSmithObject GetParent()
        {
            SoulSmithObjectGetParentEventArgs e = new SoulSmithObjectGetParentEventArgs();

            GetParentEventHandler?.Invoke(this, e);

            return e.Parent;
        }

        private void ReturnParent(object sender, SoulSmithObjectGetParentEventArgs e)
        {
            e.Parent = this;
        }

        public virtual void CollectDrawPackets(IReadOnlyPosition parentAbsolutePosition, Color color, IAddOnly<DrawPacket> renderQueue, Microsoft.Xna.Framework.Rectangle? scissorRect = null)
        {

        }

        public virtual void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, Position absolutePosition = null) //TODO investigate absolutePosition
        { //TODO make IInputPacketGenerator
            foreach (SoulSmithObject child in Children)
            {
                child.CollectInputPackets(parentAbsolutePosition, inputQueue);
            }
        }

        public virtual InputPacket CreateInputPacket(Func<InputPacketFuncInput, InputPacketFuncOutput> func, IReadOnlyPosition absPos = null, IZone clickZone = null, bool requestHover = false, int priority = 0)
        {
            InputPacket packet = new InputPacket(
                clickZone,
                func,
                priority,
                absPos,
                requestHover);

            return packet;
        }

        public virtual void Dispose()
        {
            foreach (var child in Children)
            {
                child.Dispose();
            }
        }

        public ReadOnlyCollection<SoulSmithObject> Children { get { return _children.AsReadOnly(); } }
        public int ChildCount { get { return _children.Count; } }
    }

    public class SoulSmithObjectGetParentEventArgs : EventArgs
    {
        public SoulSmithObject Parent;
    }
}
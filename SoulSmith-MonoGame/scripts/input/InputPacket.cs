using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InputPacketFunc = System.Func<InputPacketFuncInput, InputPacketFuncOutput>;
using SoulSmith.Drawing;

public enum InputType 
{
    MouseHover,
    MouseLeft,
    MouseRight
}

/// <summary>
/// Packet of input requested by object. Collected and processed each frame.
/// </summary>
public class InputPacket
{
    private IReadOnlyCanvasItem _boundingZone;
    private InputPacketFunc _func;
    private int _priority;
    private int _z;
    private bool _requestHover;
    private IReadOnlyCanvasPosition _position;
    private IReadOnlySoulSmithObject _sender;

    public InputPacket(
        IReadOnlyCanvasItem boundingZone,
        InputPacketFunc func,
        int priority,
        IReadOnlySoulSmithObject sender,
        IReadOnlyCanvasPosition position = null,
        bool requestHover = false)
    {
        _boundingZone = boundingZone;
        _func = func;
        _priority = priority;
        _requestHover = requestHover;
        _position = position;
        _sender = sender;

        if (_position != null)
        {
            _z = _position.Z;
        }
        else
        {
            _z = 0;
        }
    }

    public IReadOnlyCanvasItem BoundingZone { get {  return _boundingZone; } }
    public InputPacketFunc Func { get { return _func; } }
    public int Z { get { return _z; } }
    public int Priority { get { return _priority; } }  
    public bool RequestHover { get { return _requestHover; } }
    public IReadOnlyCanvasPosition Position { get { return _position; } }
    public IReadOnlySoulSmithObject Sender { get { return _sender; } }
}

public class InputPacketFuncInput
{
    public IReadOnlyList<InputType> Inputs;
}

public class InputPacketFuncOutput
{
    public IEnumerable<InputType> ConsumedInputs;
}


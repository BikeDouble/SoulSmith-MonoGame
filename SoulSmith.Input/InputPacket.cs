using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Shapes;

namespace SoulSmith.Input;
using InputPacketFunc = System.Func<InputPacketFuncInput, InputPacketFuncOutput>;

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
    private IZone _clickZone;
    private InputPacketFunc _func;
    private int _priority;
    private int _z;
    private bool _requestHover;
    private IReadOnlyPosition _position;

    public InputPacket(
        IZone clickZone,
        InputPacketFunc func,
        int priority,
        IReadOnlyPosition position = null,
        bool requestHover = false)
    {
        _clickZone = clickZone;
        _func = func;
        _priority = priority;
        _requestHover = requestHover;
        _position = position;

        if (_position != null)
        {
            _z = _position.Z;
        }
        else
        {
            _z = 0;
        }
    }

    public IZone Zone { get {  return _clickZone; } }
    public InputPacketFunc Func { get { return _func; } }
    public int Z { get { return _z; } }
    public int Priority { get { return _priority; } }  
    public bool RequestHover { get { return _requestHover; } }
    public IReadOnlyPosition Position { get { return _position; } }
}

public class InputPacketFuncInput
{
    public IReadOnlyList<InputType> Inputs;
}

public class InputPacketFuncOutput
{
    public IEnumerable<InputType> ConsumedInputs;
}


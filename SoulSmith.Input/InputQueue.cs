using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Core;

namespace SoulSmith.Input;
using InputPacketFunc = System.Func<InputPacketFuncArgs, InputPacketFuncOutput>;
public class InputQueue : IAddOnly<InputPacket>
{
    List<InputPacket> packets = new List<InputPacket>();
    Vector2 mousePos = Vector2.Zero;

    public void Process(List<InputType> capturedInputs, List<InputType> consumedInputs)
    {
        if (packets.Count == 0) return;

        if (capturedInputs.Count == 0) return;

        mousePos = MouseFunctions.GetPosition();

        packets = packets.OrderByDescending((a) => a.Priority).ThenByDescending((a) => a.Z).ToList();

        foreach (InputPacket packet in packets)
        {
            ProcessPacket(packet, capturedInputs, consumedInputs);
        }
    }

    private void ProcessPacket(InputPacket packet, IReadOnlyList<InputType> availableInputs, List<InputType> consumedInputs)
    {
        InputPacketFunc func = packet.Func;

        if (func == null) { return; }

        InputPacketFuncArgs funcInput = new();
        funcInput.CapturedInputs = new List<InputType>(availableInputs);
        funcInput.ConsumedInputs = new List<InputType>(consumedInputs);

        if (availableInputs.Contains(InputType.MouseHover) && 
            packet.RequestHover &&
            packet.Zone != null && 
            packet.Position != null &&
            packet.Zone.ContainsGlobal(mousePos, packet.Position))
        {
            funcInput.IsMouseHovering = true;    
        } 
        else
        {
            funcInput.IsMouseHovering = false;
        }
            
        InputPacketFuncOutput funcOutput = func(funcInput);

        if (funcOutput != null)
        {
            if (funcOutput.NewlyConsumedInputs != null)
            {
                foreach (InputType consumedInput in funcOutput.NewlyConsumedInputs)
                {
                    if (!consumedInputs.Contains(consumedInput))
                    {
                        consumedInputs.Add(consumedInput);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        packets.Clear();
    }

    public void Add(InputPacket packet)
    {
        packets.Add(packet);
    }
}


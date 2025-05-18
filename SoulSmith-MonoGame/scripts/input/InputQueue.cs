using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Core;
using InputPacketFunc = System.Func<InputPacketFuncInput, InputPacketFuncOutput>;

public class InputQueue : IAddOnly<InputPacket>
{
    List<InputPacket> packets = new List<InputPacket>();
    Vector2 mousePos = Vector2.Zero;

    public void Process(List<InputType> availableInputs)
    {
        if (packets.Count == 0) return;

        if (availableInputs.Count == 0) return;

        mousePos = MouseFunctions.GetPosition();

        packets.OrderBy((a) => -a.Priority).ThenBy((a) => -a.Z);

        foreach (InputPacket packet in packets)
        {
            ProcessPacket(packet, availableInputs);
        }
    }

    private void ProcessPacket(InputPacket packet, List<InputType> availableInputs)
    {
        InputPacketFunc func = packet.Func;

        if (func == null) { return; }

        InputPacketFuncInput funcInput = new();
        if (availableInputs.Contains(InputType.MouseHover) && packet.RequestHover)
        {
            if (packet.BoundingZone != null && 
                //packet.Position != null &&
                packet.BoundingZone.ContainsPointRelative(mousePos - packet.Position.Coordinates))
            {
                funcInput.Inputs = availableInputs;
            }
            else
            {
                List<InputType> listWithoutHover = new List<InputType>(availableInputs);
                listWithoutHover.Remove(InputType.MouseHover);
                funcInput.Inputs = listWithoutHover;
            }
        } 
        else
        {
            funcInput.Inputs = availableInputs;
        }
            
        InputPacketFuncOutput funcOutput = func(funcInput);

        if (funcOutput != null)
        {
            if (funcOutput.ConsumedInputs != null)
            {
                foreach (InputType consumedInput in funcOutput.ConsumedInputs)
                {
                    availableInputs.Remove(consumedInput);
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


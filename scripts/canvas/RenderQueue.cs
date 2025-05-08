using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class RenderQueue
{
    List<DrawPacket> packets = new List<DrawPacket>();

    public void Draw(SpriteBatch spriteBatch)
    {
        if (packets.Count == 0) return;

        packets.Sort((a, b) => a.Z.CompareTo(b.Z));

        foreach (DrawPacket packet in packets) 
        {
            packet.Draw(spriteBatch);
        }
    }

    public void Clear()
    {
        packets.Clear();
    }

    public void Add(DrawPacket packet)
    {
        packets.Add(packet);
    }
}


/*
namespace Assistant
{
    using System;

    public sealed class WorldItem : Packet
    {
        public WorldItem(Item item) : base(0x1a)
        {
            base.EnsureCapacity(20);
            uint serial = (uint) item.Serial;
            ushort itemID = (ushort) item.ItemID;
            ushort amount = item.Amount;
            int x = item.Position.X;
            int y = item.Position.Y;
            ushort hue = item.Hue;
            byte packetFlags = item.GetPacketFlags();
            byte direction = item.Direction;
            if (amount != 0)
            {
                serial |= 0x80000000;
            }
            else
            {
                serial &= 0x7fffffff;
            }
            base.Write(serial);
            base.Write((ushort) (itemID & 0x7fff));
            if (amount != 0)
            {
                base.Write(amount);
            }
            x &= 0x7fff;
            if (direction != 0)
            {
                x |= 0x8000;
            }
            base.Write((ushort) x);
            y &= 0x3fff;
            if (hue != 0)
            {
                y |= 0x8000;
            }
            if (packetFlags != 0)
            {
                y |= 0x4000;
            }
            base.Write((ushort) y);
            if (direction != 0)
            {
                base.Write(direction);
            }
            base.Write((sbyte) item.Position.Z);
            if (hue != 0)
            {
                base.Write(hue);
            }
            if (packetFlags != 0)
            {
                base.Write(packetFlags);
            }
        }
    }
}
*/
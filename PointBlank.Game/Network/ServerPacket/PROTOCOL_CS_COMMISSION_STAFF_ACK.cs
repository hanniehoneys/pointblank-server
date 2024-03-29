﻿using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_COMMISSION_STAFF_ACK : SendPacket
  {
    private uint result;

    public PROTOCOL_CS_COMMISSION_STAFF_ACK(uint result)
    {
      this.result = result;
    }

    public override void write()
    {
      this.writeH((short) 1861);
      this.writeD(this.result);
    }
  }
}

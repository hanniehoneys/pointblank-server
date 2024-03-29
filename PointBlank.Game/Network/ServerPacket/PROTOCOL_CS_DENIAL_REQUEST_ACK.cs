﻿using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_CS_DENIAL_REQUEST_ACK : SendPacket
  {
    private int result;

    public PROTOCOL_CS_DENIAL_REQUEST_ACK(int result)
    {
      this.result = result;
    }

    public override void write()
    {
      this.writeH((short) 1850);
      this.writeD(this.result);
    }
  }
}

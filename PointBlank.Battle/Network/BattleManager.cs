﻿using PointBlank.Battle.Data.Configs;
using System;
using System.Net;
using System.Net.Sockets;

namespace PointBlank.Battle.Network
{
  public class BattleManager
  {
    private static UdpClient UdpClient;

    public static void Connect()
    {
      try
      {
        UdpClient = new UdpClient();
        uint IOC_IN = 0x80000000;
        uint IOC_VENDOR = 0x18000000;
        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        UdpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        IPEndPoint LocalEP = new IPEndPoint(IPAddress.Parse(BattleConfig.hosIp), BattleConfig.hosPort);
        var UdpState = new UdpState(LocalEP, UdpClient);
        UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        UdpClient.Client.Bind(LocalEP);
        UdpClient.BeginReceive(AcceptCallback, UdpState);
        Logger.debug("Active Server. (" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + ")");
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString() + "\r\nAn error occurred while listing the Udp connections!!");
      }
    }

    private static void Read(BattleManager.UdpState state)
    {
      try
      {
        BattleManager.UdpClient.BeginReceive(new AsyncCallback(BattleManager.AcceptCallback), (object) state);
      }
      catch (Exception ex)
      {
        Logger.error(ex.ToString());
      }
    }

    private static void AcceptCallback(IAsyncResult ar)
    {
      if (!ar.IsCompleted)
        Logger.warning("Result is not completed.");
      ar.AsyncWaitHandle.WaitOne(5000);
      DateTime now = DateTime.Now;
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
      UdpClient udpClient = ((BattleManager.UdpState) ar.AsyncState).UdpClient;
      IPEndPoint endPoint = ((BattleManager.UdpState) ar.AsyncState).EndPoint;
      try
      {
        byte[] Buff = udpClient.EndReceive(ar, ref remoteEP);
        if (Buff.Length >= 22)
        {
          BattleHandler battleHandler = new BattleHandler(BattleManager.UdpClient, Buff, remoteEP, now);
        }
        else
          Logger.warning("No Length (22) Buffer: " + BitConverter.ToString(Buff));
      }
      catch (Exception ex)
      {
        Logger.warning("Exception: " + (object) remoteEP.Address + ":" + (object) remoteEP.Port);
        Logger.warning(ex.ToString());
      }
      BattleManager.Read(new BattleManager.UdpState(endPoint, udpClient));
    }

    public static void Send(byte[] data, IPEndPoint ip)
    {
      BattleManager.UdpClient.Send(data, data.Length, ip);
    }

    private class UdpState
    {
      public IPEndPoint EndPoint;
      public UdpClient UdpClient;

      public UdpState(IPEndPoint EndPoint, UdpClient UdpClient)
      {
        this.EndPoint = EndPoint;
        this.UdpClient = UdpClient;
      }
    }
  }
}

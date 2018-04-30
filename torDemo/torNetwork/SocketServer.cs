using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Google.ProtocolBuffers;
using System.Diagnostics;
using ChuMeng;

namespace torNetwork
{
    /// <summary>
    /// Message buffer.
    /// </summary>
    public  class MsgBuffer{
        public int position = 0;
        public System.Byte[] buffer;
        public int size{
            get{
                return buffer.Length - position;
            }
        }
    }

    /// <summary>
    /// Agent.
    /// </summary>
    public class Agent{
        public static uint maxId = 0;

        public uint id;

        Socket mSocket;

        ServerMsgReader msgReader;

        bool isClose = false;

        //
        public Actor actor;

        //
        public WatchDog watchDog;

        //
        SocketServer server;

        //
        List<MsgBuffer> msgBuffers = new List<MsgBuffer>();

        //
        EndPoint endPoint;

        //
        private byte[] mTemp = new byte[0x20000];


        /// <summary>
        /// Initializes a new instance of the <see cref="T:torNetwork.Agent"/> class.
        /// </summary>
        /// <param name="socket">Socket.</param>
        public Agent(Socket socket)
        {
            id = ++maxId;
            mSocket = socket;
            endPoint = mSocket.RemoteEndPoint;
            msgReader = new ServerMsgReader();
        }
    }

    /// <summary>
    /// Socket server.
    /// </summary>
    public class SocketServer:Actor
    {
        public SocketServer()
        {
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mirage;
using Mirage.Sockets.Udp;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args[0] == "-server")
        {
            var server = StartUDPServer(7777);
            await RunUpdates(server, () => { }, 60);
        }
        if (args[0] == "-client")
        {
            var client = StartUDPClient("localhost", 7777);
            await RunUpdates(client, () => { }, 60);
        }
    }

    public static MirageServer StartUDPServer(int port)
    {
        var server = new MirageServer();

        var socketFactory = new UdpSocketFactory();
        var socket = socketFactory.CreateSocket();
        var endpoint = socketFactory.GetBindEndPoint(port);
        server.StartServer(socket, socketFactory.MaxPacketSize, endpoint);

        return server;
    }
    public static MirageClient StartUDPClient(string address, int port)
    {
        var client = new MirageClient();

        var socketFactory = new UdpSocketFactory();
        var socket = socketFactory.CreateSocket();
        var endpoint = socketFactory.GetConnectEndPoint(address, checked((ushort)port));
        client.Connect(socket, socketFactory.MaxPacketSize, endpoint);
        return client;
    }
    public static async Task RunUpdates(MiragePeer miragePeer, Action update, int updatesPerSecond, CancellationToken cancellation = default)
    {
        var updateInterval = 1000f / updatesPerSecond;

        var timer = Stopwatch.StartNew();
        var nextUpdate = 0f;
        while (!cancellation.IsCancellationRequested)
        {
            var now = timer.ElapsedMilliseconds;
            if (now > nextUpdate)
            {
                nextUpdate = now + updateInterval;
                miragePeer.UpdateReceive();
                update?.Invoke();
                miragePeer.UpdateSent();
            }

            await Task.Delay(1);
        }
    }
}


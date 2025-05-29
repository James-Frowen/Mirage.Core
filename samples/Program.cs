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
        var server_task = Task.CompletedTask;
        MirageServer server = null;
        MirageClient client = null;

        if (args[0] == "-server")
        {
            server = StartUDPServer(7777);
            server_task = RunUpdates(server, () => { }, 60);
        }

        var client_task = Task.CompletedTask;
        if (args[1] == "-client")
        {
            client = StartUDPClient("localhost", 7777);
            client_task = RunUpdates(client, () => { }, 60);
        }

        if (server != null && client != null)
            await SendDebugEcho(server, client);

        await Task.WhenAll(server_task, client_task);
    }

    [NetworkMessage]
    public struct DebugEcho
    {
        public int n;
    }
    private static async Task SendDebugEcho(MirageServer server, MirageClient client)
    {
        server.MessageHandler.RegisterHandler<DebugEcho>((p, msg) =>
        {
            p.Send(msg);
        });

        client.MessageHandler.RegisterHandler<DebugEcho>((p, msg) =>
        {
            Console.WriteLine($"Echo received {msg.n}");
        });

        for (var i = 0; i < 100; i++)
        {
            Console.WriteLine($"Echo Sent {i}");
            client.Player.Send(new DebugEcho { n = i });
            await Task.Delay(1000);
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


using System;
using Mirage.Logging;
using Mirage.Serialization;
using Mirage.SocketLayer;

namespace Mirage
{
    /// <summary>
    /// A High level network connection. This is used for connections from client-to-server and for connection from server-to-client.
    /// </summary>
    /// <remarks>
    /// <para>A NetworkConnection corresponds to a specific connection for a host in the transport layer. It has a connectionId that is assigned by the transport layer and passed to the Initialize function.</para>
    /// <para>A NetworkClient has one NetworkConnection. A NetworkServerSimple manages multiple NetworkConnections. The NetworkServer has multiple "remote" connections and a "local" connection for the local client.</para>
    /// <para>The NetworkConnection class provides message sending and handling facilities. For sending data over a network, there are methods to send message objects, byte arrays, and NetworkWriter objects. To handle data arriving from the network, handler functions can be registered for message Ids, byte arrays can be processed by HandleBytes(), and NetworkReader object can be processed by HandleReader().</para>
    /// <para>NetworkConnection objects also act as observers for networked objects. When a connection is an observer of a networked object with a NetworkIdentity, then the object will be visible to corresponding client for the connection, and incremental state changes will be sent to the client.</para>
    /// <para>There are many virtual functions on NetworkConnection that allow its behaviour to be customized. NetworkClient and NetworkServer can both be made to instantiate custom classes derived from NetworkConnection by setting their networkConnectionClass member variable.</para>
    /// </remarks>
    public sealed class NetworkPlayer : INetworkPlayer
    {
        private static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkPlayer));

        /// <summary>
        /// Transport level connection
        /// </summary>
        /// <remarks>
        /// <para>On a server, this Id is unique for every connection on the server. On a client this Id is local to the client, it is not the same as the Id on the server for this connection.</para>
        /// <para>Transport layers connections begin at one. So on a client with a single connection to a server, the connectionId of that connection will be one. In NetworkServer, the connectionId of the local connection is zero.</para>
        /// <para>Clients do not know their connectionId on the server, and do not know the connectionId of other clients on the server.</para>
        /// </remarks>
        private readonly IConnection _connection;

        public bool IsHost { get; }

        /// <summary>
        /// Has this player been marked as disconnected
        /// <para>Messages sent to disconnected players will be ignored</para>
        /// </summary>
        private bool _isDisconnected = false;

        /// <summary>
        /// Flag that tells us if the scene has fully loaded in for player.
        /// <para>This property is read-only. It is set by the system on the client when the scene has fully loaded, and set by the system on the server when a ready message is received from a client.</para>
        /// <para>A client that is ready is sent spawned objects by the server and updates to the state of spawned objects. A client that is not ready is not sent spawned objects.</para>
        /// <para>Starts as true, when a client connects it is assumed that it is already in a ready scene. It will be set to not ready if NetworkSceneManager loads a scene</para>
        /// </summary>
        public bool SceneIsReady { get; set; } = true;

        /// <summary>
        /// The IP address / URL / FQDN associated with the connection.
        /// Can be useful for a game master to do IP Bans etc.
        /// </summary>
        public IEndPoint Address => _connection.EndPoint;

        public IConnection Connection => _connection;

        /// <summary>
        /// Disconnects the player.
        /// <para>A disconnected player can not send messages</para>
        /// </summary>
        /// <remarks>
        /// This method exists so that users do not need to add reference to SocketLayer asmdef
        /// </remarks>
        public void Disconnect()
        {
            // dont need to call disconnect twice, so just return
            if (_isDisconnected)
                return;

            _connection.Disconnect();
            _isDisconnected = true;
        }

        /// <summary>
        /// Marks player as disconnected, used when the disconnect call is from peer
        /// <para>A disconnected player can not send messages</para>
        /// </summary>
        public void MarkAsDisconnected()
        {
            _isDisconnected = true;
        }

        /// <summary>
        /// Creates a new NetworkConnection with the specified address and connectionId
        /// </summary>
        /// <param name="networkConnectionId"></param>
        public NetworkPlayer(IConnection connection, bool isHost)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            IsHost = isHost;
        }

        /// <summary>
        /// This sends a network message to the connection.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The transport layer channel to send on.</param>
        /// <returns></returns>
        public void Send<T>(T message, Channel channelId = Channel.Reliable)
        {
            if (_isDisconnected) { return; }

            using (var writer = NetworkWriterPool.GetWriter())
            {
                MessagePacker.Pack(message, writer);

                var segment = writer.ToArraySegment();
                NetworkDiagnostics.OnSend(message, segment.Count, 1);
                if (logger.LogEnabled()) logger.Log($"Sending {typeof(T)} to {this} channel:{channelId}");
                Send(segment, channelId);
            }
        }

        /// <summary>
        /// Sends a block of data
        /// <para>Only use this method if data has message Id already included, otherwise receives wont know how to handle it. Otherwise use <see cref="Send{T}(T, int)"/></para>
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="channelId"></param>
        public void Send(ArraySegment<byte> segment, Channel channelId = Channel.Reliable)
        {
            if (_isDisconnected) { return; }

            if (channelId == Channel.Reliable)
            {
                _connection.SendReliable(segment);
            }
            else
            {
                _connection.SendUnreliable(segment);
            }
        }

        /// <summary>
        /// This sends a network message to the connection.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="msg">The message to send.</param>
        /// <param name="channelId">The transport layer channel to send on.</param>
        /// <returns></returns>
        public void Send<T>(T message, INotifyCallBack callBacks)
        {
            if (_isDisconnected) { return; }

            using (var writer = NetworkWriterPool.GetWriter())
            {
                MessagePacker.Pack(message, writer);

                var segment = writer.ToArraySegment();
                NetworkDiagnostics.OnSend(message, segment.Count, 1);
                if (logger.LogEnabled()) logger.Log($"Sending {typeof(T)} to {this} channel:Notify");
                _connection.SendNotify(segment, callBacks);
            }
        }

        public override string ToString()
        {
            return $"connection({Address})";
        }
    }
}

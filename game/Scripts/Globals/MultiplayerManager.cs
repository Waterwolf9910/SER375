
using Godot;

// //TODO: Maybe a server for lobbies
// public partial class MultiplayerManager : Node {

//     #pragma warning disable CS8625
//     public static MultiplayerManager INSTANCE {get; private set;} = null;
//     #pragma warning restore CS8625

//     [Signal]
//     public delegate void player_connectedEventHandler(long id);

//     [Signal]
//     public delegate void player_disconnectedEventHandler(long id);

//     [Signal]
//     public delegate void server_createdEventHandler();

//     [Signal]
//     public delegate void connectedEventHandler();

//     [Signal]
//     public delegate void connection_failedEventHandler();

//     [Signal]
//     public delegate void disconnectedEventHandler();

//     [Signal]
//     public delegate void new_peerEventHandler(long id);

//     public const int PORT = 3030;

//     private PackedScene menu {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/Test Scenes/multiplayer_start.tscn");

//     // private MultiplayerApi server_ = new SceneMultiplayer();
//     // private MultiplayerApi client_ = new SceneMultiplayer();
//     private Node client_node = new() {
//         Name = "Client"
//     };
//     private Node server_node = new()  {
//         Name = "Server"
//     };

//     public long unique_id => client_node.Multiplayer.GetUniqueId();
//     public bool is_connected => client_node.Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;
    
//     // private ENetMultiplayerPeer? server = null;
//     // private ENetMultiplayerPeer client = new();
//     // public MultiplayerApi multiplayer = new();

//     public override void _Ready() {
//         base._Ready();
//         this.AddChild(this.server_node);
//         this.AddChild(this.client_node);
//         this.client_node.Owner = this;
//         this.server_node.Owner = this;
//         this.GetTree().SetMultiplayer(new SceneMultiplayer(), client_node.GetPath());
//         this.GetTree().SetMultiplayer(new SceneMultiplayer(), server_node.GetPath());

//         this.server_node.Multiplayer.PeerConnected += OnClientAdded;
//         this.server_node.Multiplayer.PeerDisconnected += OnClientRemoved;
        

//         INSTANCE = this;
//     }

//     private void setupClientSignals() {
//         this.client_node.Multiplayer.ConnectedToServer += this.OnConnectedToServer;
//         this.client_node.Multiplayer.ConnectionFailed += EmitSignalconnection_failed;
//         this.client_node.Multiplayer.PeerConnected += EmitSignalnew_peer;
//         this.client_node.Multiplayer.PeerDisconnected += OnDisconnectedFromServer;
//         this.client_node.Multiplayer.ServerDisconnected += EmitSignaldisconnected;
//     }

//     private void clearClientSignals() {
//         if (!this.client_node.Multiplayer.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(this.OnConnectedToServer))) {
//             return;
//         }
//         this.client_node.Multiplayer.ConnectedToServer -= this.OnConnectedToServer;
//         this.client_node.Multiplayer.ConnectionFailed -= EmitSignalconnection_failed;
//         this.client_node.Multiplayer.PeerConnected -= EmitSignalnew_peer;
//         this.client_node.Multiplayer.PeerDisconnected -= OnDisconnectedFromServer;
//         this.client_node.Multiplayer.ServerDisconnected -= EmitSignaldisconnected;
//     }

//     public void StartServer(bool standalone = false) {
//         var server_peer = new ENetMultiplayerPeer();
//         // server_peer.Connect(ENetMultiplayerPeer.SignalName.PeerConnected, Callable.From((long i) => {
//         //     GD.Print("a");
//         // }));
//         // server_peer.Connect(ENetMultiplayerPeer.SignalName.PeerDisconnected, Callable.From(() => {
//         //     GD.Print("b");
//         // }));
//         // server_peer.PeerConnected += OnClientConnectToServer;
//         // server_peer.PeerDisconnected += OnClientDisconnectFromServer;
//         Error err1 = server_peer.CreateServer(PORT, 2);
//         if (err1 != Error.Ok) {
//             GD.PrintErr($"Error creating server {err1}");
//             return;
//         }
//         this.server_node.Multiplayer.MultiplayerPeer = server_peer;
//         GD.Print($"Server listening on port {PORT}");
//         if (!standalone) {
//             Error err2 = ConnectToServer("127.0.0.1", PORT, true);
//             if (err2 != Error.Ok) {
//                 GD.PrintErr($"Error connecting to local server {err2}");
//                 this.StopServer();
//                 return;
//             }
//         }
//         EmitSignalserver_created();
//     }

//     public Error ConnectToServer(string address, int port, bool server_client = false) {
//         var client_peer = new ENetMultiplayerPeer();
//         var result = client_peer.CreateClient(address, port);
//         // client_peer.PeerDisconnected += OnDisconnectFromServer;
//         // client_peer.PeerConnected += (id) => {
//         //     GD.PrintS("Connected to server", $"({id})");
//         // };
//         this.Disconnect();
//         if (!server_client) {
//             setupClientSignals();
//         }
//         this.client_node.Multiplayer.MultiplayerPeer = client_peer;
//         GD.Print($"Connecting to {address}:{port}");
//         return result;
//     }

//     public void StopServer() {
//         this.server_node.Multiplayer.MultiplayerPeer.Close();
//         // this.server_node.Multiplayer.MultiplayerPeer = null;
//         this.Disconnect();
//         this.GetTree().ChangeSceneToPacked(this.menu);
//     }

//     public void Disconnect() {
//         this.clearClientSignals();
//         this.client_node.Multiplayer.MultiplayerPeer.Close();
//         // this.client_node.Multiplayer.MultiplayerPeer = null;
//     }

//     private void OnClientAdded(long id) {
//         GD.Print($"Peer {id} connected");
//         if (id == 1) {
//             return;
//         }
//         EmitSignal(SignalName.player_connected, id);
//     }

//     private void OnClientRemoved(long id) {
//         GD.Print($"Client {id} disconnected");
//         EmitSignal(SignalName.player_disconnected, id);
//     }

//     private void OnDisconnectedFromServer(long id) {
//         GD.Print("Disconnected from server");
//     }

//     private void OnConnectedToServer() {
//         GD.Print("Connected to server!");
//         EmitSignalconnected();
//     }

//     public void AttachMultiplayer(MultiplayerApi multiplayer, bool server = false) {
//         if (this.server_node.Multiplayer.GetPeers().Length > 0) {
//             // GD.PrintS("running", this.client_node.Multiplayer.get);
//             // multiplayer.MultiplayerPeer = this.server_node.Multiplayer.MultiplayerPeer;
//             multiplayer.MultiplayerPeer = this.server_node.Multiplayer.MultiplayerPeer;
//             return;
//         }
//         multiplayer.MultiplayerPeer = this.client_node.Multiplayer.MultiplayerPeer;
//     }

//     [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
//     public void SendInitInfoToServer(Deck card, string name) {}

    
// }

// //TODO: Maybe a server for lobbies
// public partial class MultiplayerManager : Node {

//     #pragma warning disable CS8625
//     public static MultiplayerManager INSTANCE {get; private set;} = null;
//     #pragma warning restore CS8625

//     [Signal]
//     public delegate void player_connectedEventHandler(long id);

//     [Signal]
//     public delegate void player_disconnectedEventHandler(long id);

//     [Signal]
//     public delegate void server_createdEventHandler();

//     [Signal]
//     public delegate void connectedEventHandler();

//     [Signal]
//     public delegate void connection_failedEventHandler();

//     [Signal]
//     public delegate void disconnectedEventHandler();

//     [Signal]
//     public delegate void new_peerEventHandler(long id);

//     public const int PORT = 3030;

//     private PackedScene menu {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/Test Scenes/multiplayer_start.tscn");

//     private MultiplayerApi server = new SceneMultiplayer();
//     private MultiplayerApi client = new SceneMultiplayer();
//     private Node server_node = new()  {
//         Name = "Server"
//     };

//     public long unique_id => this.client.GetUniqueId();
//     public bool is_connected => this.client.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;

//     public bool is_server = false;
    
//     // private ENetMultiplayerPeer? server = null;
//     // private ENetMultiplayerPeer client = new();
//     // public MultiplayerApi multiplayer = new();

//     public override void _Ready() {
//         base._Ready();
//         this.AddChild(this.server_node);
//         this.server_node.Owner = this;
//         this.GetTree().SetMultiplayer(this.client, this.GetTree().CurrentScene.GetPath());
//         this.GetTree().SetMultiplayer(this.server, server_node.GetPath());

//         this.server.PeerConnected += OnClientAdded;
//         this.server.PeerDisconnected += OnClientRemoved;
        
//         this.GetTree().SceneChanged += OnSceneChanged;

//         INSTANCE = this;
//     }

//     public void OnSceneChanged() {
//         this.GetTree().SetMultiplayer(is_server ? this.server : this.client, this.GetTree().CurrentScene.GetPath());
//         // if (!is_server) {
//         //     this.setupClientSignals();
//         // }
//     }

//     private void setupClientSignals() {
//         this.client.ConnectedToServer += this.OnConnectedToServer;
//         this.client.ConnectionFailed += EmitSignalconnection_failed;
//         this.client.PeerConnected += EmitSignalnew_peer;
//         this.client.PeerDisconnected += OnDisconnectedFromServer;
//         this.client.ServerDisconnected += EmitSignaldisconnected;
//     }

//     private void clearClientSignals() {
//         if (!this.client.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(this.OnConnectedToServer))) {
//             return;
//         }
//         this.client.ConnectedToServer -= this.OnConnectedToServer;
//         this.client.ConnectionFailed -= EmitSignalconnection_failed;
//         this.client.PeerConnected -= EmitSignalnew_peer;
//         this.client.PeerDisconnected -= OnDisconnectedFromServer;
//         this.client.ServerDisconnected -= EmitSignaldisconnected;
//     }

//     public void StartServer(bool standalone = false) {
//         var server_peer = new ENetMultiplayerPeer();
//         is_server = true;
//         this.GetTree().SetMultiplayer(this.server, this.GetTree().CurrentScene.GetPath());
//         this.GetTree().SetMultiplayer(this.client, server_node.GetPath());
//         // server_peer.Connect(ENetMultiplayerPeer.SignalName.PeerConnected, Callable.From((long i) => {
//         //     GD.Print("a");
//         // }));
//         // server_peer.Connect(ENetMultiplayerPeer.SignalName.PeerDisconnected, Callable.From(() => {
//         //     GD.Print("b");
//         // }));
//         // server_peer.PeerConnected += OnClientConnectToServer;
//         // server_peer.PeerDisconnected += OnClientDisconnectFromServer;
//         Error err1 = server_peer.CreateServer(PORT, 2);
//         if (err1 != Error.Ok) {
//             GD.PrintErr($"Error creating server {err1}");
//             return;
//         }
//         this.server.MultiplayerPeer = server_peer;
//         GD.Print($"Server listening on port {PORT}");
//         if (!standalone) {
//             Error err2 = ConnectToServer("127.0.0.1", PORT);
//             if (err2 != Error.Ok) {
//                 GD.PrintErr($"Error connecting to local server {err2}");
//                 this.StopServer();
//                 return;
//             }
//         }
//         EmitSignalserver_created();
//     }

//     public Error ConnectToServer(string address, int port) {
//         var client_peer = new ENetMultiplayerPeer();
//         var result = client_peer.CreateClient(address, port);
//         // client_peer.PeerDisconnected += OnDisconnectFromServer;
//         // client_peer.PeerConnected += (id) => {
//         //     GD.PrintS("Connected to server", $"({id})");
//         // };
//         this.Disconnect();
//         if (!is_server) {
//             setupClientSignals();
//         }
//         this.client.MultiplayerPeer = client_peer;
//         GD.Print($"Connecting to {address}:{port}");
//         return result;
//     }

//     public void StopServer() {
//         this.GetTree().SetMultiplayer(this.client, this.GetTree().CurrentScene.GetPath());
//         this.GetTree().SetMultiplayer(this.server, server_node.GetPath());
//         is_server = false;
//         this.server.MultiplayerPeer.Close();
//         // this.server_node.Multiplayer.MultiplayerPeer = null;
//         this.Disconnect();
//         this.GetTree().ChangeSceneToPacked(this.menu);
//     }

//     public void Disconnect() {
//         this.clearClientSignals();
//         this.client.MultiplayerPeer.Close();
//         // this.client_node.Multiplayer.MultiplayerPeer = null;
//     }

//     private void OnClientAdded(long id) {
//         if (id == 1) {
//             return;
//         }
//         GD.Print($"Peer {id} connected");
//         EmitSignal(SignalName.player_connected, id);
//     }

//     private void OnClientRemoved(long id) {
//         GD.Print($"Client {id} disconnected");
//         EmitSignal(SignalName.player_disconnected, id);
//     }

//     private void OnDisconnectedFromServer(long id) {
//         GD.Print("Disconnected from server");
//     }

//     private void OnConnectedToServer() {
//         GD.Print($"Connected to server! ({this.client.GetUniqueId()})");
//         EmitSignalconnected();
//     }

//     // public void AttachMultiplayer(MultiplayerApi multiplayer) {
//         // if (this.server.GetPeers().Length > 0) {
//         //     // GD.PrintS("running", this.client_node.Multiplayer.get);
//         //     // multiplayer.MultiplayerPeer = this.server_node.Multiplayer.MultiplayerPeer;
//         //     multiplayer.MultiplayerPeer = this.server.MultiplayerPeer;
//         //     return;
//         // }
//         // multiplayer.MultiplayerPeer = this.client.MultiplayerPeer;
//     // }

//     [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
//     public void SendInitInfoToServer(Deck card, string name) {}

    
// }


//TODO: Maybe a server for lobbies
public partial class MultiplayerManager : Node {

    #pragma warning disable CS8625
    public static MultiplayerManager INSTANCE {get; private set;} = null;
    #pragma warning restore CS8625

    [Signal]
    public delegate void player_connectedEventHandler(long id);

    [Signal]
    public delegate void player_disconnectedEventHandler(long id);

    [Signal]
    public delegate void server_createdEventHandler();

    [Signal]
    public delegate void connectedEventHandler();

    [Signal]
    public delegate void connection_failedEventHandler();

    [Signal]
    public delegate void disconnectedEventHandler();

    [Signal]
    public delegate void new_peerEventHandler(long id);

    public const int PORT = 3030;

    private PackedScene menu {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/Test Scenes/multiplayer_start.tscn");

    private MultiplayerApi scene_multiplayer = new SceneMultiplayer();
    // private MultiplayerApi server = new SceneMultiplayer();
    // private MultiplayerApi client = new SceneMultiplayer();
    // private Node server_node = new()  {
    //     Name = "Server"
    // };

    public long unique_id => this.scene_multiplayer.GetUniqueId();
    public bool is_connected =>  is_server || this.scene_multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected  ;

    public bool is_server = false;
    
    // private ENetMultiplayerPeer? server = null;
    // private ENetMultiplayerPeer client = new();
    // public MultiplayerApi multiplayer = new();

    public override void _Ready() {
        base._Ready();
        this.GetTree().SetMultiplayer(this.scene_multiplayer, this.GetTree().CurrentScene.GetPath());

        this.scene_multiplayer.PeerConnected += OnClientAdded;
        this.scene_multiplayer.PeerDisconnected += OnClientRemoved;
        
        this.GetTree().SceneChanged += OnSceneChanged;

        INSTANCE = this;
    }

    public void OnSceneChanged() {
        this.GetTree().SetMultiplayer(this.scene_multiplayer, this.GetTree().CurrentScene.GetPath());
    
        // if (!is_server) {
        //     this.setupClientSignals();
        // }
    }

    private void setupClientSignals() {
        if (this.is_server) {
            return;
        }
        this.scene_multiplayer.ConnectedToServer += this.OnConnectedToServer;
        this.scene_multiplayer.ConnectionFailed += EmitSignalconnection_failed;
        this.scene_multiplayer.PeerConnected += EmitSignalnew_peer;
        this.scene_multiplayer.PeerDisconnected += OnDisconnectedFromServer;
        this.scene_multiplayer.ServerDisconnected += EmitSignaldisconnected;
    }

    private void clearClientSignals() {
        if (this.is_server || !this.scene_multiplayer.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(this.OnConnectedToServer))) {
            return;
        }
        this.scene_multiplayer.ConnectedToServer -= this.OnConnectedToServer;
        this.scene_multiplayer.ConnectionFailed -= EmitSignalconnection_failed;
        this.scene_multiplayer.PeerConnected -= EmitSignalnew_peer;
        this.scene_multiplayer.PeerDisconnected -= OnDisconnectedFromServer;
        this.scene_multiplayer.ServerDisconnected -= EmitSignaldisconnected;
    }

    public void StartServer() {
        var server_peer = new ENetMultiplayerPeer();
        is_server = true;
        Error err1 = server_peer.CreateServer(PORT, 2);
        if (err1 != Error.Ok) {
            GD.PrintErr($"Error creating server {err1}");
            return;
        }
        this.scene_multiplayer.MultiplayerPeer = server_peer;
        GD.Print($"Server listening on port {PORT}");
        EmitSignalserver_created();
    }

    public Error ConnectToServer(string address, int port) {
        var client_peer = new ENetMultiplayerPeer();
        var result = client_peer.CreateClient(address, port);
        if (is_connected) {
            this.Disconnect();
        }
        setupClientSignals();
        this.scene_multiplayer.MultiplayerPeer = client_peer;
        GD.Print($"Connecting to {address}:{port}");
        return result;
    }

    public void StopServer() {
        is_server = false;
        this.scene_multiplayer.MultiplayerPeer.Close();
        this.Disconnect();
        this.GetTree().ChangeSceneToPacked(this.menu);
    }

    public void Disconnect() {
        this.clearClientSignals();
        this.scene_multiplayer.MultiplayerPeer.Close();
        // this.client_node.Multiplayer.MultiplayerPeer = null;
    }

    private void OnClientAdded(long id) {
        if (id == 1) {
            return;
        }
        GD.Print($"Peer {id} connected");
        EmitSignal(SignalName.player_connected, id);
    }

    private void OnClientRemoved(long id) {
        GD.Print($"Client {id} disconnected");
        EmitSignal(SignalName.player_disconnected, id);
    }

    private void OnDisconnectedFromServer(long id) {
        GD.Print("Disconnected from server");
    }

    private void OnConnectedToServer() {
        GD.Print($"Connected to server! ({this.scene_multiplayer.GetUniqueId()})");
        EmitSignalconnected();
    }

    // public void AttachMultiplayer(MultiplayerApi multiplayer) {
        // if (this.server.GetPeers().Length > 0) {
        //     // GD.PrintS("running", this.client_node.Multiplayer.get);
        //     // multiplayer.MultiplayerPeer = this.server_node.Multiplayer.MultiplayerPeer;
        //     multiplayer.MultiplayerPeer = this.server.MultiplayerPeer;
        //     return;
        // }
        // multiplayer.MultiplayerPeer = this.client.MultiplayerPeer;
    // }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void SendInitInfoToServer(Deck card, string name) {}

    
}

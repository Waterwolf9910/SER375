
using Godot;

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

    // private MultiplayerApi scene_multiplayer = new SceneMultiplayer();
    private MultiplayerApi server = new SceneMultiplayer();
    private MultiplayerApi client = new SceneMultiplayer();
    // private Node server_node = new()  {
    //     Name = "Server"
    // };

    public long unique_id => this.client.GetUniqueId();
    public bool is_connected =>  is_server || this.client.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected  ;

    public bool is_server = false;
    
    // private ENetMultiplayerPeer? server = null;
    // private ENetMultiplayerPeer client = new();
    // public MultiplayerApi multiplayer = new();

    public override void _Ready() {
        base._Ready();
        this.GetTree().SetMultiplayer(this.client, this.GetTree().CurrentScene.GetPath());
        this.GetTree().SetMultiplayer(this.server, this.GetPath());

        this.server.PeerConnected += OnClientAdded;
        this.server.PeerDisconnected += OnClientRemoved;
        this.client.PeerConnected += OnClientAdded;
        this.client.PeerDisconnected += OnClientRemoved;
        
        this.GetTree().SceneChanged += OnSceneChanged;

        INSTANCE = this;
    }

    public void OnSceneChanged() {
        this.GetTree().SetMultiplayer(this.is_server ? this.server : this.client, this.GetTree().CurrentScene.GetPath());
    
        // if (!is_server) {
        //     this.setupClientSignals();
        // }
    }

    private void setupClientSignals() {
        if (this.is_server) {
            return;
        }
        this.client.ConnectedToServer += this.OnConnectedToServer;
        this.client.ConnectionFailed += EmitSignalconnection_failed;
        this.client.ServerDisconnected += EmitSignaldisconnected;
    }

    private void clearClientSignals() {
        if (this.is_server || !this.client.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(this.OnConnectedToServer))) {
            return;
        }
        this.client.ConnectedToServer -= this.OnConnectedToServer;
        this.client.ConnectionFailed -= EmitSignalconnection_failed;
        this.client.ServerDisconnected -= EmitSignaldisconnected;
    }

    public void StartServer() {
        var server_peer = new ENetMultiplayerPeer();
        is_server = true;
        Error err1 = server_peer.CreateServer(PORT, 2);
        if (err1 != Error.Ok) {
            GD.PrintErr($"Error creating server {err1}");
            return;
        }
        // this.GetTree().SetMultiplayer(this.server, this.GetTree().CurrentScene.GetPath());
        this.server.MultiplayerPeer = server_peer;
        GD.Print($"Server listening on port {PORT}");
        this.ConnectToServer("127.0.0.1", PORT);
        EmitSignalserver_created();
    }

    public Error ConnectToServer(string address, int port) {
        var client_peer = new ENetMultiplayerPeer();
        var result = client_peer.CreateClient(address, port);
        if (is_connected) {
            this.Disconnect();
        }
        setupClientSignals();
        this.client.MultiplayerPeer = client_peer;
        GD.Print($"Connecting to {address}:{port}");
        return result;
    }

    public void StopServer() {
        is_server = false;
        this.server.MultiplayerPeer.Close();
        this.server.MultiplayerPeer = null;
        this.Disconnect();
        this.GetTree().ChangeSceneToPacked(this.menu);
    }

    public void Disconnect() {
        this.clearClientSignals();
        this.client.MultiplayerPeer.Close();
        this.client.MultiplayerPeer = null;
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
        GD.Print($"Connected to server! ({this.client.GetUniqueId()})");
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


using Godot;
using Godot.Collections;
using System.Diagnostics;
using System.Linq;

//TODO: Maybe a server for lobbies
public partial class MultiplayerManager : Node {

    Array<string> names = ["name1", "name2", "name3", "name4", "name5"];

    #pragma warning disable CS8625
    public static MultiplayerManager INSTANCE {get; private set;} = null;
    #pragma warning restore CS8625

    public static Dictionary<long, PlayerInfo> players {get; private set;} = [];
    public static Dictionary<long, long> battle {get;} = [];

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

        this.scene_multiplayer.PeerConnected += onClientAdded;
        this.scene_multiplayer.PeerDisconnected += onClientRemoved;
        
        this.GetTree().SceneChanged += OnSceneChanged;

        INSTANCE = this;
    }

    public void OnSceneChanged() {
        Callable.From(() => {
            this.GetTree().SetMultiplayer(this.scene_multiplayer, this.GetTree().CurrentScene.GetPath());
        }).CallDeferred();
    
        // if (!is_server) {
        //     this.setupClientSignals();
        // }
    }

    private void setupClientSignals() {
        if (this.is_server) {
            return;
        }
        this.scene_multiplayer.ConnectedToServer += this.onConnectedToServer;
        this.scene_multiplayer.ConnectionFailed += EmitSignalconnection_failed;
        this.scene_multiplayer.PeerConnected += EmitSignalnew_peer;
        this.scene_multiplayer.PeerDisconnected += onDisconnectedFromServer;
        this.scene_multiplayer.ServerDisconnected += EmitSignaldisconnected;
    }

    private void clearClientSignals() {
        if (this.is_server || !this.scene_multiplayer.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(this.onConnectedToServer))) {
            return;
        }
        this.scene_multiplayer.ConnectedToServer -= this.onConnectedToServer;
        this.scene_multiplayer.ConnectionFailed -= EmitSignalconnection_failed;
        this.scene_multiplayer.PeerConnected -= EmitSignalnew_peer;
        this.scene_multiplayer.PeerDisconnected -= onDisconnectedFromServer;
        this.scene_multiplayer.ServerDisconnected -= EmitSignaldisconnected;
    }

    public void startServer() {
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
        players.Add(1, createLocalPlayer());
    }

    public Error connectToServer(string address, int port) {
        var client_peer = new ENetMultiplayerPeer();
        var result = client_peer.CreateClient(address, port);
        if (is_connected) {
            this.disconnect();
        }
        setupClientSignals();
        this.scene_multiplayer.MultiplayerPeer = client_peer;
        GD.Print($"Connecting to {address}:{port}");
        return result;
    }

    public void stopServer() {
        players.Clear();
        is_server = false;
        this.scene_multiplayer.MultiplayerPeer.Close();
        this.disconnect();
        this.GetTree().ChangeSceneToPacked(this.menu);
    }

    public void disconnect() {
        this.clearClientSignals();
        this.scene_multiplayer.MultiplayerPeer.Close();
        // this.client_node.Multiplayer.MultiplayerPeer = null;
    }

    private void onClientAdded(long id) {
        if (id == 1) {
            return;
        }
        GD.Print($"Peer {id} connected");
        EmitSignal(SignalName.player_connected, id);
        var peers = this.scene_multiplayer.GetPeers();
        this.CallDeferred(MethodName.RpcId, id, MethodName.syncPlayers, players);
        // this.RpcId(id, MethodName.syncPlayers, players);
    }

    private void onClientRemoved(long id) {
        players.Remove(id);
        GD.Print($"Client {id} disconnected");
        EmitSignal(SignalName.player_disconnected, id);
        this.RpcId(id, MethodName.removePlayer, players);
    }

    private void onDisconnectedFromServer(long id) {
        GD.Print("Disconnected from server");
    }

    private void onConnectedToServer() {
        GD.Print($"Connected to server! (as {this.scene_multiplayer.GetUniqueId()})");
        EmitSignalconnected();
        //todo: set player name and some other info via setting
        PlayerInfo player_info = new ();
        this.RpcId(1, MethodName.onInitInfo, player_info);
    }

    public PlayerInfo? getPlayerInfo(long id) {
        if (players.TryGetValue(id, out var info)) {
            return info;
        }
        return null;
    }

    public PlayerInfo createLocalPlayer() {
        return new() {
            player_name = "",
            deck = new() {
                card_ids = []
            },
            peer_id = this.unique_id
        };
    }

    public Dictionary<long, string> getPlayers() {
        Dictionary<long, string> d = new ();
        foreach (var kvp in players) {
            d.Add(kvp.Key, kvp.Value.player_name);
        }
        return d;
        // new(players.Select(v => new System.Collections.Generic.KeyValuePair<long, string>(v.Key, v.Value.player_name)));
    }

    private int get_rpc_caller => this.Multiplayer.GetRemoteSenderId();

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void onInitInfo(PlayerInfo info) { // client -> server
        info.peer_id = this.get_rpc_caller;
        // players.Add(info.peer_id, info);
        this.Rpc(MethodName.addPlayer, info);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void setDeck(Deck deck) { // client -> server
        if (!players.TryGetValue(this.Multiplayer.GetRemoteSenderId(), out var info)) {
            this.RpcId(this.get_rpc_caller, MethodName.sendError, "Your player does not exist!");
            return;
        }
        info.deck = deck;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void setupBattle(long id) { // client -> server
        if (!players.TryGetValue(this.get_rpc_caller, out var info) || !players.TryGetValue(id, out var info2)) {
            this.RpcId(this.get_rpc_caller, MethodName.sendError, "Unable to find one of the players");
            return;
        }
        bool p2_first = false;
        if (new RandomNumberGenerator().Randf() > .50f) {
            p2_first = true;
        }
        // server -> clients
        this.RpcId(info.peer_id, RyansTestScene.MethodName.startBattle, p2_first ? info.peer_id : info2.peer_id);
        this.RpcId(info2.peer_id, RyansTestScene.MethodName.startBattle, p2_first ? info2.peer_id : info.peer_id);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void syncPlayers(Dictionary<long, PlayerInfo> players) { //server -> clients
        if (this.get_rpc_caller != 1) {
            return;
        }
        MultiplayerManager.players = players;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void addPlayer(PlayerInfo info) { // server -> clients
        if (this.get_rpc_caller != 1) {
            return;
        }
        players.Add(info.peer_id, info);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void removePlayer(long id) { // server -> clients
        if (this.get_rpc_caller != 1) {
            return;
        }
        players.Remove(id);
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void sendError(string message) { // any
        GD.PrintErr($"{message}\n{new StackTrace()}");
    }

    
}

using Godot;
using System;
using System.Linq;

public partial class RyansTestScene : Node2D {

    private PackedScene battle {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/TestScenes/battle.tscn");
    private PackedScene player_scene {get;} = ResourceLoader.Load<PackedScene>("res://Nodes/Entities/test_player.tscn");
    
    // MultiplayerSpawner spawner;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // spawner = this.GetNode<MultiplayerSpawner>("%Spawner");
    
        MultiplayerManager.INSTANCE.new_peer += addPlayer;
        MultiplayerManager.INSTANCE.player_connected += addPlayer;
        addPlayer(MultiplayerManager.INSTANCE.unique_id);
        // MultiplayerManager.INSTANCE.player_disconnected += (id) => {
        //     var a = this.GetTree().Root.GetChildren();
        //     this.GetTree().Root.FindChild($"Player {id}").QueueFree();
        // };
        // if (MultiplayerManager.INSTANCE.is_connected) {
        //     addPlayer(1);
        //     addPlayer(MultiplayerManager.INSTANCE.unique_id);
        // }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        if (Input.IsKeyPressed(Key.B)) {
            GD.PrintS("hi");
            MultiplayerManager.INSTANCE.RpcId(1,
                MultiplayerManager.MethodName.setupBattle,
                MultiplayerManager.INSTANCE.getPlayers().First(v => v.Key != MultiplayerManager.INSTANCE.unique_id).Key
            );
        }
    }
    public void addPlayer(long id) {
        // if (!this.Multiplayer.IsServer()) {
        //     return;
        // }
        var new_player = player_scene.Instantiate<Player>();
        new_player.Name = $"Player {id}";
        this.AddChild(new_player, true);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void startBattle(long id, long starting_id) {
        var scene = (Battle) this.SwitchMultiplayerScene(battle);
        if (scene == null) {
            return;
        }
        scene.initBattle(id, MultiplayerManager.INSTANCE.unique_id, starting_id);
    }
}

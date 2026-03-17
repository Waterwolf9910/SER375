using Godot;
using System;

public partial class RyansTestScene : Node2D {

    // MultiplayerSpawner spawner;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // spawner = this.GetNode<MultiplayerSpawner>("%Spawner");
    
        // MultiplayerManager.INSTANCE.new_peer += addPlayer;
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
    }
}

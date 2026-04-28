using Godot;
using System;

public partial class MultiplayerStart : Node {

    public static MultiplayerStart? MULTIPLAYER_INSTANCE {get; private set;}
    private PackedScene world {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/Test Scenes/ryan.tscn");
    // private PackedScene player_scene {get;} = ResourceLoader.Load<PackedScene>("res://Nodes/Entities/test_player.tscn");
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export]
    private Control menu;
    [Export]
    public Node scene_folder;
    public Node current_scene;
    [Export]
    public MultiplayerSpawner spawner;
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        MULTIPLAYER_INSTANCE = this;
        // this.spawner.AddSpawnableScene(this.player_scene.ResourcePath);

        MultiplayerManager.INSTANCE.server_created += () => {
            this.SwitchMultiplayerScene(world);
            this.menu.Hide();
            // if (this.GetTree().ChangeSceneToPacked(this.world) != Error.Ok) {
            //     GD.PrintErr("Unable to create world");
            //     MultiplayerManager.INSTANCE.StopServer();
            // }
        };
        // MultiplayerManager.INSTANCE.new_peer += addPlayer;
        MultiplayerManager.INSTANCE.connected += () => {
            this.SwitchMultiplayerScene(world);
            this.menu.Hide();
            // if (this.GetTree().ChangeSceneToPacked(this.world) != Error.Ok) {
            //     GD.PrintErr("Unable to create world");
            //     MultiplayerManager.INSTANCE.Disconnect();
            // }
        };
        MultiplayerManager.INSTANCE.disconnected += () => {
            this.current_scene?.QueueFree();
            this.menu.Show();
        };
        // MultiplayerManager.INSTANCE.player_connected += addPlayer;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void CreateServer() {
        MultiplayerManager.INSTANCE.startServer();
    }

    public void JoinServer() {
        MultiplayerManager.INSTANCE.connectToServer("127.0.0.1", MultiplayerManager.PORT);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        MULTIPLAYER_INSTANCE = null;
    }
    
}
public static class Extensions {
    public static Node SwitchMultiplayerScene(this Node node, PackedScene scene) {
        var current_scene = scene.Instantiate();
        return SwitchMultiplayerScene(node, current_scene);
    }
    public static T SwitchMultiplayerScene<T>(this Node node, T instantiated_scene) where T : Node {
        if (MultiplayerStart.MULTIPLAYER_INSTANCE == null) {
            throw new Exception("Not in a multiplayer mode");
        }
        MultiplayerStart.MULTIPLAYER_INSTANCE.current_scene?.QueueFree();
        
        instantiated_scene.Name = "@Scene@";
        MultiplayerStart.MULTIPLAYER_INSTANCE.scene_folder.AddChild(instantiated_scene);
        instantiated_scene.Owner = MultiplayerStart.MULTIPLAYER_INSTANCE.scene_folder;
        MultiplayerStart.MULTIPLAYER_INSTANCE.spawner.SpawnPath = instantiated_scene.GetPath();
        MultiplayerStart.MULTIPLAYER_INSTANCE.current_scene = instantiated_scene;
        return instantiated_scene;
    }

    public static void AddSyncScene(this Node node, string resource_path) {
        if (MultiplayerStart.MULTIPLAYER_INSTANCE == null) {
            throw new Exception("Not in a multiplayer mode");
        }
        MultiplayerStart.MULTIPLAYER_INSTANCE.spawner.AddSpawnableScene(resource_path);
    }
}

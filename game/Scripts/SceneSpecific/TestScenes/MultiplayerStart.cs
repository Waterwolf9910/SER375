using Godot;
using Godot.Collections;

public partial class MultiplayerStart : Node {

    private PackedScene world {get;} = ResourceLoader.Load<PackedScene>("res://Scenes/Test Scenes/ryan.tscn");
    private PackedScene player_scene {get;} = ResourceLoader.Load<PackedScene>("res://Nodes/Entities/test_player.tscn");
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export]
    private Control menu;
    [Export]
    private Node scene_folder;
    private Node current_scene;
    // [Export]
    // private MultiplayerSpawner spawner;
    private Array<long> added = [];
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        // this.spawner.AddSpawnableScene(this.player_scene.ResourcePath);

        MultiplayerManager.INSTANCE.server_created += () => {
            this.current_scene?.QueueFree();
            this.current_scene = this.world.Instantiate();
            this.scene_folder.AddChild(this.current_scene);
            GD.PrintS("Server's unique_id:", MultiplayerManager.INSTANCE.unique_id);
            this.current_scene.Owner = this.scene_folder;
            // this.spawner.SpawnPath = this.current_scene.GetPath();
            // addPlayer(1);
            this.menu.Hide();
            if (added.Contains(MultiplayerManager.INSTANCE.unique_id)) {
                return;
            }
            addPlayer(MultiplayerManager.INSTANCE.unique_id);
            added.Add(MultiplayerManager.INSTANCE.unique_id);
            // if (this.GetTree().ChangeSceneToPacked(this.world) != Error.Ok) {
            //     GD.PrintErr("Unable to create world");
            //     MultiplayerManager.INSTANCE.StopServer();
            // }
        };
        MultiplayerManager.INSTANCE.new_peer += (id) => {
            if (added.Contains(id)) {
                return;
            }
            addPlayer(id);
            added.Add(id);
        };
        MultiplayerManager.INSTANCE.connected += () => {
            if (MultiplayerManager.INSTANCE.is_server) {
                return;
            }
            this.current_scene?.QueueFree();
            this.current_scene = this.world.Instantiate();
            this.scene_folder.AddChild(this.current_scene);
            this.current_scene.Owner = this.scene_folder;
            // this.spawner.SpawnPath = this.current_scene.GetPath();
            this.menu.Hide();
            if (added.Contains(MultiplayerManager.INSTANCE.unique_id)) {
                return;
            }
            addPlayer(MultiplayerManager.INSTANCE.unique_id);
            added.Add(MultiplayerManager.INSTANCE.unique_id);
            // if (this.GetTree().ChangeSceneToPacked(this.world) != Error.Ok) {
            //     GD.PrintErr("Unable to create world");
            //     MultiplayerManager.INSTANCE.Disconnect();
            // }
        };
        MultiplayerManager.INSTANCE.player_connected += (id) => {
            // if (MultiplayerManager.INSTANCE.is_server) {
            //     return;
            // }
            if (added.Contains(id)) {
                return;
            }
            addPlayer(id);
            added.Add(id);
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void CreateServer() {
        MultiplayerManager.INSTANCE.StartServer();
    }

    public void JoinServer() {
        MultiplayerManager.INSTANCE.ConnectToServer("127.0.0.1", MultiplayerManager.PORT);
    }

    public void addPlayer(long id) {
        // if (!this.Multiplayer.IsServer()) {
        //     return;
        // }
        var new_player = player_scene.Instantiate<Player>();
        new_player.Name = $"Player {id}";
        this.current_scene.AddChild(new_player, true);
    }
}

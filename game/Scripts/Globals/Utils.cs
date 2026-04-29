

using Godot;
using System.Linq;

public static partial class Utils {

    public static void QuitGame(this Node node) {
        node.GetTree().Root.PropagateNotification((int) Node.NotificationWMCloseRequest);
        Callable.From(() => {
            node.GetTree().Quit();
        }).CallDeferred();
    }

    public static void ChangeScene(this Node2D node, Node scene, bool copy_node_to_scene = false, Vector2? scene_spawn_pos = null) {
        var tree = node.GetTree();

        if (!copy_node_to_scene) {
            tree.ChangeSceneToNode(scene);
        }

        Vector2 spawn_pos = node.GlobalPosition;
        if (scene_spawn_pos == null) {
            var spawn_node = scene.GetNodeOrNull<Node2D>("%Spawn");
            if (spawn_node != null) {
                spawn_pos = spawn_node.GlobalPosition;
            }
        } else {
            spawn_pos = (Vector2) scene_spawn_pos;
        }

        var two_way_potent = scene
            .GetChildren(false)
            .Where(v => v is Interactable interactable &&
                interactable.components.TryGetValue("travel-return", out var return_available) &&
                return_available.AsBool()
            )
            .Select(v => (Interactable) v)
            .OrderBy(v => spawn_pos.DistanceSquaredTo(v.GlobalPosition));

        tree.CurrentScene.RemoveChild(node);
        if (two_way_potent.Any()) {
            Interactable interactable = two_way_potent.Count() == 1 ? two_way_potent.First() : two_way_potent.First(interactable => interactable.components.TryGetValue("travel-current", out var current_travel) && current_travel.AsBool());
            
            PackedScene saved_scene = new();
            saved_scene.Pack(tree.CurrentScene);
            interactable.components["travel"] = saved_scene;

            if (interactable.components.TryGetValue("travel-custom_return", out var custom)) {
                spawn_pos = custom.AsVector2();
            } else {
                if (!interactable.components.TryGetValue("travel-no_set_pos", out var no_set_pos) || !no_set_pos.AsBool()) {
                    interactable.components["travel-return_pos"] = node.GlobalPosition;
                }
            }
            interactable.components["travel-current"] = false;
        }

        scene.AddChild(node);
        node.Owner = scene;
        node.GlobalPosition = spawn_pos;
        tree.ChangeSceneToNode(scene);

    }

    public static void ChangeScene(this Node2D node, PackedScene scene, bool copy_node_to_scene = false, Vector2? scene_spawn_pos = null) {
        var tree = node.GetTree();
        if (!copy_node_to_scene) {
            tree.ChangeSceneToPacked(scene);
            return;
        }
        var instance = scene.Instantiate();
        ChangeScene(node, instance, copy_node_to_scene, scene_spawn_pos);
    }

    // public static void ChangeSceneToPacked(this Node _node, PackedScene packedScene) {
    //     _node.GetTree().ChangeSceneToPacked(packedScene);
    // }

    // public static void ChangeSceneToFile(this Node _node, string path) {
    //     _node.GetTree().ChangeSceneToFile(path);
    // }

    // public static void ChangeSceneToNode(this Node _node, Node node) {
    //     _node.GetTree().ChangeSceneToNode(node);
    // }

}

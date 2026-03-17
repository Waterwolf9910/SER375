

using Godot;

public static partial class Utils {
    
    public static void QuitGame(this Node node) {
        node.GetTree().Root.PropagateNotification((int) Node.NotificationWMCloseRequest);
        Callable.From(() => {
            node.GetTree().Quit();
        }).CallDeferred();
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

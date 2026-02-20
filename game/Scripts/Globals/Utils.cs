

using Godot;

public static partial class Utils {
    
    public static void QuitGame(this Node node) {
        node.GetTree().Root.PropagateNotification((int) Node.NotificationWMCloseRequest);
        Callable.From(() => {
            node.GetTree().Quit();
        }).CallDeferred();
    }

}

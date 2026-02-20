using Godot;
using System;

public partial class RyansTestScene : Node2D {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var a = GD.Load<ItemStack>("res://Resources/tests/ItemStack.tres");
        a.Notification((int) NotificationPostinitialize);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}

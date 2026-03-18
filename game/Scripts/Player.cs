using Godot;
using System;

public partial class Player : CharacterBody2D, IEntityHolder {

    [Export]
    public Entity entity {get; set;} = new();
    [Export]
    public float Speed = 300.0f;
    // public const float JumpVelocity = -400.0f;
    [Export]
    private Camera2D camera;

    public override void _PhysicsProcess(double delta) {
        Vector2 velocity = Velocity;

        // // Add the gravity.
        // if (!IsOnFloor()) {
        //     velocity += GetGravity() * (float) delta;
        // }

        // // Handle Jump.
        // if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
        //     velocity.Y = JumpVelocity;
        // }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.

        if (!this.IsMultiplayerAuthority()) {
        //     GD.PrintS(this.Multiplayer.GetUniqueId());
            return;
        }
        Vector2 direction = Input.GetVector("left", "right", "up", "down");
        if (direction.X == 0) {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }
        velocity.X = direction.X * Speed;
        velocity.Y = direction.Y * Speed;

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Ready() {
        base._Ready();
        var label = this.GetNode<Label>("%Label");
        label.Text = Name;
        if (this.IsMultiplayerAuthority()) {
            this.camera.MakeCurrent();
        }
    }

    public override void _EnterTree() {
        base._EnterTree();
        GD.PrintS("enter", int.Parse(this.Name.ToString().Replace("Player ", "")));
        this.SetMultiplayerAuthority(int.Parse(this.Name.ToString().Replace("Player ", "")));
    }
}

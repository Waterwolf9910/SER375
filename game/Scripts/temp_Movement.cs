using Godot;
using System;

public partial class temp_Movement : Sprite2D {
    [Export]
    public int speed {get; set; }= 5;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        if (Input.IsActionPressed("up")) {
            this.Position = new Vector2(this.Position.X, Math.Max(this.GetViewportRect().Position.Y, this.Position.Y - this.speed));
        }
        if (Input.IsActionPressed("down")) {
            this.Position = new Vector2(this.Position.X, Math.Min(this.GetViewportRect().End.Y, this.Position.Y + this.speed));
        }
        if (Input.IsActionPressed("left")) {
            this.Position = new Vector2(Math.Max(this.GetViewportRect().Position.X, this.Position.X - this.speed), this.Position.Y);
        }
        if (Input.IsActionPressed("right")) {
            this.Position = new Vector2(Math.Min(this.GetViewportRect().End.X, this.Position.X + this.speed), this.Position.Y);
        }
    }
}

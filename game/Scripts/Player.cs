using Godot;

public partial class Player: AnimatedSprite2D
{

	// Don't forget to rebuild the project so the editor knows about the new signal.

	[Signal]
	public delegate void HitEventHandler();
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
		Start(Vector2.Zero);
	}
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		// GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
    public override void _Process(double delta) {
        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("Right")) {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("Left")) {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("Down")) {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("Up")) {
            velocity.Y -= 1;
        }

        if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
			this.Play();
        } else {
            this.Stop();
        }
        Position += velocity * (float) delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
        if (velocity.X > 0) {
            this.Animation = "Right";
        } else if (velocity.X < 0) {
            this.Animation = "Left";
        } 
        else if (velocity.Y < 0) {
            this.Animation = "Back";
        } else if (velocity.Y > 0) {
            this.Animation = "Front";
        }
	}

	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	public Vector2 ScreenSize; // Size of the game window.

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		// GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
using Godot;

public partial class Player : CharacterBody2D {

    [Export] AnimatedSprite2D sprite;
    [Export] public Deck deck;

    // Don't forget to rebuild the project so the editor knows about the new signal.

    [Signal]
    public delegate void HitEventHandler();
    public override void _Ready() {
        ScreenSize = GetViewportRect().Size;
    }

    public override void _Process(double delta) {
        handleMovement(delta);

        if (Input.IsActionJustPressed("Interact") && current_enemy != null) {
            startBattle();
        }
    }

    public void handleMovement(double delta) {
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
            this.sprite.Play();
        } else {
            this.sprite.Stop();
        }
        Position += velocity * (float) delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
        if (velocity.X > 0) {
            this.sprite.Animation = "Right";
        } else if (velocity.X < 0) {
            this.sprite.Animation = "Left";
        } else if (velocity.Y < 0) {
            this.sprite.Animation = "Back";
        } else if (velocity.Y > 0) {
            this.sprite.Animation = "Front";
        }

    }

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    public Vector2 ScreenSize; // Size of the game window.

    public BattlableNPC current_enemy; // The enemy the player is currently battling, if any.
    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnAreaEntered(Area2D body) {
        if (body.GetParent() is BattlableNPC npc) {
            current_enemy = npc;
            GD.Print("Entered battle area of " + npc.Name);
        }
    }

    private void OnAreaExited(Area2D body) {
        if (body.GetParent() is BattlableNPC npc) {
            current_enemy = null;
            GD.Print("Exited battle area of " + npc.Name);
        }
    }

    public void startBattle() {

        //Battle Start Function Here
        this.switchToBattle(this.deck, current_enemy.deck);

        GD.Print("Battle Started!");
    }
}
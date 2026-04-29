using Godot;
using Godot.Collections;
using System.Linq;
using System.Threading.Tasks;

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
        if (in_interaction) {
            return;
        }
        if (Velocity.X > 0) {
            this.sprite.Animation = "Right";
        } else if (Velocity.X < 0) {
            this.sprite.Animation = "Left";
        } else if (Velocity.Y < 0) {
            this.sprite.Animation = "Back";
        } else if (Velocity.Y > 0) {
            this.sprite.Animation = "Front";
        }
        if (Input.IsActionJustPressed("Interact") && current_interactable != null && !in_interaction) {
            // await
            _ = interact();
        }
    }

    public override void _PhysicsProcess(double delta) {
        handleMovement(delta);
    }

    public void handleMovement(double delta) {
        if (in_interaction) {
            return;
        }
        var velocity = Velocity; // The player's movement vector.

        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
        if (direction != Vector2.Zero) {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
        } else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
        }

        if (velocity.Length() > 0) {
            // velocity = velocity.Normalized() * Speed;
            this.sprite.Play();
        } else {
            this.sprite.Stop();
        }
        // Position += velocity * (float) delta;
        // Position = new Vector2(
        //     x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
        //     y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        // );

        this.Velocity = velocity;
        this.MoveAndSlide();
    }

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    public Vector2 ScreenSize; // Size of the game window.

    public Interactable current_interactable; // The enemy the player is currently battling, if any.
    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnAreaEntered(Area2D body) {
        if (body.GetParent() is Interactable interactable) {
            current_interactable = interactable;
            GD.Print("Entered area of " + interactable.Name);
            if (interactable.activate_on_touch) {
                Callable.From(() => {
                    _ = interact();
                }).CallDeferred();
            }
        }
    }

    private void OnAreaExited(Area2D body) {
        if (body.GetParent() is Interactable interactable) {
            current_interactable = null;
            GD.Print("Exited area of " + interactable.Name);
        }
    }

    public bool in_interaction = false;

    public async Task interact() {
        in_interaction = true;

        // component name, raw name
        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> order = [];

        foreach (var component_name in current_interactable.components.Keys) {
            var split = component_name.Split(".");
            var name = split[0];
            var modifiers = split[1..]; // TODO

            if (modifiers.Length < 1) {
                var indexes = order.Select((v, i) => new System.Collections.Generic.KeyValuePair<string, int>(v.Value, i));
                var _obj = indexes.LastOrDefault(v => v.Key.Contains($"pre{name}"));
                if (_obj.Key == null) {
                    order.Add(new(name, component_name));
                    continue;
                }
                order.Insert(_obj.Value, new(name, component_name));
                continue;
                // order.Add(component_name);
            }

            foreach (var modifier in modifiers) {
                if (modifier.StartsWith("pre")) {
                    var indexes = order.Select((v, i) => new System.Collections.Generic.KeyValuePair<string, int>(v.Value, i));
                    var _obj = indexes.LastOrDefault(v => v.Key.Contains(name));
                    if (order.Count == 0) {
                        order.Add(new(name, component_name));
                        continue;
                    }
                    order.Insert(_obj.Value, new(name, component_name));
                } else if (modifier.StartsWith("post")) {
                    var indexes = order.Select((v, i) => new System.Collections.Generic.KeyValuePair<string, int>(v.Value, i));
                    var _obj = indexes.Last(v => v.Key.Contains(name));
                    if (order.Count == 0) {
                        order.Add(new(name, component_name));
                        continue;
                    }
                    order.Insert(_obj.Value + 1, new(name, component_name));
                }
            }
        }

        foreach (var component in order) {
            switch (component.Key) {
                case "dialogue": {
                    var convo = current_interactable.components[component.Value].As<Conversation>();
                    Dialogue.INSTANCE.runDialogue(convo);
                    await ToSignal(Dialogue.INSTANCE, Dialogue.SignalName.onDialogueEnd);
                    break;
                }
                case "battle": {
                    //TODO: Handle after battle events
                    //Battle Start Function Here
                    this.switchToBattle(this.deck, current_interactable.components[component.Value].As<Deck>());
                    GD.Print("Battle Started!");
                    break;
                }
                case "travel": {
                    var scene = current_interactable.components[component.Value].As<PackedScene>();
                    current_interactable.components["travel-current"] = true;
                    if (current_interactable.components.TryGetValue("travel-return_pos", out var return_pos)) {
                        this.ChangeScene(scene, true, return_pos.As<Vector2>());
                    } else {
                        this.ChangeScene(scene, true);
                    }
                    break;
                }
            }
        }

        Callable.From(() => {
            in_interaction = false;
        }).CallDeferred();

    }
}

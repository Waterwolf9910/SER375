using Godot;

public partial class Dialogue : CanvasLayer {
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static Dialogue INSTANCE { get; private set; }

    [Export] public TextureRect Portrait;
    [Export] public Label DialogueText;
    [Export] public Label CharacterName;
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Signal]
    delegate void onDialogueEndEventHandler();

    private Conversation current_conversation = new();
    private int current_msg = 0;

    public override void _Ready() {
        base._Ready();
        INSTANCE = this;
    }

    // public void SetDialogue(Texture2D image, string name, string text) {
    //     Portrait.Texture = image;
    //     CharacterName.Text = name;
    //     DialogueText.Text = text;
    // }

    public override void _UnhandledKeyInput(InputEvent @event) {
        if (@event is not InputEventKey key) {
            return;
        }
    }

    public override void _Process(double delta) {
        base._Process(delta);

        if (Input.IsActionJustPressed("Interact") && this.Visible) {
            nextMessage();
        }
    }

    public void nextMessage() {
        if (this.current_conversation.messages.Count <= this.current_msg) {
            this.Visible = false;
            this.EmitSignal(SignalName.onDialogueEnd);
            return;
        }
        var msg = this.current_conversation.messages[this.current_msg];
        if (this.current_conversation.images.TryGetValue(msg.image, out var texture)) {
            Portrait.Texture = texture;
        } else {
            Portrait.Texture = null;
        }
        this.CharacterName.Text = msg.character_name;
        this.DialogueText.Text = msg.message;
        this.current_msg++;
    }

    public void runDialogue(Conversation conversation) {
        this.current_msg = 0;
        this.current_conversation = conversation;
        nextMessage();
        this.Visible = true;
    }

    // From another script that has a reference to your Dialogue node:
    //var dlg = GetNode<Dialogue>("CanvasLayer/Control");

    //var portrait = GD.Load<Texture2D>("res://Assets/iris.png"); Path to portrait image
    //dlg.SetDialogue(portrait, "Iris", "*Character Text Here*"); Set text here
}

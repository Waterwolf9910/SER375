using Godot;

public partial class Dialogue : Control {
    [Export] public Sprite2D Portrait;
    [Export] public Label DialogueText;
    [Export] public Label CharacterName;

    public void SetDialogue(Texture2D image, string name, string text) {
        Portrait.Texture = image;
        CharacterName.Text = name;
        DialogueText.Text = text;
    }

    // From another script that has a reference to your Dialogue node:
    //var dlg = GetNode<Dialogue>("CanvasLayer/Control");

    //var portrait = GD.Load<Texture2D>("res://Assets/iris.png"); Path to portrait image
    //dlg.SetDialogue(portrait, "Iris", "*Character Text Here*"); Set text here
}
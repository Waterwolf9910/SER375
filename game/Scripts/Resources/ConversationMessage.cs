using Godot;

[GlobalClass]
public partial class ConversationMessage : Resource {
    
    [Export] public string image = "";

    [Export] public string message = "";

    [Export] public string character_name = "???";

    [Export] public bool instant_continue = false;
}

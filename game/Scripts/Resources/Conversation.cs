
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Conversation : Resource {
    
    [Export] public Array<ConversationMessage> messages = [];

    [Export] public Dictionary<string, Texture2D> images = [];
}

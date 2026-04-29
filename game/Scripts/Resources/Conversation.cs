
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Conversation : Resource {

    [Export] public Array<ConversationMessage> messages = [];

    // Other Conversation related data / events can be added here

}

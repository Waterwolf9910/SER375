using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Deck : Resource {
    [Export]
    Array<string> card_ids = [];
}

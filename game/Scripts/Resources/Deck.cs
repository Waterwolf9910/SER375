using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Deck : Resource {

    [Export] public string anti_hero_id = "iris";

    [Export]
    public Array<string> card_ids = [];

    [Export]
    public Array<string> item_ids = [];

}

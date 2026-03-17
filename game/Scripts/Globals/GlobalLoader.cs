

using Godot;
using Godot.Collections;
using System.Linq;

public partial class GlobalLoader : Node {
    
    // card_id, card
    [Export]
    private Dictionary<string, Card> internal_cards = [];

    // card_id, card
    [Export]
    private Dictionary<string, Item> internal_items = [];

    // mod_id -> card_id, card
    [Export]
    private Dictionary<string, Dictionary<string, Card>> modded_cards = [];

    // mod_id -> card_id, card
    [Export]
    private Dictionary<string, Dictionary<string, Item>> modded_items = [];

    // maybe combine instead to load modded resources.
    public void LoadModdedCards(string mod_id, Array<Card> cards) {}
    public void LoadModdedItems(string mod_id, Array<Item> items) {}

    // load our base cards here
    public override void _Ready() {
        base._Ready();
        var path = "res://Resources"; //think of names to place items, cards, etc. so we are not loading all of our resources at once
        var card_folder = DirAccess.Open(path);
        if (card_folder == null) {
            return;
        }
        LoadFromList(path, card_folder, internal_cards);
    }

    private void LoadFromList(string prev_path, DirAccess card_folder, Dictionary<string, Card> card_list) {
        foreach (string file in card_folder.GetFiles().Where(v => v.EndsWith(".tres") || v.EndsWith(".res"))) {
            var res_path = prev_path + "/" + file;
            var res = ResourceLoader.Load(res_path);
            if (res is Card card) {
                // GD.PrintS("a", card.id);
                internal_cards.Add(card.id, card);
            }
            if (res is Item item) {
                internal_items.Add(item.id, item);
            }
        }
        foreach (string dir in card_folder.GetDirectories()) {
            DirAccess access = DirAccess.Open(dir);
            if (access == null) {
                continue;
            }
            LoadFromList(prev_path + '/' + dir, access, card_list);
        }
    }

    private GlobalLoader() {}
}

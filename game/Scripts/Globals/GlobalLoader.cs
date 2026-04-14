

using Godot;
using Godot.Collections;
using System.Linq;

public partial class GlobalLoader : Node {

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static GlobalLoader INSTANCE {get; private set;}
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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
        INSTANCE = this;
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
            DirAccess access = DirAccess.Open(prev_path + '/' + dir);
            if (access == null) {
                continue;
            }
            LoadFromList(prev_path + '/' + dir, access, card_list);
        }
    }

    public Card? getCard(string id) {
        if (!this.internal_cards.TryGetValue(id, out var card)) {
            return null;
        }
        return card;
    }

    public Card? getCard(string modid, string id) {
        if (!this.modded_cards.TryGetValue(modid, out var cards) || !cards.TryGetValue(id, out var card) ) {
            return null;
        }
        return card;
    }

    public Item? getItem(string id) {
        if (!this.internal_items.TryGetValue(id, out var item)) {
            return null;
        }
        return item;
    }

    public Item? getItem(string modid, string id) {
        if (!this.modded_items.TryGetValue(modid, out var items) || !items.TryGetValue(id, out var item) ) {
            return null;
        }
        return item;
    }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private GlobalLoader() {}
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}


using Godot;
using Godot.Collections;

public partial class Battle : Node {

    RandomNumberGenerator rng = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export] public ActiveDeck p1_ad;
    [Export] public ActiveDeck p2_ad;
    [Export] public RichTextLabel status;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    PlayerInfo? player1;
    PlayerInfo? player2;
    Turn turn = Turn.Player;
    bool is_multiplayer = false;

    public Array<int> p1_alive = [-1, 0, 1, 2];
    public Array<int> p2_alive = [-1, 0, 1, 2];

    public override void _Ready() {
        base._Ready();
        this.p1_ad.cardClicked += onCardClick;
        this.p2_ad.cardClicked += onCardClick;
    }

    public override void _Process(double delta) {
        if (this.p1_alive.Count > 1 && this.p2_alive.Count < 1) {
            this.GetTree().ChangeSceneToPacked(previous_scene);
            return;
        }

        if (this.p1_alive.Count < 1 && this.p2_alive.Count > 1) {
            this.GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
            return;
        }
        handleTurn();
    }

    public void initBattle(long player1, long player2, long starting_id) {
        this.is_multiplayer = true;
        this.player1 = MultiplayerManager.INSTANCE.getPlayerInfo(player1);
        this.player2 = MultiplayerManager.INSTANCE.getPlayerInfo(player2);

        this.p1_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(this.player1!.deck.anti_hero_id));
        this.p2_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(this.player2!.deck.anti_hero_id));

        if (player1 == starting_id) {
            this.turn = Turn.Player;
        }
        GD.PrintS(this.player1?.player_name, this.player2?.player_name);
    }

    PackedScene previous_scene = new();

    public void initBattle(Deck player_deck, Deck npc_decks, PackedScene previous_scene) {
        this.is_multiplayer = false;
        this.p1_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(player_deck.anti_hero_id));
        this.p2_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(npc_decks.anti_hero_id));
        
        for (var i = 0; i < 3; ++i) {
            this.p1_ad[i].SetCard(GlobalLoader.INSTANCE.getCard(player_deck.card_ids.PickRandom()));
            this.p2_ad[i].SetCard(GlobalLoader.INSTANCE.getCard(npc_decks.card_ids.PickRandom()));
        }
        print("Battle Start");
        this.turn = Turn.Player;
        this.previous_scene = previous_scene;
    }

    public void handleTurn() {
        switch (this.turn) {
            case Turn.PEffects: {
                //TODO
                nextTurn();
                break;
            }
            case Turn.Enemy: {
                localEnemyTurn();
                break;
            }
            case Turn.EEffects: {
                //TODO
                nextTurn();
                break;
            }
        }
    }

    // player 1's only
    public Card? selected_card;

    public void onCardClick(bool double_click, int card_index, bool player_card) {
        if (turn != Turn.Player) {
            return;
        }
        // maybe some sort of inspect or dialogue on single click?

        if (double_click) {
            return;
        }
        Card card;
        
        if (player_card) {
            card = p1_ad[card_index].card!;
        } else {
            card = p2_ad[card_index].card!;
        }

        if (card!.hp > 0 && player_card) {
            print($"Selected [color=green]{card.name}[/color]");
            selected_card = card;
        } else {
            print($"Unable to select [color=gray]{card.name}[/color]");
        }

        if (selected_card == null || player_card) {
            return;
        }
        
        var results = selected_card.FlipAndApply(card);
        print($"Player Attacked [color=green]{card.name}[/color] with [color=red]{selected_card.name}[/color]");
        for (int i = 0; i < results.Count; ++i) {
            print($"    [[color=yellow]{selected_card.name}[/color] - Coin {i}]: {(results[i].isHeads ? "Heads" : "Tails")} - {results[i].effect.type} ({results[i].effect.value}).");
        }
        print($"     Card Health {selected_card.hp}/{selected_card.max_hp}");

        print($"[color=red]{card.name}[/color] has {card.hp}/{card.max_hp} hp");

        if (card.hp <= 0) {
            p2_alive.Remove(card_index);
            print($"[color=gree]{card.name} has died[/color]");
        }

        nextTurn();
    }

    public void nextTurn() {
        print("");
        switch (turn) {
            case Turn.Player: {
                this.turn = Turn.PEffects;
                break;
            }
            case Turn.PEffects: {
                this.turn = Turn.Enemy;
                print("Enemy Turn");
                break;
            }
            case Turn.Enemy: {
                this.turn = Turn.EEffects;
                break;
            }
            case Turn.EEffects: {
                this.turn = Turn.Player;
                print("Player Turn");
                break;
            }
        }
    }

    public void localEnemyTurn() {
        var attacking_card = p2_ad[p2_alive.PickRandom()].card!;
        var player_index = p1_alive.PickRandom();
        var player_target = p1_ad[player_index].card!;

        var results = attacking_card.FlipAndApply(player_target);
        print($"[color=red]NPC[/color] attacked [color=green]{player_target.name}[/color] with [color=red]{attacking_card.name}[/color]");
        for (int i = 0; i < results.Count; ++i) {
            print($"    [{attacking_card.name} - Coin {i+1}]: {(results[i].isHeads ? "Heads" : "Tails")} - {results[i].effect.type} ({results[i].effect.value})");
        }
        print($"    Card Health {attacking_card.hp}/{attacking_card.max_hp}");

        print($"[color=green]{player_target.name}[/color] has {player_target.hp} hp");
        if (player_target.hp <= 0) {
            p1_alive.Remove(player_index);
            print($"{player_target.name} has died");
        }

        nextTurn();
    }

    public void print(string msg) {
        GD.PrintRich(msg);
        status.Text += msg + '\n';
    }

    enum Turn {
        Player,
        /// <summary>
        /// effects on the player
        /// </summary>
        PEffects,
        Enemy,
        /// <summary>
        /// effects on an enemy
        /// </summary>
        EEffects
    }
}
public static class BattleExtension {
    private static PackedScene battle_scene = GD.Load<PackedScene>("res://Scenes/Test Scenes/battle.tscn");
    public static void switchToBattle(this Node node, Deck player, Deck npc) {
        var battle = battle_scene.Instantiate<Battle>();
        var scene = new PackedScene();
        scene.Pack(node.GetTree().CurrentScene);
        battle.initBattle(player, npc, scene);
        node.GetTree().ChangeSceneToNode(battle);
        //switch to battle scene and init battle with player info
    }
}

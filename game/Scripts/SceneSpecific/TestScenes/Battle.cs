
using Godot;

public partial class Battle : Node2D {

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Export] public ActiveDeck player1_ad;
    [Export] public ActiveDeck player2_ad;
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    PlayerInfo? player1;
    PlayerInfo? player2;
    bool my_turn = false;

    public override void _Ready() {
        base._Ready();
    }

    public void initBattle(long player1, long player2, long starting_id) {
        this.player1 = MultiplayerManager.INSTANCE.getPlayerInfo(player1);
        this.player2 = MultiplayerManager.INSTANCE.getPlayerInfo(player2);

        this.player1_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(this.player1!.deck.anti_hero_id));
        this.player2_ad.anti_hero.SetCard(GlobalLoader.INSTANCE.getCard(this.player2!.deck.anti_hero_id));

        if (player1 == starting_id) {
            this.my_turn = true;
        }
        GD.PrintS(this.player1?.player_name, this.player2?.player_name);
    }

}

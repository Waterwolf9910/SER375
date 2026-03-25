
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class Card : Resource {

    private string _id = "";

    [Export]
    public string id {
        get {
            return _id.Replace("card.", "");
        }
        private set {
            if (string.IsNullOrWhiteSpace(value)) {
                _id = "card." + name.Replace(" ", "_").ToLower();
                return;
            }
            _id = "card." + value;
        }
    }

    private string _name = "";

    [Export]
    public string name {
        get {
            return this._name;
        }
        private set {
            string old_gen_id = "card." + name.Replace(" ", "_").ToLower();
            if (string.IsNullOrWhiteSpace(this._id) || this._id == old_gen_id) {
                this._id = "card." + value.Replace(" ", "_").ToLower();
            }
            this._name = value;
        }
    }

    [Export]
    public string description { get; private set; } = "";

    /// <summary>
    /// Customized effects that will run on card play
    /// </summary>
    [Export]
    public Array<CardEffect> coinEffects { get; private set; } = [];

    [Export]
    public SpriteFrames frames { get; private set; } = new SpriteFrames();

    [Export]
    public Texture2D border { get; set; } = new();

    [Export]
    public double max_hp { get; private set; } = 0;

    [Export]
    public double hp { get; private set; } = 0;

    [Export]
    public double bonus_attack { get; private set; } = 0;

    [Export]
    public double current_defense { get; private set; } = 0;

    /// <summary>
    /// How many "coins" this card costs
    /// </summary>
    [Export]
    public int cost { get; set; } = 0;

    public int attackCard(Card a, Card b) {
        //this function allows a card to attack another card and reduce the defending cards hp 
        //by the attacking cards attack minus the defending cards defense
        var card_a_attack = 0.0;
        var card_b_defence = 0.0;

        // should use this result
        var _a = a.FlipCoins();
        var _b = b.FlipCoins();

        // var heads = new Godot.RandomNumberGenerator().RandiRange(0, a.coinEffects.Count);

        foreach (var effect in a.coinEffects) {
            if (effect.type == CardEffect.EffectType.Attack) {
                card_a_attack += effect.value;
            }
        }

        foreach (var effect in b.coinEffects) {
            if (effect.type == CardEffect.EffectType.Defense) {
                card_b_defence += effect.value;
            }
        }

        double damage = card_a_attack - card_b_defence;

        //damage cannot be negative, sets damage to 0 if such
        if (damage < 0) {
            damage = 0;
        } else {
            b.hp -= (int) damage;
        }
        //debug statement. can be commented out later
        Console.WriteLine($"Card {a.name} attacked {b.name} for {(int) damage} damage! {b.name} has {(int) b.hp} hp left.");
        return (int) b.hp;
    }


    public class FlipResult {
        public CardEffect effect;
        public bool isHeads;
    }

    /// <summary>
    /// Flips all coins on this card and returns each result.
    /// Heads = effect triggers, tails = effect is skipped.
    /// </summary>
    public List<FlipResult> FlipCoins() {
        var random = new Random();
        var results = new List<FlipResult>();

        foreach (var coinEffect in coinEffects) {
            var result = new FlipResult {
                effect = coinEffect,
                isHeads = random.Next(2) == 0   // 50/50
            };
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Flips all coins and applies heads effects to the target card.
    /// Returns the list of results so the caller can animate/display them.
    /// </summary>
    public List<FlipResult> FlipAndApply(Card self, Card target) {
        var results = FlipCoins();

        foreach (var result in results) {
            if (!result.isHeads)
                continue;  // Tails — skip

            switch (result.effect.type) {
                case CardEffect.EffectType.Attack:
                    target.attackCard(self, target);
                    break;
                case CardEffect.EffectType.Defense:
                    foreach (var effect in self.coinEffects) {
                        if (effect.type == CardEffect.EffectType.Defense) {
                            self.current_defense += effect.value;
                        }
                    }
                    // self.defense += result.effect.value;
                    break;
                case CardEffect.EffectType.Heal:
                    self.hp += result.effect.value;
                    break;
                case CardEffect.EffectType.Enhance:
                    if (result.effect.selection == Selection.Target) {
                        target.bonus_attack += result.effect.value;
                    }
                    break;
                    /*
                    case CoinEffect.EffectType.Custom:
                        // Custom script hook — handle via your existing actions system
                        GD.Print($"Custom effect triggered on {target.name}");
                        break;
                    */
            }

            GD.Print($"[{name}] Coin landed HEADS — {result.effect.type} ({result.effect.value}) applied!");
        }

        return results;  // Return so UI/animations can react to each flip
    }
    protected Card() {
    }
}

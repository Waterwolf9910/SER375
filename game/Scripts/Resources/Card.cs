
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class Card : Resource {

    private string _id = "";

    [Export]
    public string id {
        get => this._id.Replace("card.", "");
        private set {
            if (string.IsNullOrWhiteSpace(value)) {
                this._id = "card." + name.Replace(" ", "_").ToLower();
                this.EmitChanged();
                return;
            }
            this._id = "card." + value;
            this.EmitChanged();
        }
    }

    private string _name = "";

    [Export]
    public string name {
        get => this._name; private set {
            string old_gen_id = "card." + name.Replace(" ", "_").ToLower();
            if (string.IsNullOrWhiteSpace(this._id) || this._id == old_gen_id) {
                this._id = "card." + value.Replace(" ", "_").ToLower();
            }
            this._name = value;
            this.EmitChanged();
        }
    }

    public string _description = "";

    [Export]
    public string description {
        get => this._description; private set {
            this._description = value;
            this.EmitChanged();
        }
    }

    /// <summary>
    /// Customized effects that will run on card play
    /// </summary>
    [Export]
    public Array<CardEffect> coinEffects { get; private set; } = [];

    public SpriteFrames _frames = new();
    [Export]
    public SpriteFrames frames {
        get => this._frames;
        private set {
            this._frames = value;
            this._frames.Changed += this.EmitChanged;
            this.EmitChanged();
        }
    }
    // public AnimatedTexture frames { get; private set; } = new AnimatedTexture();

    public Texture2D _border = new();
    [Export]
    public Texture2D border {
        get => this._border; set {
            this._border = value;
            this.EmitChanged();
        }
    }


    public double _max_hp = 0;

    [Export]
    public double max_hp {
        get => this._max_hp; private set {
            this._max_hp = value;
            this.EmitChanged();
        }
    }

    public double _hp = 0;
    [Export]
    public double hp {
        get => this._hp; private set {
            this._hp = value;
            this.EmitChanged();
        }
    }

    public double _bonus_attack = 0;
    [Export]
    public double bonus_attack {
        get => this._bonus_attack; private set {
            this._bonus_attack = value;
            this.EmitChanged();
        }
    }

    public double _current_defense = 0;
    [Export]
    public double current_defense {
        get => this._current_defense; private set {
            this._current_defense = value;
            this.EmitChanged();
        }
    }

    public double _max_defense = 0;
    [Export]
    public double max_defense {
        get => this._max_defense; private set {
            this._max_defense = value;
            this.EmitChanged();
        }
    }

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

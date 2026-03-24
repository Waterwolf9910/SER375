
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class CoinEffect : Resource {
    public enum EffectType {
        Attack, Defense, Heal, Enhance
    }

    [Export] public EffectType type { get; set; } = EffectType.Attack;

    //The value applied when this coin lands heads (e.g. +10 attack)
    [Export] public double value { get; set; } = 0;

    //Optional script for custom effects, mirrors Card.actions
    //[Export] public Script customAction { get; set; } = null;
}

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
    public Array<Script> actions { get; private set; } = [];

    [Export]
    public Array<CoinEffect> coinEffects { get; private set; } = [];

    [Export]
    public SpriteFrames frames { get; private set; } = new SpriteFrames();

    [Export]
    public Texture2D border { get; set; } = new();

    // Although we have script effects above, we need to have the cards basic attack and defense additions, expecally for other people to make custom cards without scripting
    [Export]
    public double attack { get; private set; } = 0;

    [Export]
    public double defense { get; private set; } = 0;

    [Export]
    public double hp { get; private set; } = 0;

    [Export]
    public double coins { get; private set; } = 0;

    [Export]
    public Element element { get; private set; } = Element.None;

    /// <summary>
    /// How many "coins" this card costs
    /// </summary>
    [Export]
    public int cost { get; set; } = 0;


    public int attackCard(Card a, Card b) {
        //this function allows a card to attack another card and reduce the defending cards hp 
        //by the attacking cards attack minus the defending cards defense
        double damage = a.attack - b.defense;

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
        public CoinEffect effect;
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
                case CoinEffect.EffectType.Attack:
                    target.attackCard(self, target);
                    break;
                case CoinEffect.EffectType.Defense:
                    self.defense += result.effect.value;
                    break;
                case CoinEffect.EffectType.Heal:
                    self.hp += result.effect.value;
                    break;
                case CoinEffect.EffectType.Enhance:
                    target.attack += result.effect.value;
                    break;
                    /*case CoinEffect.EffectType.Custom:
                        // Custom script hook — handle via your existing actions system
                        GD.Print($"Custom effect triggered on {target.name}");
                        break;
                    */
            }

            GD.Print($"[{name}] Coin landed HEADS — {result.effect.type} ({result.effect.value}) applied to {target.name}");
        }

        return results;  // Return so UI/animations can react to each flip
    }
    protected Card() {
    }
}

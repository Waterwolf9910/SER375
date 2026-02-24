
using Godot;
using Godot.Collections;

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
    public string description {get; private set;} = "";

    /// <summary>
    /// Customized effects that will run on card play
    /// </summary>
    [Export]
    public Array<Script> actions {get; private set;} = [];

    [Export]
    public SpriteFrames frames {get; private set;} = new SpriteFrames();

    [Export]
    public Texture2D border {get; set;} = new ();

    // Although we have script effects above, we need to have the cards basic attack and defense additions, expecally for other people to make custom cards without scripting
    [Export]
    public double attack {get; private set;} = 0;

    [Export]
    public double defense {get; private set;} = 0;

    [Export]
    public Element element {get; private set;} = Element.None;

    /// <summary>
    /// How many "coins" this card costs
    /// </summary>
    [Export]
    public int cost {get; set;}= 0;
    
    protected Card() {}
}

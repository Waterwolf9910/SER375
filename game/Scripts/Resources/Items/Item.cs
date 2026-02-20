using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class Item : Resource {

    public static string ID_BASE {get;} = "item.";
    public static int BASE_MAX_STACK_SIZE {get;} = 3;
    
    private string _id = "";
    [Export]
    public string id {
        get {
            return this._id.Replace(ID_BASE, "");
        }
        private set {
            if (string.IsNullOrWhiteSpace(value)) {
                this._id = ID_BASE + this._name.Replace(" ", "_");
                return;
            }
            this._id = ID_BASE + value;
        }
    }

    private string _name = "";
    [Export]
    public string name {
        get {
            return this._name;
        }
        private set {
            string old_gen_id = "item." + name.Replace(" ", "_").ToLower();
            if (string.IsNullOrWhiteSpace(this._id) || this._id == old_gen_id) {
                this._id = "item." + value.Replace(" ", "_").ToLower();
            }
            this._name = value;
        }
    }

    [Export]
    public string description {get; private set;} = "";

    [Export]
    public int max_stack_size {get; private set;} = BASE_MAX_STACK_SIZE;

    public bool stackable => max_stack_size > 1;

    [Export]
    public Array<Script> scripts {get; private set;} = [];

    // Item should never need to be extended, any runtime access or use will be done within ItemStack
    private Item() {}

}

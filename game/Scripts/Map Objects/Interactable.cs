
using Godot;
using Godot.Collections;

public partial class Interactable : Node2D {
    
    [Export] public Dictionary<string, Variant> components = [];

    [Export] public bool activate_on_touch = false;
}

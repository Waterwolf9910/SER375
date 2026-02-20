
using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class ItemStack : Resource {

    [Export]
    public Item? item { get; protected set; }

    private int _count = 1;

    [Export(PropertyHint.Range, "1,1,or_greater")]
    public int count {
        get => _count;
        protected set => this.setCount(value);
    }

    private string _display_name = "";
    [Export]
    public string display_name {
        get => string.IsNullOrEmpty(_display_name) ? item?.name ?? "" : _display_name;
        set => _display_name = value;
    }


    // prob unnecessary
    [Export]
    public Godot.Collections.Dictionary<string, Variant> custom_data = [];

    protected void setCount(int count) {
        _count = Math.Min(Math.Max(0, count), item?.max_stack_size ?? 1);
    }

    public bool isStackable(ItemStack stack) {
        if (stack == null) {
            return false;
        }
        return stack.item?.id == this.item?.id && (this.item?.stackable ?? false) && stack.display_name == this.display_name;
    }

    public int merge(ItemStack? stack) {
        if (stack == null) {
            return 0;
        }
        if (!isStackable(stack)) {
            return 0;
        }
        int result = this.count + stack.count;
        int remainder = 0;
        while (result > this.item?.max_stack_size) {
            remainder += result - this.item.max_stack_size;
        }
        this.count = result - remainder;
        return stack.count = remainder;
    }

    public ItemStack split() {
        ItemStack stack = of(this, (int) Math.Ceiling(this.count / 2f));
        this.count = (int) Math.Floor(this.count / 2f);
        return stack;
    }

    public ItemStack take(int amount) {
        ItemStack stack = of(this, amount > this.count ? amount + (this.count - amount) : amount);
        this.count -= amount;
        return stack;
    }

    public void takeOne(ItemStack stack) {
        if (!this.isStackable(stack) || this.count < 1 || stack.count >= item?.max_stack_size) {
            return;
        }
        
        this.count--;
        stack.count++;
    }

    public static ItemStack of(Item item, int count = 1) {
        return new ItemStack(item) {
            count = Math.Min(count, item.max_stack_size)
        };
    }

    public static ItemStack of(ItemStack stack, int count = 1) {
        ItemStack nstack = (ItemStack) stack.Duplicate(true);
        nstack.count = Math.Min(count, stack.item?.max_stack_size ?? 1);
        return nstack;
    }

    public override bool Equals(object? obj) {
        if (obj == null) {
            return false;
        }
        if (obj is Item item) {
            return item == this.item;
        }
        if (obj is not ItemStack stack) {
            return false;
        }
        return stack == this || (
            stack.item == this.item &&
            stack._display_name == this._display_name
        );
        
    }

    public override int GetHashCode() {
        return HashCode.Combine(this.item?.id, this.display_name);
    }

    public void use(Entity target) {
        if (this.item == null) {
            return;
        }
        var sorted = this.item.scripts.Where(s => s is IStackAction).OrderByDescending(s => ((IStackAction)s).priority());
        foreach (IStackAction action in sorted) {
            if (action.use(this, target) == IStackAction.Result.Cancel) {
                break;
            }
        }
    }

    public ItemStack(Item item) {
        this.item = item;
    }

    protected ItemStack() {}

}

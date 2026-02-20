
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class Inventory : Resource, IEnumerable<ItemStack?> {
    Array<ItemStack?> slots = [];

    [Export]
    public int size {
        get => this.slots.Count;
        set {
            this.slots.Resize(value);
            this.NotifyPropertyListChanged();
        }
    }

    /// <summary>
    /// Called on size changes
    /// </summary>
    [Signal]
    public delegate void on_changeEventHandler();

    /// <summary>
    /// Creates a new inventory from an array of item stacks
    /// </summary>
    /// <param name="items">The array of items</param>-+-
    public Inventory(IList<ItemStack> items) : this() {
        foreach (var item in items) {
            this.slots.Add(item);
        }
    }

    public ItemStack? this[int index] {
        get => this.slots[index];
    }

    /// <summary>
    /// Populates slots with items
    /// </summary>
    /// <param name="items">The list of items to add</param>
    public void setSlots(IList<ItemStack?> items) {
        if (items == null) {
            return;
        }
        // foreach (Slot slot in slots) {
        //     ItemStack item = slot.getItem();
        //     item?.Free();
        // }
        for (int i = 0; i < this.size; ++i) {
            if (i >= items.Count) {
                this.slots[i] = null;
                continue;
            }
            if (items[i] != null && items[i]!.item != null) {
                this.slots[i] = (ItemStack?) items[i]?.Duplicate();
            }
        }
    }

    /// <summary>
    /// Adds a stack into the availble spots in an inventory
    /// </summary>
    /// <param name="stack">The stack to add</param>
    /// <returns>The remaining items in the stack</returns>
    public int addStack(ref ItemStack? stack) {
        if (stack == null) {
            return 0;
        }

        while (stack.count != 0) {
            for (int slot = 0; slot < this.slots.Count && stack.count > 0; ++slot) {
                ItemStack? inv_stack = this.slots[slot];

                if (inv_stack == null || !inv_stack.isStackable(stack) && inv_stack.count > inv_stack.item!.max_stack_size) {
                    continue;
                }
                stackItem(slot, stack);
            }
        }

        if (stack.count == 0) {
            stack = null;
            return 0;
        }

        return stack.count;
    }

    /// <summary>
    /// Adds or sets a stack of items in the slot
    /// </summary>
    /// <param name="slot">the slot to offect</param>
    /// <param name="stack">the stack to attempt add</param>
    /// <param name="merge_only">Only merge if the item exists in this slot and can be</param>
    /// <returns>How many items were stacked</returns>
    public int stackItem(int slot, ItemStack? stack, bool merge_only = false) {
        if (stack == null) {
            return 0;
        }
        ItemStack? inv_slot = this.slots[slot];
        if (inv_slot == null) {
            if (!merge_only) {
                this.setStack(slot, stack.take(int.MaxValue));
            }
            return merge_only ? stack.count : 0;
        }
        if (!inv_slot.isStackable(stack)) {
            return stack.count;
        }
        int result = inv_slot.merge(stack);
        return result;
    }

    public ItemStack? swap(int slot, ItemStack stack) {
        ItemStack? old = this.slots[slot];
        this.slots[slot] = stack;
        return old;
    }

    public ItemStack? getStack(int slot) {
        return this.slots[slot];
    }
    
    public void setStack(int slot, ItemStack? stack) {
        this.slots[slot] = stack;
    }

    /// <summary>
    /// Creates a new inventory of size
    /// </summary>
    /// <param name="size">Size of the inventory</param>
    /// <returns>The created inventory</returns>
    public static Inventory of(int size) {
        Array<ItemStack> array = [];
        array.Resize(size);

        return new(array);
    }

    /// <summary>
    /// Creates a new inventory of size filled with stack
    /// </summary>
    /// <param name="size">Size of the inventory</param>
    /// <param name="stack">The item stack to fill the inventory</param>
    /// <returns>The created inventory</returns>
    public static Inventory of(int size, ItemStack stack) {
        Array<ItemStack> array = [];
        array.Resize(size);
        array.Fill(stack);

        return new(array);
    }

    public IEnumerator<ItemStack?> GetEnumerator() {
        return this.slots.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    protected Inventory() {}
}

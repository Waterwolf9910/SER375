public interface IStackAction {

    enum Result {
        /// <summary>
        /// Continue to other stack user
        /// </summary>
        Continue,
        /// <summary>
        /// Stop execution at this stack user
        /// </summary>
        Cancel
    }

    public int priority() {
        return 0;
    }

    /// <summary>
    /// The function to call on the stack
    /// </summary>
    /// <param name="stack">The stack that is being used</param>
    /// <param name="target">The target of the item</param>
    /// <returns>If we should continue to other stack users</returns>
    public Result use(ItemStack stack, Entity target);

}

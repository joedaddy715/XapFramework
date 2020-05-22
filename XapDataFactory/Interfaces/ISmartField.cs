namespace Xap.Data.Factory.Interfaces {
    public interface ISmartField {
        /// <summary>
        /// Sets or gets the text representation
        /// of the value.
        /// </summary>
        /// <remarks>
        /// An empty string indicates an empty value.
        /// </remarks>
        string Text { get; set; }
        /// <summary>
        /// Gets a value indicating whether the
        /// field's value is empty.
        /// </summary>
        bool IsEmpty { get; }
    }
}

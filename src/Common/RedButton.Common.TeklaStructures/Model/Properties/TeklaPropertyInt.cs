namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public sealed class TeklaPropertyInt
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public TeklaPropertyInt(string name, int? value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            if (Value == null)
                return string.Empty;

            return Value.Value.ToString();
        }
    }
}

namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public sealed class TeklaPropertyString
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        public TeklaPropertyString(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

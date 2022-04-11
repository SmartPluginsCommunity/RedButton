namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public sealed class TeklaPropertyString : TeklaPropertyBase
    {
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public TeklaPropertyString(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

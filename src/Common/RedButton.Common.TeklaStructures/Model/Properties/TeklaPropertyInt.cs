namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public sealed class TeklaPropertyInt : TeklaPropertyBase
    {
        /// <summary>
        /// Value
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
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

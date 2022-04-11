using System;

namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public sealed class TeklaPropertyDouble : TeklaPropertyBase
    {
        /// <summary>
        /// Value
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public TeklaPropertyDouble(string name, double? value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            if (Value == null)
                return string.Empty;

            var v = Math.Round(Value.Value, 3);
            return v.ToString();
        }
    }
}

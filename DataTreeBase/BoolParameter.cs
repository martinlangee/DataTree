namespace DataTreeBase
{
    public sealed class BoolParameter : DataTreeParameter<bool>
    {
        public BoolParameter(DataTreeContainer parent, string id, string name, bool defaultValue = false)
            : base(parent, id, name, defaultValue)
        {
        }

        public override string AsString
        {
            get { return Value.ToString(); }
            internal set
            {
                bool boolVal;
                if (bool.TryParse(value, out boolVal))
                    Value = boolVal;
            }
        }
    }
}

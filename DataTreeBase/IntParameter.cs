namespace DataTreeBase
{
    public sealed class IntParameter : DataTreeParameter<int>
    {
        public IntParameter(DataTreeContainer parent, string id, string name, int defaultValue)
            : base(parent, id, name, defaultValue)
        {
        }

        public override string AsString
        {
            get { return Value.ToString(); }
            internal set
            {
                int intVal;
                if (int.TryParse(value, out intVal))
                    Value = intVal;
            }
        }
    }
}

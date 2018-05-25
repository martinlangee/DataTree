using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTreeBase
{
    public abstract class DataTreeParameter<T>: DataTreeParameterBase
    {
        private T _bufferedValue;

        public DataTreeParameter(DataTreeContainer parent, string id, string name, T defaultValue)
            : base(parent, id, name)
        {
            Value = defaultValue;
            _bufferedValue = defaultValue;
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (IsEqualValue(value, _value)) return;

                _value = value;
                OnChanged(this);
            } 
        }

        public event Action<DataTreeParameter<T>> OnChanged = parameter => {};

        public override bool IsModified => !IsEqualValue(Value, _bufferedValue);

        public override void ResetModified()
        {
            _bufferedValue = Value;
        }

        public override void Restore()
        {
            Value = _bufferedValue;
        }

        protected abstract bool IsEqualValue(T value, T bufferedValue);
    }
}

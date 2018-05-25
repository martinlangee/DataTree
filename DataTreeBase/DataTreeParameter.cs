using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTreeBase
{
    public abstract class DataTreeParameter<T>: DataTreeParameterBase
    {
        private T _value;
        private T _bufferedValue;

        public DataTreeParameter(DataTreeContainer parent, string id, string name, T defaultValue)
            : base(parent, id, name)
        {
            _value = defaultValue;
            _bufferedValue = defaultValue;
        }

        protected void FireOnChanged()
        {
            OnChanged(this);
        }


        public virtual T Value
        {
            get { return _value; }
            set
            {
                if (IsEqualValue(value, _value)) return;

                _value = value;
                FireOnChanged();
            } 
        }

        public event Action<DataTreeParameterBase> OnChanged = parameter => {};

        public override bool IsModified => !IsEqualValue(Value, _bufferedValue);

        public override void ResetModified()
        {
            _bufferedValue = Value;
        }

        public override void Restore()
        {
            Value = _bufferedValue;
        }

        protected virtual bool IsEqualValue(T value1, T value2)
        {
            return Comparer<T>.Default.Compare(value1, value2) == 0;
        }
    }
}

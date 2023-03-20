using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;

public class ObservableArray<T>
{
    public int Length => _elements.Length;

    public event EventHandler<ArrayChangedEventArgs<T>>? ElementChanged;

    private T[] _elements;


    public ObservableArray(int size)
    {
        _elements = new T[size];
    }

    public T this[int index]
    {
        get
        {
            return _elements[index];
        }
        set
        {
            T oldValue = _elements[index];
            _elements[index] = value;
            OnElementChanged(index, oldValue, value);
        }
    }

    protected virtual void OnElementChanged(int index, T oldValue, T newValue)
    {
        ElementChanged?.Invoke(this, new ArrayChangedEventArgs<T>(index, oldValue, newValue));
    }
}

public class ArrayChangedEventArgs<T> : EventArgs
{
    public int Index { get; }
    public T OldValue { get; }
    public T NewValue { get; }

    public ArrayChangedEventArgs(int index, T oldValue, T newValue)
    {
        Index = index;
        OldValue = oldValue;
        NewValue = newValue;
    }
}


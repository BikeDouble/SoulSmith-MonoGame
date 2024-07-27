
using System;
using System.Collections;
using System.Collections.Generic;

public partial class DropOutStack<T> : IEnumerable<T>
{
    private T[] _values;
    private int _top;
    private int _valuesContained;
    private int _capacity;

    public DropOutStack(int capacity)
    {
        _capacity = capacity;
        _values = new T[capacity]; 
    }

    public void Push(T value)
    {
        if (_valuesContained < _capacity)
        {
            _valuesContained++;
        }
        _values[_top] = value;
        _top = (_top + 1) % _capacity;
    }

    public T Pop()
    {
        if (_valuesContained > 0) 
        {
            _valuesContained--;
        }
        _top = (_capacity + _top - 1) % _capacity;
        return _values[_top];
    }

    public T Peek()
    {
        return _values[(_capacity + _top - 1) % _capacity];
    }

    public bool IsEmpty()
    {
        return _valuesContained == 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _valuesContained; i++)
        {
            int index = (_capacity + _top - 1 - i) % _capacity;
            yield return _values[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

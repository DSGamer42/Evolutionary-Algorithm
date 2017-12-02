using System;
using System.Collections.Generic;

namespace Evolution
{
    public class SimpleRatedPicker<T>
    {
        private readonly List<KeyValuePair<T,int>> _content = new List<KeyValuePair<T,int>>();

        private int _counter = 0;
        private Random r;

        public SimpleRatedPicker(Random random)
        {
            r = random;
        }

        public SimpleRatedPicker(): this(new Random()){}

        public void Add(int chance, T item)
        {
            _counter += chance;
            _content.Add(new KeyValuePair<T, int>(item, _counter));
        }

        public T Pick()
        {
            if(_content.Count == 0)
                throw new PickerIsEmptyException();
            if (_counter == 0)
                throw new UnableToPickException("All Chances Zero");

            for (int i = 0; i < _content.Count; i++)
            {
                if (_content[i].Value < r.Next(0, _counter + 1))
                    return _content[i].Key;
            }
            throw new UnableToPickException();
        }

        public void Clear()
        {
            _content.Clear();
            _counter = 0;
        }
    }

    [Serializable]
    internal class UnableToPickException : Exception
    {
        public UnableToPickException(){}

        public UnableToPickException(string message) : base(message){}
    }

    [Serializable]
    internal class PickerIsEmptyException : Exception {}
}

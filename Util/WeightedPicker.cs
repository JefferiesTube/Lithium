namespace Lithium.Util
{
    public class PickerEmptyException : Exception { }

    public class WeightedPicker<T>
    {
        private readonly Random _random;
        private bool _hasChanged;
        private readonly Dictionary<T, float> _dictionary = new Dictionary<T, float>();
        public int Count => _dictionary.Count;
        private double _totalWeight;

        public float this[T key]
        {
            get => _dictionary[key];

            set
            {
                _dictionary[key] = value;
                _hasChanged = true;
            }
        }

        public WeightedPicker(Random random)
        {
            _random = random;
        }

        public WeightedPicker() : this(new Random(Guid.NewGuid().GetHashCode())) { }

        public T Pick()
        {
            if (_hasChanged)
                Rebuild();

            float randomPercentage = _random.Next((int)(_totalWeight * 100)) / 100f;

            return PickManually(randomPercentage);
        }

        public T PickManually(float value)
        {
            if (_hasChanged)
                Rebuild();

            value = Math.Min((float)_totalWeight, (Math.Max(0, value)));

            float accumulate = 0;
            KeyValuePair<T, float>? last = null;

            foreach (KeyValuePair<T, float> pair in _dictionary)
            {
                if (accumulate > value)
                    break;
                last = pair;
                accumulate += pair.Value;
            }

            if (!last.HasValue)
                throw new PickerEmptyException();

            return last.Value.Key;
        }

        private void Rebuild()
        {
            _totalWeight = _dictionary.Values.Sum();
            _hasChanged = false;
        }

        public void Add(T value, float weight)
        {
            _dictionary.Add(value, weight);
            _hasChanged = true;
        }

        public void AddRange(IEnumerable<KeyValuePair<T, float>> entries)
        {
            foreach (KeyValuePair<T, float> entry in entries)
            {
                _dictionary.Add(entry.Key, entry.Value);
            }
            _hasChanged = true;
        }

        public bool Remove(T key)
        {
            _hasChanged = true;
            return _dictionary.Remove(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
            _hasChanged = true;
        }
    }
}

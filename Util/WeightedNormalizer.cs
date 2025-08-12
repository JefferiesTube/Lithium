namespace Lithium.Util
{
    public class WeightedNormalizer
    {
        private readonly List<float> _weights = [];
        private readonly List<float> _values = [];
        private readonly List<float> _cdf = [];
        private bool _isInitialized;

        public void Add(float weight, float value)
        {
            if (weight < 0f)
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be non-negative.");

            _weights.Add(weight);
            _values.Add(value);
            _isInitialized = false;
        }

        private void Initialize()
        {
            _cdf.Clear();
            float totalWeight = _weights.Sum();

            if (totalWeight == 0f)
            {
                // Fallback: assume a single neutral value if available,
                // otherwise default to 1.0 to avoid downstream zeros.
                _cdf.Clear();
                _cdf.Add(1f);
                _isInitialized = true;
                return;
            }

            float cumulative = 0f;
            foreach (float w in _weights)
            {
                cumulative += w / totalWeight;
                _cdf.Add(cumulative);
            }

            _isInitialized = true;
        }

        public float Evaluate(float n)
        {
            if (!_isInitialized)
                Initialize();

            if (_values.Count == 0)
                throw new InvalidOperationException("No values added to normalize.");

            switch (n)
            {
                case <= 0f:
                    return _values[0];
                case >= 1f:
                    return _values[^1];
            }

            int idx = _cdf.FindIndex(x => x > n);
            switch (idx)
            {
                case -1:
                    return _values[^1];
                case 0:
                    return _values[0];
            }

            int lowerIdx = idx - 1;
            float lowerCdf = _cdf[lowerIdx];
            float upperCdf = _cdf[idx];
            float t = (n - lowerCdf) / (upperCdf - lowerCdf);

            float lowerVal = _values[lowerIdx];
            float upperVal = _values[idx];

            return lowerVal + (upperVal - lowerVal) * t;
        }
    }
}

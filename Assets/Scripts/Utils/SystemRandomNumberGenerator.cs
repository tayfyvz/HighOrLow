using System;

namespace Utils
{
    public class SystemRandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly Random _random;
    
        public SystemRandomNumberGenerator(int seed = 0)
        {
            _random = seed == 0 ? new Random() : new Random(seed);
        }
    
        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
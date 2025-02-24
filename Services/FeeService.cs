using System;
using System.Threading;

namespace CargoPayAPI.Services
{
    public sealed class FeeService
    {
        private static readonly Lazy<FeeService> _instance = new(() => new FeeService());
        private readonly Random _random;
        private decimal _currentFee;
        private readonly Timer _timer;

        private FeeService()
        {
            _random = new Random();
            _currentFee = GetRandomFee(); 
            _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1)); 
        }

        public static FeeService Instance => _instance.Value;

        public decimal GetCurrentFee()
        {
            return _currentFee;
        }

        private void UpdateFee(object state)
        {
            decimal newMultiplier = (decimal)_random.NextDouble() * 2;
            _currentFee *= newMultiplier;            
        }

        private decimal GetRandomFee()
        {
            return (decimal)_random.NextDouble() * 2;
        }
    }
}

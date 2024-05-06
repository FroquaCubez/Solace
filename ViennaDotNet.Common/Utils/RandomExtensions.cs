namespace ViennaDotNet.Common.Utils
{
    public static class RandomExtensions
    {
        public static float NextSingle(this Random random, float min, float max)
        {
            if (min >= max)
                throw new ArgumentOutOfRangeException("min", "Minimum value must be less than maximum value.");

            double range = max - min;
            double sample = random.NextDouble() * range;
            return (float)(sample + min);
        }
    }
}

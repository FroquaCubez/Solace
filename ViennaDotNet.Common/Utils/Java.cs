namespace ViennaDotNet.Common.Utils
{
    public class Java
    {
        private Java()
        {

        }

        public static class IntStream
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="start">Inclusive</param>
            /// <param name="end">Exclusive</param>
            /// <returns></returns>
            public static IEnumerable<int> Range(int start, int end)
            {
                if (start > end)
                    throw new ArgumentException();

                for (int i = start; i < end; i++)
                    yield return i;
            }
        }
    }
}

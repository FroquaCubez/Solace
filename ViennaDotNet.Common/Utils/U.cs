using Uma.Uuid;

namespace ViennaDotNet.Common.Utils
{
    public static class U
    {
        private static IUuidGenerator uuidGenerator = new Version4Generator();

        public static Uuid RandomUuid()
            => uuidGenerator.NewUuid();

        public static long CurrentTimeMillis()
            => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

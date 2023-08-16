
namespace Common
{
    public static class KeysForCache
    {
        public static string getAdvertiseForShowKey(this int key)
        {
            return $"Advertise-{key}";
        }
    }
}

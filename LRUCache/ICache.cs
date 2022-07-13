namespace LRUCache
{
    public interface ICache<K, V>
    {
        V Get(K key);

        void Put(K key, V value);
    }
}

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IRepository<K,T> where T : class
    {
        public Task<T> Add(T item);
        public Task<T> DeleteByKey(K key);
        public Task<T> Update(T item);
        public Task<T> GetByKey(K key);
        public Task<IEnumerable<T>> GetAll();
    }
}

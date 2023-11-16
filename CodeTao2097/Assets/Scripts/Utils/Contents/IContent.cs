namespace CodeTao
{
    public interface IContent<T> where T : IContent<T>
    {
        public IContainer<T> Container { get; set; } 
        
        public bool AddToContainer(IContainer<T> container)
        {
            bool result = container.AddContent(this);
            if (result)
                Container = container;
            return result;
        }
        
        public bool RemoveFromContainer(IContainer<T> container)
        {
            bool result = container.RemoveContent(this);
            if (result)
                Container = null;
            return result;
        }
    }
}
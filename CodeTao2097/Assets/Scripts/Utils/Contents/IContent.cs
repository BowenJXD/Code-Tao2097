using System;

namespace CodeTao
{
    public interface IContent<T> where T : IContent<T>
    {
        public IContainer<T> Container { get; set; }

        public Action<IContent<T>> AddAfter { get; set; }
        
        public Action<IContent<T>> RemoveAfter { get; set; }

        public bool AddToContainer(IContainer<T> container)
        {
            bool result = container.AddContent(this);
            if (result)
            {
                Container = container;
                AddAfter?.Invoke(this);
            }

            return result;
        }
        
        public bool RemoveFromContainer(IContainer<T> container)
        {
            bool result = container.RemoveContent(this);
            if (result)
            {
                Container = null;
                RemoveAfter?.Invoke(this);
            }

            return result;
        }
    }
}
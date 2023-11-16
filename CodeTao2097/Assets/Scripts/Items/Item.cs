using QFramework;

namespace CodeTao
{
    public class Item : ViewController, IContent<Item>
    {
        public IContainer<Item> Container { get; set; }
    }
}
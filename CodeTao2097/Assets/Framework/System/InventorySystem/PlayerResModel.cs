namespace QFramework
{
    public interface IPlayerResModel : IModel
    {
        IBindableProperty<int> CoinNum { get; }
    }
    public class PlayerResModel : AbstractModel, IPlayerResModel
    {
        public IBindableProperty<int> CoinNum { get; } = new BindableProperty<int>(10);

        protected override void OnInit()
        {

        }
    }
}
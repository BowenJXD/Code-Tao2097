using QFramework;

public interface IPlayerModel : IModel
{
    IBindableProperty<int> HP { get; }
}
public class PlayerModel : AbstractModel, IPlayerModel
{
    IBindableProperty<int> IPlayerModel.HP { get; } = new BindableProperty<int>(10);

    protected override void OnInit()
    {

    }
}
using QFramework;
using QFramework.EquipmentSystem;
using UnityEngine;

namespace Framework
{
    public class QF_Game : Architecture<QF_Game>
    {
        protected override void Init()
        {
            RegisterUtility<IStorage>(new JsonStorage());
            RegisterUtility<IJsonConfig>(new LoadConfig());

            RegisterModel<IPlayerResModel>(new PlayerResModel());

            RegisterSystem<IAudioMgrSystem>(new AudioMgrSystem());
            RegisterSystem<IArchiveSystem>(new ArchiveSystem());
            RegisterSystem<IDelayTimeSystem>(new DelayTimeSystem());
            RegisterSystem<IInputDeviceMgrSystem>(new InputDeviceMgrSystem());
            RegisterSystem<IMessageSystem>(new MessageSystem());
            RegisterSystem<IObjectPoolSystem>(new ObjectPoolSystem());
            RegisterSystem<ISceneMgrSystem>(new SceneMgrSystem());
            RegisterSystem<IUGUISystem>(new UGUISystem());
            RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
            RegisterSystem<IInventorySystem>(new InventorySystem());
            RegisterSystem<IEquipmentFactorySystem>(new EquipmentFactorySystem());

            RegisterSystem<GameMgrSystem>(new GameMgrSystem());

            RegisterModel<IPlayerModel>(new PlayerModel());
            /*RegisterSystem<IGunSystem>(new GunSystem());*/
            RegisterSystem<IBuffSystem>(new BuffSystem());

            /*RegisterModel<IBagConfigModel>(Resources.Load<BagConfigModel>("SO_Data/SO_BagConfigModel"));
            RegisterModel<ITestModel>(Resources.Load<TestModel>("SO_Data/SO_TestModel"));
            RegisterSystem<ITestSystem>(Resources.Load<TestSystem>("SO_Data/SO_TestSystem"));*/
        }
    }
    
    public class QF_GameController : MonoBehaviour, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture() => QF_Game.Interface;
    }
    
    public class QF_UIController : UIPanel, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture() => QF_Game.Interface;
    }
}
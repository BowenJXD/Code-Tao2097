using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    // ## 最后更新: 2023.11.06 18:36
    // ## 需要修改到架构中的几个部分
    // public partial interface IArchitecture
    // public abstract partial class Architecture<T>
    // public interface ICommand : ICommandContainer
    // public interface IQuery<TResult> : IQuery

    // 里氏替换原则 因为我扩展了两种方式的命令接口 所以为了实现池子 我用一个父接口来存储两种类型的命令
    // 为了防止重复代码 我将原ICommand中的规则 转到了父接口中 让两个派生的命令接口继承父类接口
    public interface ICommandContainer : ICanSetArchitecture,
        ICanGetSystem, ICanGetModel, ICanGetUtility, ICanSendEvent, ICanSendCommand, ICanSendQuery{ }
    // 派生出一个带参数的命令接口 继承命令接口容器 因为父容器继承了规则 所以他也拥有了相应的规则
    public interface IParameterCommand<TP> : ICommandContainer { void Execute(TP parameter); }
    // 这个 IQuery 作为所有类型 Query 的容器 将规则转移到父容器
    public interface IQuery : ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanSendQuery { }
    // 扩展一个带参数的查询接口
    public interface IParameterQuery<TResult, TParameter> : IQuery { TResult Do(TParameter parameter); }
    // 用于设置参数的泛型接口
    public interface ICanSetCommandData<T> { void Set(T data); }
    public interface ICommandData { Type Type { get; set; } }
    public interface ICommandData<T> : ICommandData { T Info { get; } }
    // 重写一个抽象的查询基类
    public abstract class AbstractQuery<TResult, TParameter> : IParameterQuery<TResult, TParameter>
    {
        protected abstract TResult OnDo(TParameter parameter);
        TResult IParameterQuery<TResult, TParameter>.Do(TParameter parameter) => OnDo(parameter);

        private IArchitecture mArchitecture;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;
    }
    // 重写一个抽象的 带一个参数的命令 
    public abstract class AbstractDataCommand<T> : AbstractCommand, ICanSetCommandData<T>
    {
        protected T Data { get; private set; }
        void ICanSetCommandData<T>.Set(T data) => Data = data;
    }
    public abstract class AbstractConfigModelBySO : ScriptableObject, IModel
    {
        private IArchitecture mArchitecture;
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
        void ICanInit.Init() { }
        public void Deinit() => OnDeinit();
        protected virtual void OnDeinit() { }
        bool ICanInit.Initialized { get; set; }
    }
    public abstract class AbstractModelBySO : ScriptableObject, IModel
    {
        private IArchitecture mArchitecture;
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();
        public void Deinit() => OnDeinit();
        protected virtual void OnDeinit() { }
        protected abstract void OnInit();
    }
    public abstract class AbstractSystemBySO : ScriptableObject, ISystem
    {
        private IArchitecture mArchitecture;
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();
        public void Deinit() => OnDeinit();
        protected virtual void OnDeinit() { }
        protected abstract void OnInit();
    }
    // 重写一个带参数的抽象命令基类 该方式适合普通的命令使用 一般是一次算一次的命令
    public abstract class AbstractParameterCommand<TP> : IParameterCommand<TP>
    {
        protected abstract void OnExecute(TP parameter);
        void IParameterCommand<TP>.Execute(TP parameter) => OnExecute(parameter);

        private IArchitecture mArchitecture;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;
    }
    // 扩展底层接口
    public partial interface IArchitecture
    {
        void ExecuteCommandFromPool<TCmd>(ICommandData data) where TCmd : ICommand;
        void ExecuteCommandFromPool<TCmd, TD>(ICommandData<TD> data) where TCmd : ICommand, ICanSetCommandData<TD>;

        void SendCommandFromPool<TCmd>() where TCmd : ICommand;
        TResult SendCommandFromPool<TCmd, TResult>() where TCmd : ICommand<TResult>;
        void SendCommand<TCmd, TD>(TD data, bool fromPool = false) where TCmd : ICanSetCommandData<TD>, ICommand, new();
        void SendParameterCommand<TCmd, TP>(TP parameter, bool fromPool = false) where TCmd : IParameterCommand<TP>, new();
        // 基于QueryPool的查询方法
        TResult SendQueryFromPool<TQuery, TResult>() where TQuery : IQuery<TResult>, new();
        TResult SendQuery<TQuery, TResult, TP>(TP parameter) where TQuery : IParameterQuery<TResult, TP>, new();
        TResult SendQueryFromPool<TQuery, TResult, TP>(TP parameter) where TQuery : IParameterQuery<TResult, TP>, new();
    }
    public abstract partial class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        private Dictionary<Type, ICommandContainer> mCmdPool = new Dictionary<Type, ICommandContainer>();
        private Dictionary<Type, IQuery> mQueryPool = new Dictionary<Type, IQuery>();

        void IArchitecture.SendCommand<TCmd, TD>(TD data, bool fromPool)
        {
            TCmd command = fromPool ? GetCommand<TCmd>() : NewCommand<TCmd>();
            command.Set(data);
            command.Execute();
        }
        // 从对象池拿到一条命令 并 执行
        void IArchitecture.SendCommandFromPool<TCmd>() => GetCommand<TCmd>().Execute();
        TR IArchitecture.SendCommandFromPool<TCmd, TR>() => GetCommand<TCmd>().Execute();
        // 从命令池或New的方式获取一条带参数的命令
        void IArchitecture.SendParameterCommand<TCmd, TP>(TP parameter, bool fromPool)
        {
            (fromPool ? GetCommand<TCmd>() : NewCommand<TCmd>()).Execute(parameter);
        }
        private TCmd NewCommand<TCmd>() where TCmd : ICanSetArchitecture, new()
        {
            TCmd command = new TCmd();
            command.SetArchitecture(this);
            return command;
        }
        void IArchitecture.ExecuteCommandFromPool<TCmd>(ICommandData data)
        {
            ((TCmd)GetCommand(data.Type)).Execute();
        }
        void IArchitecture.ExecuteCommandFromPool<TCmd, TD>(ICommandData<TD> data)
        {
            var command = (TCmd)GetCommand(data.Type);
            command.Set(data.Info);
            command.Execute();
        }
        private ICommandContainer GetCommand(Type type)
        {
            if (!mCmdPool.TryGetValue(type, out var command))
            {
                command = (ICommandContainer)Activator.CreateInstance(type);
                command.SetArchitecture(this);
                mCmdPool.Add(type, command);
            }
            return command;
        }
        // 从池子中获取一条命令
        private TCmd GetCommand<TCmd>() where TCmd : ICommandContainer
        {
            return (TCmd)GetCommand(typeof(TCmd));
        }
        // 从池子获取一条查询
        private TQuery GetQuery<TQuery>() where TQuery : IQuery, new()
        {
            Type type = typeof(TQuery);
            if (!mQueryPool.TryGetValue(type, out var query))
            {
                query = new TQuery();
                query.SetArchitecture(this);
                mQueryPool.Add(type, query);
            }
            return (TQuery)query;
        }
        // 从Query池发送一条查询
        TResult IArchitecture.SendQueryFromPool<TQuery, TResult>() => GetQuery<TQuery>().Do();
        // 从Query池发送一条带参数的查询
        TResult IArchitecture.SendQueryFromPool<TQuery, TResult, TP>(TP parameter) => GetQuery<TQuery>().Do(parameter);
        // 用New的方式发送一条带参数的查询
        TResult IArchitecture.SendQuery<TQuery, TResult, TP>(TP parameter)
        {
            var query = new TQuery();
            query.SetArchitecture(this);
            return query.Do(parameter);
        }
    }
    public static class QFQueryEx
    {
        public static TResult SendQueryFromPool<TQuery, TResult>(this ICanSendQuery self) where TQuery : IQuery<TResult>, new()
        {
            return self.GetArchitecture().SendQueryFromPool<TQuery, TResult>();
        }
        public static TResult SendQueryFromPool<TQuery, TResult, TP>(this ICanSendQuery self, TP parameter) where TQuery : IParameterQuery<TResult, TP>, new()
        {
            return self.GetArchitecture().SendQueryFromPool<TQuery, TResult, TP>(parameter);
        }
        public static TResult SendQuery<TQuery, TResult, TP>(this ICanSendQuery self, TP parameter) where TQuery : IParameterQuery<TResult, TP>, new()
        {
            return self.GetArchitecture().SendQuery<TQuery, TResult, TP>(parameter);
        }
    }
    public static class QFCommandEx
    {
        public static void ExecuteCommandFromPool<TCmd>(this ICanSendCommand self, ICommandData data) where TCmd : ICommand
        {
            self.GetArchitecture().ExecuteCommandFromPool<TCmd>(data);
        }
        public static void ExecuteCommandFromPool<TCmd, TD>(this ICanSendCommand self, ICommandData<TD> data) where TCmd : ICommand, ICanSetCommandData<TD>
        {
            self.GetArchitecture().ExecuteCommandFromPool<TCmd>(data);
        }
        public static void SendCommandFromPool<TCmd>(this ICanSendCommand self) where TCmd : ICommand
        {
            self.GetArchitecture().SendCommandFromPool<TCmd>();
        }
        public static TResult SendCommandFromPool<TCmd, TResult>(this ICanSendCommand self) where TCmd : ICommand<TResult>
        {
            return self.GetArchitecture().SendCommandFromPool<TCmd, TResult>();
        }
        public static void SendCommand<TCmd, TP>(this ICanSendCommand self, TP parameter, bool fromPool = false) where TCmd : ICanSetCommandData<TP>, ICommand, new()
        {
            self.GetArchitecture().SendCommand<TCmd, TP>(parameter, fromPool);
        }
        public static void SendParameterCommand<TCmd, TP>(this ICanSendCommand self, TP parameter, bool fromPool = false) where TCmd : IParameterCommand<TP>, new()
        {
            self.GetArchitecture().SendParameterCommand<TCmd, TP>(parameter, fromPool);
        }
    }
}
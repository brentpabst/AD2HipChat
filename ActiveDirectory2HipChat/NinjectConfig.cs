using System.Data.Entity;
using ActiveDirectory2HipChat.Data;
using ActiveDirectory2HipChat.Directory;
using ActiveDirectory2HipChat.Services;
using Ninject.Modules;

namespace ActiveDirectory2HipChat
{
    public class NinjectConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>().InSingletonScope();
            Bind<IUserRepository>().To<UserRepository>().InSingletonScope();
            Bind<IDirectoryContext>().To<DirectoryContext>().InSingletonScope();
            Bind<DbContext>().To<DataContext>().InSingletonScope();
        }
    }
}

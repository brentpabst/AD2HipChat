using System.Data.Entity;
using Ad2HipChat.Data;
using Ad2HipChat.Directory;
using Ad2HipChat.Services;
using Ninject.Modules;

namespace Ad2HipChat
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

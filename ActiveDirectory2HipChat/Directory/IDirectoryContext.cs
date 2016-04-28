using System.DirectoryServices.AccountManagement;

namespace ActiveDirectory2HipChat.Directory
{
    public interface IDirectoryContext
    {
        PrincipalContext LoadAndConnect();
    }
}

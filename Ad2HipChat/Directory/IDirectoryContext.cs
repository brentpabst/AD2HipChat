using System.DirectoryServices.AccountManagement;

namespace Ad2HipChat.Directory
{
    public interface IDirectoryContext
    {
        PrincipalContext LoadAndConnect();
    }
}

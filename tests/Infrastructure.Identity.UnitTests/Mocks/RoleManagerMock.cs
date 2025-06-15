using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.Identity.UnitTests.Mocks;

public static class RoleManagerMock
{
    public static Mock<RoleManager<TRole>> GetMock<TRole>()
        where TRole : class
    {
        var store = new Mock<IRoleStore<TRole>>();
        var roleManager = new Mock<RoleManager<TRole>>(store.Object, null, null, null, null);
        return roleManager;
    }
}

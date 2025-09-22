using MinimalApi.Domain.DTOs.Enums;
using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public sealed class AdministratorTest
{
    [TestMethod("GetSetProperties")]
    public void TestGetSetProperties()
    {
        Administrator administrator = new()
        {
            Id = 1,
            Email = "adm@test.com",
            Password = "12345678",
            Profile = Profile.ADMINISTRATOR
        };
        Assert.AreEqual(1, administrator.Id);
        Assert.AreEqual("adm@test.com", administrator.Email);
        Assert.AreEqual("12345678", administrator.Password);
        Assert.AreEqual(Profile.ADMINISTRATOR, administrator.Profile);
    }
}

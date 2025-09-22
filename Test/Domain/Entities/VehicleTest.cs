using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public sealed class VehicleTest
{
    [TestMethod("GetAndSetProperties")]
    public void TestGetAndSetProperties()
    {
        Vehicle vehicle = new()
        {
            Id = 1,
            Name = "Siena",
            Brand = "Fiat",
            Year = 2011
        };

        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Siena", vehicle.Name);
        Assert.AreEqual("Fiat", vehicle.Brand);
        Assert.AreEqual(2011, vehicle.Year);
    }
}
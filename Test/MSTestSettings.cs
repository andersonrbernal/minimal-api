[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Test;

[TestClass]
public static class MSTestSettings
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        context.WriteLine($"[INIT] BaseDir: {AppContext.BaseDirectory} | appsettings exists: {File.Exists(path)}");
    }
}
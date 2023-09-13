using System;
using System.Reflection;

namespace Mirage
{
    public static class GeneratedCode
    {
        public const string GENERATED_NAMEPACE = "Mirage";
        public const string GENERATED_CLASS = "GeneratedNetworkCode";
        public const string INIT_METHOD = "InitReadWriters";
        private static bool hasInit = false;

        public static void Init()
        {
            if (hasInit)
                return;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Namespace != GENERATED_NAMEPACE || type.Name != GENERATED_CLASS)
                        continue;

                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name != INIT_METHOD)
                            continue;

                        Console.WriteLine($"Init Generated code in {assembly.FullName}");
                        method.Invoke(null, null);
                    }
                }
            }

            hasInit = true;
        }
    }
}

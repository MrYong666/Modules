using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectFindMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            Get();
            var helloType = typeof(BaseClass);

            //
            Assembly assem = Assembly.GetAssembly(helloType);
            var t2 = assem.GetTypes().Where(s => s.BaseType == helloType);

            //
            List<Type> types = new List<Type>();

            var t1 = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (helloType.IsAssignableFrom(type))
                    {
                        if (type.IsClass && !type.IsAbstract)
                        {
                            types.Add(type);
                        }
                    }
                }
            }
            types.ForEach((t) =>
            {
                var helloInstance = Activator.CreateInstance(t) as BaseClass;

                helloInstance.AddData();
            });

            Console.ReadKey();

        }
        private static  void Get()
        {
            List<Type> types = new List<Type>();
            var helloType = typeof(BaseClass);
            Assembly assem = Assembly.GetAssembly(helloType);
            var t2 = assem.GetTypes().Where(s => s.BaseType == helloType);
            foreach (var type in t2)
            {
                if (helloType.IsAssignableFrom(type))
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        types.Add(type);
                    }
                }
            }
            types.ForEach((t) =>
            {
                var helloInstance = Activator.CreateInstance(t) as BaseClass;
                helloInstance.AddData();
            });
        }
    }
}

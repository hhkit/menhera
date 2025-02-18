using System.Diagnostics;
using System.Reflection;

namespace menhera
{
    public abstract class Service { }

    public class ServiceLocator
    {
        public static ServiceLocator Main { get; private set; } = new();

        private readonly Dictionary<Type, Service> serviceMap = [];

        public ServiceLocator()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Service)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Service))))
            {
                serviceMap.Add(type, value: Activator.CreateInstance(type) as Service);
                Debug.WriteLine($"Added service {type.Name}");
            }
        }

        public static void Initialize()
        {
            if (Main == null)
                Main = new();
        }

        public void AddService<T>(T service) where T : Service
        {
            serviceMap.Add(typeof(T), service);
        }

        public T GetService<T>() where T : Service
        {
            return (T)serviceMap[typeof(T)];
        }
    }
}
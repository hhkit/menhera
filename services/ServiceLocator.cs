using System.Reflection;

namespace menhera
{
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
            }
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
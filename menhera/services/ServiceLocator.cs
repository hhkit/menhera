using System.Diagnostics;
using System.Reflection;

namespace menhera
{
    public abstract class Service { }

    public class ServiceLocator
    {
        public static ServiceLocator Main { get; private set; } = new();

        private readonly Dictionary<Type, Service> serviceMap = [];

        private class TypeNode(Type type, ConstructorInfo ctor)
        {
            public Type type = type;
            public ConstructorInfo ctor = ctor;
            public List<TypeNode> dependencies = [];
            public bool initialized = false;
        }

        public ServiceLocator()
        {
            var typeInfos = typeof(Service).Assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Service)))
                .Select((typeInfo, Index) =>
                {
                    foreach (var ctor in typeInfo.GetConstructors())
                    {
                        Debug.WriteLine($"registering {typeInfo} with ctor {ctor}");
                        return new KeyValuePair<Type, TypeNode>(typeInfo, new TypeNode(typeInfo, ctor));
                    }
                    Debug.Assert(false, $"type {typeInfo} should have a constructor");
                    throw new Exception();
                });

            Dictionary<Type, TypeNode> typeMap = new(typeInfos);

            foreach (var (type, typeNode) in typeMap)
            {
                // get ctor, construct graph
                foreach (var param in typeNode.ctor.GetParameters())
                {
                    Debug.WriteLine($"{typeNode.type.Name} -> {param}");
                    if (typeMap.TryGetValue(param.ParameterType, out var dependentTypeNode))
                    {
                        typeNode.dependencies.Add(dependentTypeNode);
                    }
                    else
                    {
                        Debug.Assert(false, $"While initializing service {typeNode.type}, could not find service of type {param.ParameterType}");
                    }
                }
            }

            var initCount = 0;
            while (initCount < typeMap.Count())
            {
                var prevCount = initCount;
                foreach (var (key, typeNode) in typeMap)
                {
                    if (typeNode.initialized)
                        continue;

                    if (typeNode.dependencies.Count == 0 || typeNode.dependencies.TrueForAll(node => node.initialized))
                    {
                        List<object> dependentServices = [];
                        foreach (var paramInfo in typeNode.ctor.GetParameters())
                        {
                            var service = serviceMap.GetValueOrDefault(paramInfo.ParameterType);
                            Debug.Assert(service != null, $"Could not find service of type {paramInfo.ParameterType}");
                            dependentServices.Add(service);
                        }
                        var constructedService = typeNode.ctor.Invoke(dependentServices.ToArray()) as Service;
                        Debug.Assert(constructedService != null, $"Failed to construct service of type {typeNode.type}");
                        serviceMap.Add(typeNode.type, constructedService);
                        typeNode.initialized = true;
                        initCount++;
                    }
                }
                Debug.Assert(prevCount < initCount, $"Ran out of initializable services {prevCount} < {initCount}, max: {typeMap.Count}");
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
using System.Diagnostics;
using System.Reflection;

namespace menhera
{
    public abstract class Service { }

    public class ServiceLocator
    {
        private readonly Dictionary<Type, Service> serviceMap = [];

        private class TypeNode(Type type, ConstructorInfo ctor)
        {
            public Type type = type;
            public ConstructorInfo ctor = ctor;
            public List<TypeNode> dependencies = [];
            public bool initialized = false;
        }

        public ServiceLocator(params Service[] overrideServices)
        {
            var typeInfos = typeof(Service).Assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Service)))
                .Select((typeInfo, Index) =>
                {
                    foreach (var ctor in typeInfo.GetConstructors())
                    {
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

            foreach (var overrideService in overrideServices)
            {
                var baseServiceNode = typeMap.Select((kvp) => kvp.Value).Where(v => overrideService.GetType().IsSubclassOf(v.type)).First();
                Debug.Assert(baseServiceNode != null, $"Service {overrideService.GetType()} must inherit a type");
                baseServiceNode.initialized = true;
                serviceMap.Add(baseServiceNode.type, overrideService);
            }

            while (serviceMap.Count < typeMap.Count)
            {
                var prevCount = serviceMap.Count;
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
                    }
                }
                Debug.Assert(prevCount < serviceMap.Count, $"Ran out of initializable services {prevCount} < {serviceMap.Count}, max: {typeMap.Count}");
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
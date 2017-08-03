using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meceqs.TypedHandling.Configuration
{
    public class HandlerCollection : IEnumerable<IHandlerMetadata>
    {
        private readonly List<IHandlerMetadata> _handlerMetadatas = new List<IHandlerMetadata>();

        public int Count => _handlerMetadatas.Count;

        /// <summary>
        /// Adds a type representing a <see cref="IHandles"/>.
        /// </summary>
        /// <remarks>
        /// Handler instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a handler as a service.
        /// </remarks>
        public void Add<THandler>() where THandler : class, IHandles
        {
            Add(typeof(THandler));
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandles"/>.
        /// </summary>
        /// <param name="handlerType">Type representing an <see cref="IHandles"/>.</param>
        /// <remarks>
        /// Handler instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a handler as a service.
        /// </remarks>
        public void Add(Type handlerType)
        {
            Guard.NotNull(handlerType, nameof(handlerType));

            EnsureValidHandler(handlerType);

            var implementedHandles = GetImplementedHandles(handlerType);
            var factory = new ActivatorHandlerMetadata(handlerType, implementedHandles);
            Add(factory);
        }

        /// <summary>
        /// Adds a <see cref="IHandlerMetadata"/> instance.
        /// </summary>
        public void Add(IHandlerMetadata handlerMetadata)
        {
            Guard.NotNull(handlerMetadata, nameof(handlerMetadata));

            if (_handlerMetadatas.Any(x => x.HandlerType == handlerMetadata.HandlerType))
            {
                throw new InvalidOperationException($"The handler type '{handlerMetadata.HandlerType}' has already been added.");
            }

            // Ensure there are no duplicate IHandles implementations in different handler types
            foreach (var existing in _handlerMetadatas)
            {
                var duplicates = handlerMetadata.ImplementedHandles.Where(x => existing.ImplementedHandles.Contains(x)).ToList();
                if (duplicates.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"The handler '{handlerMetadata.HandlerType}' can not be added because it contains the following " +
                        $"'{nameof(IHandles)}' implementations which have already been added by the handler '{existing.HandlerType}': " +
                        string.Join(";", duplicates.Select(x => $"{x.MessageType}/{x.ResultType}"))
                    );
                }
            }

            _handlerMetadatas.Add(handlerMetadata);
        }

        /// <summary>
        /// Adds all types representing an <see cref="IHandles"/> from the assembly of the given type.
        /// </summary>
        /// <param name="filter">Allows to filter the types (e.g. by namespace)</param>
        /// <remarks>
        /// Handler instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a handler as a service.
        /// </remarks>
        public void AddFromAssembly<TType>(Predicate<Type> filter = null)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            AddFromAssembly(assembly, filter);
        }

        /// <summary>
        /// Adds all types representing an <see cref="IHandles"/> from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to be searched for <see cref="IHandles"/> implementations.</param>
        /// <param name="filter">Allows to filter the types (e.g. by namespace).</param>
        /// <remarks>
        /// Handler instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a handler as a service.
        /// </remarks>
        public void AddFromAssembly(Assembly assembly, Predicate<Type> filter = null)
        {
            Guard.NotNull(assembly, nameof(assembly));

            var handlers = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where typeof(IHandles).IsAssignableFrom(type)
                           where filter == null || filter(type)
                           select type;

            foreach (var handler in handlers)
            {
                Add(handler);
            }
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandles"/>.
        /// </summary>
        /// <remarks>
        /// Handler instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a handler that will be created via
        /// type activation.
        /// </remarks>
        public void AddService<THandler>() where THandler : class, IHandles
        {
            AddService(typeof(THandler));
        }

        /// <summary>
        /// Adds a type representing an <see cref="IHandles"/>.
        /// </summary>
        /// <param name="handlerType">Type representing an <see cref="IHandles"/>.</param>
        /// <remarks>
        /// Handler instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a handler that will be created via
        /// type activation.
        /// </remarks>
        public void AddService(Type handlerType)
        {
            Guard.NotNull(handlerType, nameof(handlerType));

            EnsureValidHandler(handlerType);

            var implementedHandles = GetImplementedHandles(handlerType);
            var factory = new ServiceHandlerMetadata(handlerType, implementedHandles);
            Add(factory);
        }

        public IEnumerator<IHandlerMetadata> GetEnumerator() => _handlerMetadatas.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _handlerMetadatas.GetEnumerator();

        private static void EnsureValidHandler(Type handlerType)
        {
            if (!typeof(IHandles).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException(
                    $"Type '{handlerType}' must derive from '{typeof(IHandles)}'",
                    nameof(handlerType));
            }

            if (!handlerType.GetTypeInfo().IsClass)
            {
                throw new ArgumentException($"Type '{handlerType}' must be a class.", nameof(handlerType));
            }

            if (handlerType.GetTypeInfo().IsAbstract)
            {
                throw new ArgumentException($"Type '{handlerType}' must not be abstract.", nameof(handlerType));
            }
        }

        private static IEnumerable<HandleDefinition> GetImplementedHandles(Type handlerType)
        {
            var type = handlerType;
            while (type != null)
            {
                foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
                {
                    // We only care about IHandles interfaces.
                    if (!typeof(IHandles).IsAssignableFrom(implementedInterface))
                        continue;

                    // IHandles itself is just a marker interface.
                    if (implementedInterface == typeof(IHandles))
                        continue;

                    int typeArguments = implementedInterface.GenericTypeArguments.Length;
                    if (typeArguments < 1)
                    {
                        throw new NotSupportedException(
                            $"Interface '{implementedInterface}' does not have any generic types. " +
                            $"This method tries to read the message type and the result type from the {nameof(IHandles)} " +
                            $"implementations. There are two different {nameof(IHandles)} interfaces. " +
                            "One that accepts only a message type and one that accepts a message type and a result type." +
                            "Seems like the given interface is neither of those!");
                    }

                    if (typeArguments > 2)
                    {
                        throw new NotSupportedException(
                            $"Interface '{implementedInterface}' has more than 2 generic types (it has {typeArguments})." +
                            $"This method tries to read the message type and the result type from the {nameof(IHandles)} " +
                            $"implementations. There are two different {nameof(IHandles)} interfaces. " +
                            "One that accepts only a message type and one that accepts a message type and a result type." +
                            "Seems like the given interface is neither of those!");
                    }

                    Type messageType = implementedInterface.GenericTypeArguments[0];
                    Type resultType = implementedInterface.GenericTypeArguments.Length > 1 ? implementedInterface.GenericTypeArguments[1] : null;

                    yield return new HandleDefinition(messageType, resultType);
                }

                type = type.GetTypeInfo().BaseType;
            }
        }
    }
}
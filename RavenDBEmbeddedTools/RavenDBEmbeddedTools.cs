using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDBEmbeddedTools
{
    /// <summary>
    /// Singleton that holds the RavenDB document store connection
    /// </summary>
    internal sealed class EmbeddableDocumentStoreSingleton
    {
        private static readonly Lazy<EmbeddableDocumentStoreSingleton> lazy =
            new Lazy<EmbeddableDocumentStoreSingleton>(() => new EmbeddableDocumentStoreSingleton());

        public static EmbeddableDocumentStoreSingleton Instance { get { return lazy.Value; } }

        public EmbeddableDocumentStore EmbeddableDocumentStore { get; set; }

        private EmbeddableDocumentStoreSingleton()
        {
            this.EmbeddableDocumentStore = new EmbeddableDocumentStore
            {
                DataDirectory = "Data"
            };
            this.EmbeddableDocumentStore.Initialize();
        }
    }

    /// <summary>
    /// Use these static generic commands to query and store data in your RavenDB document store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class RavenDBGenericCommands<T>
    {
        private static EmbeddableDocumentStore GetEmbeddableDocumentStore()
        {
            return EmbeddableDocumentStoreSingleton.Instance.EmbeddableDocumentStore;
        }
        
        /// <summary>
        /// Gets the TypeTagName of T from the embedded RavenDB document store
        /// </summary>
        /// <returns></returns>
        private static string GetTypeTagName()
        {
            return GetEmbeddableDocumentStore().Conventions.GetTypeTagName(typeof(T));
        }

        /// <summary>
        /// Generates a RavenDB id based on TypeTagName plus a number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string BuildRavenDBID(int id)
        {
            return String.Concat(GetTypeTagName(), "/", id);
        }

        /// <summary>
        /// Gets an instance of T with the specified id from the embedded RavenDB document store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetById(string id)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                T instance = session.Load<T>(id);
                return instance;
            }
        }

        /// <summary>
        /// Gets an instance of T with the specified id from the embedded RavenDB document store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetById(int id)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                T instance = session.Load<T>(BuildRavenDBID(id));
                return instance;
            }
        }

        /// <summary>
        /// Gets all instances of T from the embedded RavenDB document store
        /// </summary>
        /// <returns></returns>
        public static IList<T> GetAll()
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                IList<T> results = session
                    .Query<T>()
                    .ToList();
                return results;
            }
        }

        /// <summary>
        /// Gets the number of instances of T located in the embedded RavenDB document store
        /// </summary>
        /// <returns></returns>
        public static int Count()
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                int count = session
                    .Query<T>()
                    .Count<T>();
                return count;
            }
        }

        /// <summary>
        /// Gets a IList of T matching the func passed in parameter from the embedded RavenDB document store
        /// </summary>
        /// <param name="whereFunc">The delegate that filters the T instances. For example: Func<Person, bool> searchFunc = (x) => x.Name.Contains("Smith");</param>
        /// <returns></returns>
        public static IList<T> GetByFunc(Func<T, bool> whereFunc)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                IList<T> results = session
                    .Query<T>()
                    .Where(whereFunc)
                    .ToList();
                return results;
            }
        }

        /// <summary>
        /// Inserts an instance of T in the embedded RavenDB document store
        /// </summary>
        /// <param name="instance"></param>
        public static void Insert(T instance)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                session.Store(instance);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Inserts multiple instances of T in the embedded RavenDB document store
        /// </summary>
        /// <param name="instance"></param>
        public static void Insert(IList<T> listOfInstance)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                foreach (var instance in listOfInstance)
                {
                    session.Store(instance);
                }
                session.SaveChanges();
            }
        }

        public static void Update(T instance)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                //Get previous instance version from document store (needed to hold the store session)
                string instanceOldVersionId = (string)instance.GetType().GetProperty("Id").GetValue(instance, null);
                T instanceOldVersion = GetById(instanceOldVersionId);
                //Update the previous version properties with the new version properties
                foreach (var property in instance.GetType().GetProperties())
                {
                    instance.GetType().GetProperty(property.Name).SetValue(instanceOldVersion, instance.GetType().GetProperty(property.Name).GetValue(instance, null));
                }

                session.Store(instance);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Delete an instance of T with the specified Id from the embedded RavenDB document store
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(string id)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                session.Delete(id);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Delete an instance of T with the specified Id from the embedded RavenDB document store
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            using (IDocumentSession session = GetEmbeddableDocumentStore().OpenSession())
            {
                session.Delete(BuildRavenDBID(id));
                session.SaveChanges();
            }
        }
    }
}

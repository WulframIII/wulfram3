using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Entry point into the ObjectPool Instantiation/Recycling service
/// </summary>
/// <remarks>
/// Execution Time should be -1000
/// </remarks>
[AddComponentMenu("ObjectPool/ObjectPool")]
public class ObjectPool : MonoBehaviour
{
    #region members
    /// <summary>
    /// Encapsilates a set of prefab instances.
    /// </summary>
    [Serializable]
    public class ObjectPoolItem
    {
        /// <summary>
        /// Instance to maintain in this pool
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// Initial quantity of the pool
        /// </summary>
        public int Quantity;

        /// <summary>
        /// objects ready
        /// </summary>
        [HideInInspector]
        public List<GameObject> Instances = new List<GameObject>();

        /// <summary>
        /// Last Instance returned from GetNext
        /// </summary>
        [HideInInspector]
        public int Index;

        /// <summary>
        /// gets a instance
        /// </summary>
        /// <param name="activate">will activate the item</param>
        /// <returns></returns>
        public GameObject GetNext(bool activate)
        {
            if (Instances.Count == 0)
            {
                GrowOne();
            }

            Index++;

            if (Index >= Instances.Count)
                Index = 0;

            var obj = Instances[Index];

            if (activate)
                obj.SetActive(true);

            Instances.Remove(obj);

            return obj;
        }

        /// <summary>
        /// grows the pool by one
        /// </summary>
        public void GrowOne()
        {
            var obj = (GameObject)Instantiate(Prefab);

            // uniform name lookup
            obj.name = obj.name.Replace("(Clone)", "");

            obj.transform.parent = Instance.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            
            Instances.Add(obj);
        }

        /// <summary>
        /// returns the object to the pool
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(GameObject obj)
        {
            ResetItem(obj);

            if (!Instances.Contains(obj))
                Instances.Add(obj);
        }

        void ResetItem(GameObject o)
        {
            if (o.activeSelf)
            {
                o.transform.parent = Instance.transform;
                o.transform.localPosition = Vector3.zero;
                o.transform.localRotation = Quaternion.identity;
                o.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Creates an object poolsat runtime from prefabs in a resource folder
    /// </summary>
    [Serializable]
    public class ResourcePool
    {
        /// <summary>
        /// /Resources/{Path}
        /// </summary>
        public string Path;

        /// <summary>
        /// Quantity in this pool
        /// </summary>
        public int Quantity;
    }
    #endregion

    #region static
    /// <summary>
    /// Exposes the service
    /// </summary>
    public static ObjectPool Instance { get; private set; }

    /// <summary>
    /// raised when the pool has loaded
    /// </summary>
    public static event Action OnReady;
    /// <summary>
    /// Set to true after default pools are loaded
    /// </summary>
    public static bool Ready { get; private set; }
    #endregion

    /// <summary>
    /// Object pools
    /// </summary>
    [SerializeField]
    public List<ObjectPoolItem> Pools = new List<ObjectPoolItem>();

    /// <summary>
    /// Pools created at runtime from resources
    /// </summary>
    [SerializeField]
    public List<ResourcePool> ResourcePools = new List<ResourcePool>();

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple object pools found");
            return;
        }

        Ready = false;

        Instance = this;

        foreach (var resourceFolder in ResourcePools)
        {
            var rs = Resources.LoadAll(resourceFolder.Path, typeof(GameObject));

            foreach (var o in rs)
            {
                var pool = Instance.Pools.FirstOrDefault(p => p.Prefab.name == o.name);

                if (pool == null)
                {
                    pool = new ObjectPoolItem
                    {
                        Prefab = o as GameObject,
                        Quantity = resourceFolder.Quantity,
                    };

                    Pools.Add(pool);
                }
            }
        }

        StartCoroutine(BuildPools());
    }

    void OnEnable()
    {
        // here to make awake exe befor onenable
        // wierd behaviour
    }

    void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Gets a new or recycled instance of the object from the pool.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="activate">should the item be activated</param>
    /// <returns></returns>
    public GameObject GetNext(GameObject prefab, bool activate)
    {

        var pool = Instance.Pools.FirstOrDefault(o => o.Prefab.name == prefab.name);

        if (pool == null)
        {
            pool = new ObjectPoolItem
            {
                Prefab = prefab,
                Quantity = 1,
            };

            Pools.Add(pool);
        }

        return pool.GetNext(activate);
    } 
    
    /// <summary>
    /// helper method
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    public void SpawnEffect(GameObject prefab, Vector3 position)
    {
        var instance = GetNext(prefab, false);
        instance.transform.position = position;
        instance.SetActive(true);
    }

    /// <summary>
    /// helper method
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    public void SpawnNetworkEffect(GameObject prefab, Vector3 position)
    {
        GetComponent<NetworkView>().RPC("SpawnEffectRpc", RPCMode.All,prefab.name, position);
    }

    [PunRPC]
    void SpawnEffectRpc(string prefabName, Vector3 position)
    {
        var prefab = Pools.Where(o => o.Prefab.name == prefabName).Single().GetNext(false);
        SpawnEffect(prefab,position);
    }


    /// <summary>
    /// Recycles the object back into the pool
    /// </summary>
    /// <param name="instance"></param>
    public void Recycle(GameObject instance)
    {
        var pool = Instance.Pools.FirstOrDefault(o => o.Prefab.name == instance.name);

        if (pool == null)
        {
            Destroy(instance);
        }
        else
        {
            pool.Recycle(instance);

        }
    }

    IEnumerator BuildPools()
    {
        foreach (var objectPool in Pools)
        {
            yield return StartCoroutine(BuildPool(objectPool));
        }

        Ready = true;

        if (OnReady != null)
            OnReady();
    }

    IEnumerator BuildPool(ObjectPoolItem pool)
    {
        for (int i = 0; i < pool.Quantity; i++)
        {
            if (pool.Instances.Count >= pool.Quantity)
                yield break;

            pool.GrowOne();

            yield return 1;
        }
    }
}
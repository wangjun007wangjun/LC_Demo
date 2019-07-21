using System.IO;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BaseFramework
{
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        private readonly object _obj = new object();

        private AssetBundleManifest _assetBundleManifest;
        private Dictionary<string, AssetBundleUnit> _caches;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();

            _caches = new Dictionary<string, AssetBundleUnit>();

            AssetBundleUnit infoUnit = Load("StreamingAssets");
            _assetBundleManifest = infoUnit.assetBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            Release(infoUnit);
        }

        public override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();

            UnityEngine.Object.Destroy(_assetBundleManifest);
            _assetBundleManifest = null;
            foreach (AssetBundleUnit unit in _caches.Values)
            {
                Release(unit);
            }
            _caches.Clear();
            _caches = null;
        }

        public AssetBundleUnit Load(string name)
        {
            if (_caches.ContainsKey(name)) return _caches[name];
            AssetBundleUnit assetBundleUnit = new AssetBundleUnit
                                              {
                                                  name = name,
                                                  assetBundle =
                                                      AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath,name))
                                              };
            assetBundleUnit.referenceCount++;
            if (_assetBundleManifest != null)
            {
                assetBundleUnit.dependencies = _assetBundleManifest.GetAllDependencies(name);
                assetBundleUnit.dependencies.ForEach((index, it) => {
                                                         Load(it);
                                                     });
            }
            _caches.Add(name, assetBundleUnit);

            return _caches[name];
        }

        public void LoadAsync(string name, Action<AssetBundleUnit> loadAction)
        {
            List<string> assetNames = new List<string> {name};

            string[] dependencies = GetDependencies(name);
            dependencies?.ForEach((indexer, it) => assetNames.Add(it));

            IEnumerator[] enumerators = new IEnumerator[assetNames.Count];
            assetNames.ForEach((index, it) => enumerators[index] = Load(it, index == 0));

            TaskHelper.Create<CoroutineTask>()
                      .Delay(enumerators)
                      .Do(() =>
                          {
                              if (_caches.ContainsKey(name))
                              {
                                  loadAction?.Invoke(_caches[name]);
                              }
                              else
                              {
                                  Log.E(this, $"Load asset bundle error, not find {name} in cache dic");
                              }
                          })
                      .Execute();
        }

        private IEnumerator<object> Load(string assetBundleName, bool isRoot = false)
        {
            lock (_obj)
            {
                if (!_caches.ContainsKey(assetBundleName)
                    || _caches[assetBundleName] == null)
                {
                    AssetBundleCreateRequest assetBundleCreateRequest = 
                        AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, assetBundleName));

                    Log.D("AssetBundleManager", $"load {assetBundleName}");

                    yield return assetBundleCreateRequest;

                    // wait one frame 
                    yield return null;

                    AssetBundleUnit unit = new AssetBundleUnit();
                    unit.referenceCount++;
                    unit.name = assetBundleName;
                    unit.assetBundle = assetBundleCreateRequest.assetBundle;
                    if (isRoot)
                    {
                        unit.dependencies = GetDependencies(assetBundleName);
                    }
                    
                    //防止多次点击
                    if(!_caches.ContainsKey(assetBundleName))
                        _caches.Add(assetBundleName, unit);
                }
                else
                {
                    _caches[assetBundleName].referenceCount++;
                }

                yield return null;
            }
        }

        private AssetBundleUnit GetFormCache(string name)
        {
            return _caches.ContainsKey(name) ? _caches[name] : null;
        }

        private string[] GetDependencies(string assetBundleName)
        {
            return _assetBundleManifest != null ? _assetBundleManifest.GetAllDependencies(assetBundleName) : null;
        }

        public void Release(string name,
                            bool unloadAllLoadedObjects = false)
        {
            if (_caches.ContainsKey(name))
            {
                Release(_caches[name]);
            }
        }

        public void Release(AssetBundleUnit bundleUnit,
                            bool unloadAllLoadedObjects = false)
        {
            if(bundleUnit == null)
            {
                Log.W(this, "Release asset bundle is null!");
                return;
            }

            string[] dependencies = bundleUnit.dependencies;
            dependencies.ForEach((index, it) => {
                if (_caches.ContainsKey(it))
                {
                    Release(_caches[it]);
                }
            });
            bundleUnit.referenceCount--;
            if (bundleUnit.referenceCount <= 0)
            {
                Unload(bundleUnit, unloadAllLoadedObjects);
            }
        }

        private void Unload(AssetBundleUnit assetBundleUnit,
                           bool unloadAllLoadedObjects = false)
        {
            if (assetBundleUnit == null)
            {
                Log.W(this, "Unload asset bundle is null!");
                return;
            }

            if (_caches.ContainsKey(assetBundleUnit.name))
            {
                _caches.Remove(assetBundleUnit.name);
            }
            if (assetBundleUnit.assetBundle != null)
            {
                assetBundleUnit.assetBundle.Unload(unloadAllLoadedObjects);
            }
            assetBundleUnit.name = null;
            assetBundleUnit.assetBundle = null;
            assetBundleUnit.dependencies = null;
            assetBundleUnit.referenceCount = 0;
        }

        public void ExportCurrentMessage()
        {
            StringBuilder stringBuilder = new StringBuilder();
            _caches.ForEach((key, value) => {
                if(value != null)
                {
                    stringBuilder.Append("AssetBundle path:" + key + ", referenceCount:" + value.referenceCount + "\n");
                }
                else
                {
                    Log.W(this, "ExportCurrentMessage {0} asset bundle is null", key);
                }
            });
            Log.I(this, stringBuilder.ToString());
        }
    }
}

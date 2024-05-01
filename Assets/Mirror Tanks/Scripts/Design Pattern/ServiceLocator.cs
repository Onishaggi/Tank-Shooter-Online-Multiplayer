using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }

        public void RegisterService<T>(T service)
        {
            services[typeof(T)] = service;
        }

        public T GetService<T>()
        {
            if ((T)services[typeof(T)] == null)
            {

                Debug.Log("Please add UIManager to the scene");
            }
            return (T)services[typeof(T)];
        }
    }
}

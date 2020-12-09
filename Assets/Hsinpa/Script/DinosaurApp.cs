using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
namespace Hsinpa {
    public class DinosaurApp : Singleton<DinosaurApp>
    {
        protected DinosaurApp() { } // guarantee this will be always a singleton only - can't use the constructor!

        private Subject subject;

        private Observer[] observers = new Observer[0];

        private int readyPipeline = 0;
        private int targetReadyPipeline = 2;

        private void Awake()
        {
            subject = new Subject();

            RegisterAllController(subject);

            Init();
        }

        private void Start()
        {
            AppStart(true);
        }

        private void AppStart(bool success)
        {
            Notify(EventFlag.Event.GameStart);
        }

        public void Notify(string entity, params object[] objects)
        {
            subject.notify(entity, objects);
        }

        public void Init()
        {
        }

        private void RegisterAllController(Subject p_subject)
        {
            Transform ctrlHolder = transform.Find("Controller");

            if (ctrlHolder == null) return;

            observers = transform.GetComponentsInChildren<Observer>();

            foreach (Observer observer in observers)
            {
                subject.addObserver(observer);
            }
        }

        public T GetObserver<T>() where T : Observer
        {
            foreach (Observer observer in observers)
            {
                if (observer.GetType() == typeof(T)) return (T)observer;
            }

            return default(T);
        }

        private void OnApplicationQuit()
        {

        }

    }
}
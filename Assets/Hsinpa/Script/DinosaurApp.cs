using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using System.Threading.Tasks;
using Hsinpa.CloudAnchor;

namespace Hsinpa {
    public class DinosaurApp : Singleton<DinosaurApp>
    {

        [SerializeField]
        private LightHouseAnchorManager _lightHouseAnchorManager;

        [SerializeField]
        private bool TestMode;

        protected DinosaurApp() { } // guarantee this will be always a singleton only - can't use the constructor!

        private Subject subject;

        private Observer[] observers = new Observer[0];
       
        private void Awake()
        {
            subject = new Subject();

            _lightHouseAnchorManager.OnCloudAnchorIsSetUp += OnAzureAnchorIsReady;

            RegisterAllController(subject);

            Init();
        }

        private void Start()
        {

            StartCoroutine(
                ARFoundationUtility.AysncCheckARReady( (bool avilable) => {

                    if (avilable)
                        _lightHouseAnchorManager.SetUp();
                    else
                        AppStart(false);
            }));
        }

        private void OnAzureAnchorIsReady(bool isReady) {
            AppStart(isReady);
        }

        private void AppStart(bool success)
        {
            Notify(EventFlag.Event.GameStart, success, TestMode);
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
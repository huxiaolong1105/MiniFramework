﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniFramework
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public GameObject UIRoot;
        public Canvas Canvas;
        public Camera UICamera;
        public EventSystem EventSystem;
        private string AssetBundlePath;
        private readonly Dictionary<string, UIPanel> UIPanelDict = new Dictionary<string, UIPanel>();
        public void Init()
        {
            AssetBundlePath = Application.streamingAssetsPath + "/AssetBundle/StandaloneWindows/ui";
            UIRoot = GameObject.Find("UI Root");
            if (UIRoot != null)
            {
                UIRoot.transform.SetParent(transform);
            }
            else
            {
                UIRoot = Resources.Load("UI/UI Root") as GameObject;
                UIRoot = Instantiate(UIRoot, transform);
            }
            GetCanvas();
            GetCamera();
            GetEventSystem();
            GetUI();

            ResLoader.Instance.LoadAssetBundle(this, AssetBundlePath, LoadUIFromAssetBundle);
        }
        void GetCanvas()
        {
            Canvas = UIRoot.GetComponentInChildren<Canvas>();
        }
        void GetCamera()
        {
            UICamera = UIRoot.GetComponentInChildren<Camera>();
        }

        void GetEventSystem()
        {
            EventSystem = UIRoot.GetComponentInChildren<EventSystem>();
        }
        /// <summary>
        /// 获取根路径UI
        /// </summary>
        void GetUI()
        {
            for (int i = 0; i < Canvas.transform.childCount; i++)
            {
                UIPanel panel = Canvas.transform.GetChild(i).GetComponent<UIPanel>();
                if (panel!=null&&!UIPanelDict.ContainsKey(panel.name))
                {
                    UIPanelDict.Add(panel.name, panel);
                }
            }
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="panelName"></param>
        public void OpenUI(string panelName)
        {
            if (UIPanelDict.ContainsKey(panelName))
            {
                UIPanelDict[panelName].Open();
            }           
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="panelName"></param>
        public void CloseUI(string panelName)
        {
            if (UIPanelDict.ContainsKey(panelName))
            {
                UIPanelDict[panelName].Close();
            }
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="panelName"></param>
        public void DestroyUI(string panelName)
        {
            if (UIPanelDict.ContainsKey(panelName))
            {
                Destroy(UIPanelDict[panelName].gameObject);
                UIPanelDict.Remove(panelName);
            }          
        }
        /// <summary>
        /// 禁用面板交互
        /// </summary>
        /// <param name="panelName"></param>
        public void DisableRayCast(string panelName)
        {
            if (UIPanelDict.ContainsKey(panelName))
            {
                UIPanel panel = UIPanelDict[panelName];
                Image[] images = panel.transform.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].raycastTarget = false;
                }
                Text[] texts = panel.transform.GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].raycastTarget = false;
                }
            }
        }
        /// <summary>
        /// 启用面板交互
        /// </summary>
        /// <param name="panelName"></param>
        public void EnableRayCast(string panelName)
        {
            if (UIPanelDict.ContainsKey(panelName))
            {
                UIPanel panel = UIPanelDict[panelName];
                Image[] images = panel.transform.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].raycastTarget = true;
                }
                Text[] texts = panel.transform.GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].raycastTarget = true;
                }
            }
        }
        /// <summary>
        /// 从AssetBundle中加载UI
        /// </summary>
        /// <param name="ab"></param>
        void LoadUIFromAssetBundle(AssetBundle ab)
        {
            GameObject[] objects = ab.LoadAllAssets<GameObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject ui = Instantiate(objects[i], Canvas.transform);
                ui.name = objects[i].name;
                UIPanel panel = ui.GetComponent<UIPanel>();
                if (panel != null&&!UIPanelDict.ContainsKey(panel.name))
                {
                    UIPanelDict.Add(panel.name, panel);
                }
            }
        }

    }
}

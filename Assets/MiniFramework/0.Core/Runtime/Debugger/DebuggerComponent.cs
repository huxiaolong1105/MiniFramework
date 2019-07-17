﻿using System.Collections.Generic;
using UnityEngine;
namespace MiniFramework
{
    public sealed partial class DebuggerComponent : MonoSingleton<DebuggerComponent>
    {
        //调试器默认标题
        static readonly string DefaultWindowTitle = "<b>MiniFramework Debugger</b>";
        static readonly string DefaultMiniWindowTitle = "<b>Debugger</b>";
        //调试器默认大小
        static readonly Rect DefaultWindowRect = new Rect(0, 0, Screen.width / 2, Screen.height / 2);
        static readonly float DefaultWindowScale = 1f;
        static readonly Rect DefaultSmallWindowRect = new Rect(0, 0, 60f, 60f);

        private Rect windowRect = DefaultWindowRect;
        private float windowScale = DefaultWindowScale;
        private Rect smallWindowRect = DefaultSmallWindowRect;

        private Rect dragRect = new Rect(0, 0, float.MaxValue, float.MaxValue);
        public bool DefaultSmallWindow = true;

        private List<IDebuggerWindow> windowList = new List<IDebuggerWindow>();
        private List<string> toolList = new List<string>();
        private int curSelectedWindowIndex;
        private ConsoleWindow consoleWindow = new ConsoleWindow();
        private InformationWindow informationWindow = new InformationWindow();
        private MemoryWindow memoryWindow = new MemoryWindow();
        private SettingsWindow settingWindow = new SettingsWindow();
        private FPSCounter fpsCounter = new FPSCounter();
        private void Start()
        {
            RegisterDebuggerWindow("<b>Console</b>", consoleWindow);
            RegisterDebuggerWindow("<b>Information</b>", informationWindow);
            RegisterDebuggerWindow("<b>Memory</b>", memoryWindow);
            RegisterDebuggerWindow("<b>Setting</b>", settingWindow);
            RegisterDebuggerWindow("<b>Close</b>", null);
        }
        private void Update()
        {
            fpsCounter.Update();
        }
        private void OnGUI()
        {
            GUI.matrix = Matrix4x4.Scale(new Vector3(windowScale, windowScale, 1f));
            if (DefaultSmallWindow)
            {
                smallWindowRect = GUILayout.Window(0, smallWindowRect, DrawSmallWindow, DefaultMiniWindowTitle);
            }
            else
            {
                windowRect = GUILayout.Window(0, windowRect, DrawWindow, DefaultWindowTitle);
            }
        }
        //绘制小窗口
        private void DrawSmallWindow(int windowId)
        {
            if (GUILayout.Button("FPS:" + fpsCounter.CurFps.ToString("F2"), GUILayout.Width(100f), GUILayout.Height(40f)))
            {
                DefaultSmallWindow = false;
            }
            GUI.DragWindow(dragRect);
        }
        //绘制默认窗口
        private void DrawWindow(int windowId)
        {
            int toolbarIndex = GUILayout.Toolbar(curSelectedWindowIndex, toolList.ToArray(), GUILayout.Height(30f));
            if (toolbarIndex >= windowList.Count)
            {
                DefaultSmallWindow = true;
            }
            else
            {
                windowList[toolbarIndex].OnDraw();
                curSelectedWindowIndex = toolbarIndex;
            }
            GUI.DragWindow(dragRect);
        }

        private void RegisterDebuggerWindow(string title, IDebuggerWindow window)
        {
            toolList.Add(title);
            if (window != null)
            {
                window.Initialize();
                windowList.Add(window);
            }
        }

        void OnDestory()
        {
            foreach (var item in windowList)
            {
                item.Close();
            }
        }
    }

}

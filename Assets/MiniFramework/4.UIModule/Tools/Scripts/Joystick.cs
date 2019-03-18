﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniFramework
{
    public enum JoyStickState
    {
        None,
        OnBeginDrag,
        OnDrag,
        OnEndDrag,
    }
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public RectTransform Rocker;//摇杆
        public RectTransform Base;//底座
        public float MaxDistance;//摇杆最大移动距离
        public Action OnBeginDragHandler;//摇杆开始拖拽事件
        public Action<Vector2> OnDragHandler;//摇杆拖拽中事件
        public Action OnEndDragHandler;//摇杆结束拖拽事件
        public JoyStickState CurState;//摇杆当前状态
        public Vector2 RockerDir { get { return Rocker.localPosition.normalized; } }//摇杆方向
        private Vector2 rockerPos;
        private Vector2 basePos;
        void Start()
        {
            basePos = Base.localPosition;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            rockerPos = Rocker.localPosition;
            if (OnBeginDragHandler != null)
            {
                OnBeginDragHandler();
            }
            CurState = JoyStickState.OnBeginDrag;
        }
        public void OnDrag(PointerEventData eventData)
        {
            rockerPos += eventData.delta;
            float distance = Vector2.Distance(rockerPos, Vector2.zero);
            if (distance > MaxDistance)
            {
                rockerPos = (MaxDistance / distance) * rockerPos;
            }
            Rocker.localPosition = rockerPos;
            if (OnDragHandler != null)
            {
                OnDragHandler(Rocker.localPosition.normalized);
            }
            CurState = JoyStickState.OnDrag;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Rocker.localPosition = Vector3.zero;
            if (OnEndDragHandler != null)
            {
                OnEndDragHandler();
            }
            CurState = JoyStickState.OnEndDrag;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Base, Input.mousePosition, eventData.enterEventCamera, out position);
            Base.localPosition = position + basePos;
            Base.gameObject.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Base.localPosition = basePos;
            Base.gameObject.SetActive(false);
        }
    }

}
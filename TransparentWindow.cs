using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll", SetLastError = true)] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("Dwmapi.dll")] private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    const uint LWA_COLORKEY = 0x00000001;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private IntPtr hWnd;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;

#if !UNITY_EDITOR_
        hWnd = GetActiveWindow();
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };

        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        
        //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }

    private void Update()
    {
        SetClickThrough(Physics2D.OverlapPoint(GetMousePosition()) == null);
    }

    private void SetClickThrough(bool clickThrough)
    {
        if (clickThrough)
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        else
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
    }

    private Vector3 GetMousePosition()
    {
        Vector2 mousePos = new()
        {
            x = Event.current.mousePosition.x,
            y = cam.pixelHeight - Event.current.mousePosition.y
        };

        return new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane);
    }
}

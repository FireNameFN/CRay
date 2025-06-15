using System.Threading.Tasks;

namespace CRay;

public sealed class CRayIconWindows {
    static nint Hwnd;

    public unsafe CRayIconWindows(string file) {
        nint icon;

        NativeWindows.ExtractIconExW(file, 0, null, &icon, 1);

        NativeWindows.NotifyIconData data = new() {
            Size = (uint)sizeof(NativeWindows.NotifyIconData),
            Hwnd = Hwnd,
            Flags = 3,
            CallbackMessage = 0x400,
            Icon = icon
        };

        _ = NativeWindows.Shell_NotifyIconW(0, data);
    }

    public static unsafe void Initialize() {
        NativeWindows.ClassEx classEx = new() {
            Size = (uint)sizeof(NativeWindows.ClassEx)
        };

        NativeWindows.RegisterClassExW(&classEx);

        Hwnd = NativeWindows.CreateWindowExW(0, "CRAY", null, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
    }

    static unsafe void Loop() {
        NativeWindows.Msg msg;

        while(NativeWindows.GetMessageW(&msg, 0, 0, 0) != 0) {
            _ = NativeWindows.TranslateMessage(&msg);

            NativeWindows.DispatchMessage(&msg);
        }
    }

    static nint Proc(nint hwnd, uint message, nint wParam, nint lParam) {
        switch(message) {
            case 0x010:
                _ = NativeWindows.DestroyWindow(hwnd);
                break;
            case 0x002:
                NativeWindows.PostQuitMessage(0);
                break;
            case 0x400:
                
                break;
        }

        return NativeWindows.DefWindowProcW(hwnd, message, wParam, lParam);
    }
}

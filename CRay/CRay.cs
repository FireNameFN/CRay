using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CRay;

public sealed class CRayIcon : IDisposable {
    public bool Visible {
        get => visible;
        set {
            visible = value;

            UpdateStatus();
        }
    }
    bool visible = true;

    public bool Attention {
        get => attention;
        set {
            attention = value;

            UpdateStatus();
        }
    }
    bool attention;

    public string IconsPath {
        get => iconsPath;
        set {
            if(iconsPath == value)
                return;

            iconsPath = value;

            Native.app_indicator_set_icon_theme_path(indicator, value);
        }
    }
    string iconsPath;

    public string IconName {
        get => iconName;
        set {
            if(iconName == value)
                return;

            iconName = value;

            Native.app_indicator_set_icon(indicator, value);
        }    
    }
    string iconName;

    public string AttentionIconName {
        get => attentionIconName;
        set {
            if(attentionIconName == value)
                return;

            attentionIconName = value;

            Native.app_indicator_set_icon(indicator, value);
        }    
    }
    string attentionIconName;

    readonly nint indicator;

    readonly nint menu;

    int status;

    public CRayIcon(string iconsPath, string iconName) {
        indicator = Native.app_indicator_new_with_path("test", iconName, 0, iconsPath);

        menu = Native.gtk_menu_new();

        Native.app_indicator_set_menu(indicator, menu);

        UpdateStatus();
    }

    public CRayIcon(string iconName = "icon") : this(AppContext.BaseDirectory, iconName) { }

    public static void Init() {
        Native.gtk_init(0, 0);

        Task.Factory.StartNew(Native.gtk_main, TaskCreationOptions.LongRunning);
    }

    public unsafe void AddMenuItem(string label, Action action) {
        nint item = Native.gtk_menu_item_new_with_mnemonic(label);

        Native.gtk_widget_show(item);

        nint handler = Marshal.GetFunctionPointerForDelegate(action);

        Native.g_signal_connect_data(item, "button-press-event", handler, 0, 0, 0);

        Native.gtk_menu_shell_append(menu, item);
    }

    void UpdateStatus() {
        int status = Visible ? Attention ? 2 : 1 : 0;

        if(this.status == status)
            return;

        this.status = status;

        Native.app_indicator_set_status(indicator, status);
    }

    public void Dispose() {
        Native.gtk_widget_destroy(menu);
    }
}

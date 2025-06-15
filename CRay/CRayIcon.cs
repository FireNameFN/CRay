using System;

namespace CRay;

public sealed class CRayIcon : ICRayIcon {
    public bool Visible { get => implementation.Visible; set => implementation.Visible = value; }

    public bool Attention { get => implementation.Attention; set => implementation.Attention = value; }

    public string IconsPath { get => implementation.IconsPath; set => implementation.IconsPath = value; }

    public string IconName { get => implementation.IconName; set => implementation.IconName = value; }

    public string AttentionIconName { get => implementation.AttentionIconName; set => implementation.AttentionIconName = value; }

    readonly ICRayIcon implementation;

    public CRayIcon(string iconsPath, string iconName) {
        if(OperatingSystem.IsWindows())
            implementation = new CRayIconWindows(iconsPath, iconName);
        else if(OperatingSystem.IsLinux())
            implementation = new CRayIconLinux(iconsPath, iconName);
        else
            throw new NotSupportedException();
    }

    public CRayIcon(string iconName = "icon") : this(AppContext.BaseDirectory, iconName) { }

    public static void Initialize() {
        if(OperatingSystem.IsLinux())
            CRayIconLinux.Initialize();
    }

    public void AddMenuItem(string label, Action action) {
        implementation.AddMenuItem(label, action);
    }

    public void Dispose() {
        implementation.Dispose();
    }
}

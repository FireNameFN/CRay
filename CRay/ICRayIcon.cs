using System;

namespace CRay;

public interface ICRayIcon : IDisposable {
    public bool Visible { get; set; }

    public bool Attention { get; set; }

    public string IconsPath { get; set; }

    public string IconName { get; set; }

    public string AttentionIconName { get; set; }

    public void AddMenuItem(string label, Action action);
}

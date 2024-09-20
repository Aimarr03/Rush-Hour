using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Navigation
{
    public void OnFocused();
    public void OnDefocused();
    public void OnTap();
    public void OnHold();
    public void OnCancelledHold();
}

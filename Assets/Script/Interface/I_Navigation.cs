using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Navigation
{
    public void State_Focused();
    public void State_Defocused();
    public void State_Tap();
    public void State_Hold();
    public void State_CancelledHold();
}

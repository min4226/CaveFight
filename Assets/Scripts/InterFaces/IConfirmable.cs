using UnityEngine;
using System;
public interface IConfirmable
{
    public void Confirm();

    public void SetConfirmAction(Action newAction);
}

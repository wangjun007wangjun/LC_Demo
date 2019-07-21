using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LeafButton : MonoBehaviour
{
    private enum State
    {
        Normal,Halving,Free
    }

    private State _state = State.Normal;
    public UnityEvent OnClick;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=>{OnClick?.Invoke();});
    }
}

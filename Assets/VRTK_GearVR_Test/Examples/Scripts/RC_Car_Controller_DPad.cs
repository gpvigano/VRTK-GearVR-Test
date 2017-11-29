using VRTK;
using VRTK.Examples;
using UnityEngine;

public class RC_Car_Controller_DPad : MonoBehaviour
{
    public GameObject rcCar;
    private RC_Car rcCarScript;
    bool dpadTouched = false;

    private void Start()
    {
        rcCarScript = rcCar.GetComponent<RC_Car>();
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);

        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

        GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoCarReset);
    }

    private void Update()
    {
        if (dpadTouched)
        {
            Vector2 pos = Input.mousePosition;
            pos.x = -(pos.x / Screen.width - 0.5f);
            pos.y = pos.y / Screen.height - 0.5f;
            rcCarScript.SetTouchAxis(pos);
        }
    }

    private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        dpadTouched = true;
    }

    private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.SetTouchAxis(Vector2.zero);
        dpadTouched = false;
    }


    private void DoCarReset(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.ResetCar();
        rcCarScript.SetTriggerAxis(0.5f);
    }
}

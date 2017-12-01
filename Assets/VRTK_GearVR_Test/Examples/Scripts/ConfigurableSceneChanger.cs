using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class ConfigurableSceneChanger : MonoBehaviour
{
    [Tooltip("Exit if changing to the next scene when the last one is loaded (else load the first one).")]
    public bool exitAfterLastScene = false;
    public SDK_BaseController.ButtonTypes cycleButton = SDK_BaseController.ButtonTypes.ButtonTwo;
    private bool canPress;
    private VRTK_ControllerReference controllerReference;

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Awake()
    {
        canPress = false;
        Invoke("ResetPress", 1f);
        DynamicGI.UpdateEnvironment();
    }

    private bool IsForwardPressed()
    {
        if (!VRTK_ControllerReference.IsValid(controllerReference))
        {
            return false;
        }
        if (canPress &&
            VRTK_SDK_Bridge.GetControllerButtonState(cycleButton, SDK_BaseController.ButtonPressTypes.Press, controllerReference))
        {
            return true;
        }
        return false;
    }


    private void ResetPress()
    {
        canPress = true;
    }

    private void Update()
    {
        GameObject rightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
        controllerReference = VRTK_ControllerReference.GetControllerReference(rightHand);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex;

        if (IsForwardPressed() || Input.GetKeyUp(KeyCode.Space))
        {
            nextSceneIndex++;
            if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                if (exitAfterLastScene)
                {
                    nextSceneIndex = currentSceneIndex;
                    Exit();
                }
                else
                {
                    nextSceneIndex = 0;
                }
            }
        }

        if (nextSceneIndex == currentSceneIndex)
        {
            return;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}

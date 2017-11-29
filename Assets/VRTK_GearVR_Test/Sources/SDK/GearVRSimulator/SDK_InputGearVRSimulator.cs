namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The `[VRGazeSimulator_CameraRig]` prefab is a mock Camera Rig set up that can be used to develop with VRTK without the need for VR Hardware.
    /// </summary>
    /// <remarks>
    /// Use the mouse and keyboard to move around both play area and hands and interacting with objects without the need of a hmd or VR controls.
    /// </remarks>
    public class SDK_InputGearVRSimulator : MonoBehaviour
    {
        /// <summary>
        /// Mouse input mode types
        /// </summary>
        public enum MouseInputMode
        {
            /// <summary>
            /// Mouse movement is always treated as mouse input.
            /// </summary>
            Always,
            /// <summary>
            /// Mouse movement is only treated as movement when a button is pressed.
            /// </summary>
            RequiresButtonPress
        }

        #region Public fields

        [Header("General Settings")]

        [Tooltip("Show control information in the upper left corner of the screen.")]
        public bool showControlHints = true;

        [Header("Mouse Cursor Lock Settings")]

        [Tooltip("Lock the mouse cursor to the game window.")]
        public bool lockMouseToView = true;
        [Tooltip("Whether the mouse movement always acts as input or requires a button press.")]
        public MouseInputMode mouseMovementInput = MouseInputMode.Always;

        [Header("Manual Adjustment Settings")]

        [Tooltip("Adjust player movement speed.")]
        public float playerMoveMultiplier = 5f;
        [Tooltip("Adjust player rotation speed.")]
        public float playerRotationMultiplier = 0.5f;
        [Tooltip("Adjust player sprint speed.")]
        public float playerSprintMultiplier = 2f;
        [Tooltip("Adjust the speed of the cursor movement in locked mode.")]
        public float lockedCursorMultiplier = 5f;

        [Header("Operation Key Binding Settings")]

        [Tooltip("Key used to enable mouse input if a button press is required.")]
        public KeyCode mouseMovementKey = KeyCode.Mouse1;
        [Tooltip("Key used to toggle control hints on/off.")]
        public KeyCode toggleControlHints = KeyCode.F1;
        [Tooltip("Key used to toggle control hints on/off.")]
        public KeyCode toggleMouseLock = KeyCode.F4;

        [Header("Movement Key Binding Settings (not availeble on GearVR)")]

        [Tooltip("Key used to move forward.")]
        public KeyCode moveForward = KeyCode.W;
        [Tooltip("Key used to move to the left.")]
        public KeyCode moveLeft = KeyCode.A;
        [Tooltip("Key used to move backwards.")]
        public KeyCode moveBackward = KeyCode.S;
        [Tooltip("Key used to move to the right.")]
        public KeyCode moveRight = KeyCode.D;
        [Tooltip("Key used to sprint.")]
        public KeyCode sprint = KeyCode.LeftShift;

        [Header("Controller Key Binding Settings")]

        [Tooltip("Key used to simulate touchpad button.")]
        public KeyCode touchpadAlias = KeyCode.Mouse0;
        [Tooltip("Key used to simulate back button.")]
        public KeyCode backAlias = KeyCode.Backspace;
        [Tooltip("Key used to simulate start menu button.")]
        public KeyCode startMenuAlias = KeyCode.F;

        #endregion
        #region Protected fields

        protected GameObject hintCanvas;
        protected Text hintText;
        protected Transform gazeTransform;
        protected Vector3 oldPos;
        protected Transform neck;
        protected SDK_ControllerGearVRSim gazeController;
        protected static GameObject cachedCameraRig;
        protected static bool destroyed = false;
        protected float sprintMultiplier = 1;
        protected GameObject crossHairPanel;

        #endregion

        /// <summary>
        /// The FindInScene method is used to find the `[VRGazeSimulator_CameraRig]` GameObject within the current scene.
        /// </summary>
        /// <returns>Returns the found `[VRGazeSimulator_CameraRig]` GameObject if it is found. If it is not found then it prints a debug log error.</returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null && !destroyed)
            {
                cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_InputGearVRSimulator>();
                if (!cachedCameraRig)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "[GearVRSimulator_CameraRig]", "SDK_InputGearVRSimulator", ". check that the `VRTK_GearVR_Test/Prefabs/CameraRigs/[GearVRSimulator_CameraRig]` prefab been added to the scene."));
                }
            }
            return cachedCameraRig;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            hintCanvas = transform.Find("Canvas/Control Hints").gameObject;
            crossHairPanel = transform.Find("Canvas/CrosshairPanel").gameObject;
            hintText = hintCanvas.GetComponentInChildren<Text>();
            hintCanvas.SetActive(showControlHints);
            gazeTransform = transform.Find(SDK_GearVRSimController.GAZE_CONTROLLER_NAME);
            oldPos = Input.mousePosition;
            neck = transform.Find("Neck");
            gazeController = gazeTransform.GetComponent<SDK_ControllerGearVRSim>();
            gazeController.selected = true;
            destroyed = false;

            SDK_GearVRSimController controllerSDK = VRTK_SDK_Bridge.GetControllerSDK() as SDK_GearVRSimController;
            if (controllerSDK != null)
            {
                Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>()
                {
                    {"TouchpadPress", touchpadAlias },
                    {"ButtonTwo", backAlias },
                    {"StartMenu", startMenuAlias },
                };
                controllerSDK.SetKeyMappings(keyMappings);
            }
            gazeTransform.gameObject.SetActive(true);
            crossHairPanel.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
            destroyed = true;
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(toggleControlHints))
            {
                showControlHints = !showControlHints;
                hintCanvas.SetActive(showControlHints);
            }

            if (Input.GetKeyDown(toggleMouseLock))
            {
                lockMouseToView = !lockMouseToView;
            }

            if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
            {
                if (lockMouseToView)
                {
                    Cursor.lockState = Input.GetKey(mouseMovementKey) ? CursorLockMode.Locked : CursorLockMode.None;
                }
                else if (Input.GetKeyDown(mouseMovementKey))
                {
                    oldPos = Input.mousePosition;
                }
            }
            else
            {
                Cursor.lockState = lockMouseToView ? CursorLockMode.Locked : CursorLockMode.None;
            }

            UpdateRotation();
            if (Input.GetKey(sprint))
            {
                sprintMultiplier = playerSprintMultiplier;
            }
            else
            {
                sprintMultiplier = 1;
            }
            UpdatePosition();

            if (showControlHints)
            {
                UpdateHints();
            }
        }


        protected virtual void UpdateRotation()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (IsAcceptingMouseInput())
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                rot.y += (mouseDiff * playerRotationMultiplier).x;
                transform.localRotation = Quaternion.Euler(rot);

                rot = neck.rotation.eulerAngles;

                if (rot.x > 180)
                {
                    rot.x -= 360;
                }

                if (rot.x < 80 && rot.x > -80)
                {
                    rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
                    rot.x = Mathf.Clamp(rot.x, -79, 79);
                    neck.rotation = Quaternion.Euler(rot);
                }
            }
        }

        protected virtual void UpdatePosition()
        {
            float moveMod = Time.deltaTime * playerMoveMultiplier * sprintMultiplier;
            if (Input.GetKey(moveForward))
            {
                transform.Translate(transform.forward * moveMod, Space.World);
            }
            else if (Input.GetKey(moveBackward))
            {
                transform.Translate(-transform.forward * moveMod, Space.World);
            }
            if (Input.GetKey(moveLeft))
            {
                transform.Translate(-transform.right * moveMod, Space.World);
            }
            else if (Input.GetKey(moveRight))
            {
                transform.Translate(transform.right * moveMod, Space.World);
            }
        }

        protected virtual void UpdateHints()
        {
            string hints = "";
            Func<KeyCode, string> key = (k) => "<b>" + k.ToString() + "</b>";

            // WASD Movement
            string movementKeys = moveForward.ToString() + moveLeft.ToString() + moveBackward.ToString() + moveRight.ToString();
            hints += "Toggle Control Hints: " + key(toggleControlHints) + "\n\n";
            hints += "Toggle Mouse Lock: " + key(toggleMouseLock) + "\n";
            hints += "Move Player/Playspace: <b>" + movementKeys + "</b>\n";
            hints += "Sprint Modifier: (" + key(sprint) + ")\n\n";

            hints += "D-Pad Touch: " + key(touchpadAlias) + "\n";

            hints += "Back Button Press: " + key(backAlias) + "\n";
            hints += "Start Menu: " + key(startMenuAlias) + "\n";

            hintText.text = hints.TrimEnd();
        }

        protected virtual bool IsAcceptingMouseInput()
        {
            return mouseMovementInput == MouseInputMode.Always || Input.GetKey(mouseMovementKey);
        }

        protected virtual Vector3 GetMouseDelta()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * lockedCursorMultiplier;
            }
            else
            {
                Vector3 mouseDiff = Input.mousePosition - oldPos;
                oldPos = Input.mousePosition;
                return mouseDiff;
            }
        }
    }
}

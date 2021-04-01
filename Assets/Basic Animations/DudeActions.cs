// GENERATED AUTOMATICALLY FROM 'Assets/Basic Animations/DudeActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DudeActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DudeActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DudeActions"",
    ""maps"": [
        {
            ""name"": ""CharacterControls"",
            ""id"": ""e967d49f-ea3e-4efb-9af9-2d6944b3841d"",
            ""actions"": [
                {
                    ""name"": ""ZAxis"",
                    ""type"": ""Value"",
                    ""id"": ""b74e35bc-496e-4912-8c8f-fa51b60b038c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""XAxis"",
                    ""type"": ""Value"",
                    ""id"": ""3d425cca-cdab-46da-814d-a5dfcd3ac209"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""31bd0bbd-9b79-467e-a26a-6fe6f7226f5a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""Button"",
                    ""id"": ""d89e576c-b2ed-4a15-8b20-8014792d8e64"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""80734957-aa07-4e80-8084-80a91e11bb05"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ragdoll"",
                    ""type"": ""Button"",
                    ""id"": ""8a5269b9-ca84-440c-bebe-c8453cf92db0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AltPunch"",
                    ""type"": ""Button"",
                    ""id"": ""d54b41c2-3243-43e3-a92d-e87e608d6536"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""efe306ab-0005-4289-819a-00e1ae3670ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""f3510ac0-bdf8-4de4-b16f-3a9e748e08bd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""9e102ac7-b260-4ed8-8677-1daa4555140f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZAxis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7598f824-ef73-48c6-a2f3-748ce1d26ce9"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""64398d53-afd5-404c-8ce6-6ea7588955eb"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""1aaba289-a642-46ec-917e-92fe226ce178"",
                    ""path"": ""1DAxis"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""XAxis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""702142a9-7eba-49fb-b931-17acbb2ba709"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""XAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9aa607c2-103b-41e6-82e1-062e5edaefcf"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""XAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""33649ba7-cdc1-45cf-8c12-8b7f5c2d834a"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52e7b34e-9040-448e-9a73-6a6ae0e6600f"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34d4e380-f286-4fd6-a0f0-985688238a44"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""426cf7a1-a105-47e0-9de8-6ea55ad72777"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ragdoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c79d6f63-7154-4958-98eb-6be251f9fe9f"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltPunch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db668ad7-0be9-4d3a-a45c-bf5011447c29"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46aaa361-937f-4025-aaa7-4f220a3a8ba8"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardAndMouse"",
            ""bindingGroup"": ""KeyboardAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // CharacterControls
        m_CharacterControls = asset.FindActionMap("CharacterControls", throwIfNotFound: true);
        m_CharacterControls_ZAxis = m_CharacterControls.FindAction("ZAxis", throwIfNotFound: true);
        m_CharacterControls_XAxis = m_CharacterControls.FindAction("XAxis", throwIfNotFound: true);
        m_CharacterControls_Run = m_CharacterControls.FindAction("Run", throwIfNotFound: true);
        m_CharacterControls_Roll = m_CharacterControls.FindAction("Roll", throwIfNotFound: true);
        m_CharacterControls_Punch = m_CharacterControls.FindAction("Punch", throwIfNotFound: true);
        m_CharacterControls_Ragdoll = m_CharacterControls.FindAction("Ragdoll", throwIfNotFound: true);
        m_CharacterControls_AltPunch = m_CharacterControls.FindAction("AltPunch", throwIfNotFound: true);
        m_CharacterControls_Jump = m_CharacterControls.FindAction("Jump", throwIfNotFound: true);
        m_CharacterControls_Shoot = m_CharacterControls.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CharacterControls
    private readonly InputActionMap m_CharacterControls;
    private ICharacterControlsActions m_CharacterControlsActionsCallbackInterface;
    private readonly InputAction m_CharacterControls_ZAxis;
    private readonly InputAction m_CharacterControls_XAxis;
    private readonly InputAction m_CharacterControls_Run;
    private readonly InputAction m_CharacterControls_Roll;
    private readonly InputAction m_CharacterControls_Punch;
    private readonly InputAction m_CharacterControls_Ragdoll;
    private readonly InputAction m_CharacterControls_AltPunch;
    private readonly InputAction m_CharacterControls_Jump;
    private readonly InputAction m_CharacterControls_Shoot;
    public struct CharacterControlsActions
    {
        private @DudeActions m_Wrapper;
        public CharacterControlsActions(@DudeActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ZAxis => m_Wrapper.m_CharacterControls_ZAxis;
        public InputAction @XAxis => m_Wrapper.m_CharacterControls_XAxis;
        public InputAction @Run => m_Wrapper.m_CharacterControls_Run;
        public InputAction @Roll => m_Wrapper.m_CharacterControls_Roll;
        public InputAction @Punch => m_Wrapper.m_CharacterControls_Punch;
        public InputAction @Ragdoll => m_Wrapper.m_CharacterControls_Ragdoll;
        public InputAction @AltPunch => m_Wrapper.m_CharacterControls_AltPunch;
        public InputAction @Jump => m_Wrapper.m_CharacterControls_Jump;
        public InputAction @Shoot => m_Wrapper.m_CharacterControls_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControlsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControlsActions instance)
        {
            if (m_Wrapper.m_CharacterControlsActionsCallbackInterface != null)
            {
                @ZAxis.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnZAxis;
                @ZAxis.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnZAxis;
                @ZAxis.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnZAxis;
                @XAxis.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnXAxis;
                @XAxis.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnXAxis;
                @XAxis.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnXAxis;
                @Run.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRun;
                @Roll.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRoll;
                @Roll.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRoll;
                @Roll.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRoll;
                @Punch.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnPunch;
                @Punch.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnPunch;
                @Punch.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnPunch;
                @Ragdoll.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRagdoll;
                @Ragdoll.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRagdoll;
                @Ragdoll.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnRagdoll;
                @AltPunch.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnAltPunch;
                @AltPunch.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnAltPunch;
                @AltPunch.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnAltPunch;
                @Jump.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnJump;
                @Shoot.started -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_CharacterControlsActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_CharacterControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ZAxis.started += instance.OnZAxis;
                @ZAxis.performed += instance.OnZAxis;
                @ZAxis.canceled += instance.OnZAxis;
                @XAxis.started += instance.OnXAxis;
                @XAxis.performed += instance.OnXAxis;
                @XAxis.canceled += instance.OnXAxis;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @Roll.started += instance.OnRoll;
                @Roll.performed += instance.OnRoll;
                @Roll.canceled += instance.OnRoll;
                @Punch.started += instance.OnPunch;
                @Punch.performed += instance.OnPunch;
                @Punch.canceled += instance.OnPunch;
                @Ragdoll.started += instance.OnRagdoll;
                @Ragdoll.performed += instance.OnRagdoll;
                @Ragdoll.canceled += instance.OnRagdoll;
                @AltPunch.started += instance.OnAltPunch;
                @AltPunch.performed += instance.OnAltPunch;
                @AltPunch.canceled += instance.OnAltPunch;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public CharacterControlsActions @CharacterControls => new CharacterControlsActions(this);
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardAndMouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    public interface ICharacterControlsActions
    {
        void OnZAxis(InputAction.CallbackContext context);
        void OnXAxis(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnRoll(InputAction.CallbackContext context);
        void OnPunch(InputAction.CallbackContext context);
        void OnRagdoll(InputAction.CallbackContext context);
        void OnAltPunch(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}

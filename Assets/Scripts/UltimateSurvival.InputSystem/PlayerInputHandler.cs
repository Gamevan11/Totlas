using System;
using UnityEngine;
using UltimateSurvival.InputSystem;

namespace UltimateSurvival
{
	/// <summary>
	/// Handles the player input, and feeds it to the other components.
	/// </summary>
	public class PlayerInputHandler : PlayerBehaviour
	{
        private InputManager m_Input;

        [SerializeField] private Camera _camera;
        [SerializeField] private FloatingJoystick Joystic;
        [SerializeField] private FixedTouchField Touch;

        [SerializeField] private GameObject BuildMenuButton;
        [SerializeField] private GameObject PlaceButton;
        [SerializeField] private GameObject ShotButton;

        [SerializeField] private GameObject InteractButton;
        [SerializeField] private GameObject PickupButton;

        [SerializeField] private LayerMask RayLayer;

        public static bool openAim;
        public static bool SlotTake;

        private bool _attack;
        private bool isPC = false;

        private void Awake()
        {
            if (GameController.InputManager)
                m_Input = GameController.InputManager;
            else
                enabled = false;

            Player.PlaceObject.AddListener(OnSucceded_PlaceObject);
        }

        private void Update()
        {
            #if UNITY_EDITOR
            isPC = true;
            #endif

            // Aim.
            //if (m_Input.GetButtonDown("Aim"))
            //    Player.Aim.TryStart();
            //else if (m_Input.GetButtonUp("Aim"))
            //    Player.Aim.ForceStop();

            //if (!SlotTake)
            //    {
            //        Player.Aim.ForceStop();
            //        SlotTake = true;
            //        openAim = false;
            //    }


            if (isPC)
            {
                #region PC

                if (!InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Close Inventory"))
                    InventoryController.Instance.SetState.Try(ET.InventoryState.Closed);
                if (InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Open Inventory"))
                    InventoryController.Instance.SetState.Try(ET.InventoryState.Normal);

                Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Player.MovementInput.Set(moveInput);

                var rotateObjectAxis = m_Input.GetAxisRaw("Rotate Object");
                if (Mathf.Abs(rotateObjectAxis) > 0f)
                    Player.RotateObject.Try(rotateObjectAxis);

                if (m_Input.GetButtonDown("Interact"))
                    Player.InteractOnce.Try();

                // Interact continuously.
                Player.InteractContinuously.Set(m_Input.GetButton("Interact"));

                // Run.
                bool sprintButtonHeld = m_Input.GetButton("Run");
                bool canStartSprinting = Player.IsGrounded.Get() && Player.MovementInput.Get().y > 0f;

                if (!Player.Run.Active && sprintButtonHeld && canStartSprinting)
                    Player.Run.TryStart();

                if (Player.Run.Active && !sprintButtonHeld)
                    Player.Run.ForceStop();

                //
                if (m_Input.GetButtonDown("Select Buildable"))
                {
                    if (!Player.SelectBuildable.Active)
                        Player.SelectBuildable.TryStart();
                    else
                        Player.SelectBuildable.TryStop();
                }

                if (m_Input.GetButtonDown("Attack"))
                {
                    OnAttackOpen();
                }

                if (m_Input.GetButtonUp("Attack"))
                {
                    OnAttackClose();
                }

                if (m_Input.GetButtonDown("Aim"))
                {
                    OnAim();
                }

                if (m_Input.GetButtonDown("Jump"))
                {
                    OnJump();
                }

                if (m_Input.GetButtonDown("Crouch"))
                {
                    Crouch();
                }

                if (m_Input.GetButtonDown("Place Object"))
                {
                    if (Player.CanShowObjectPreview.Is(true))
                        Player.PlaceObject.Try();
                }
                else
                    Player.CanShowObjectPreview.Set(true);

                var scrollWheelValue = m_Input.GetAxis("Mouse ScrollWheel");
                Player.ScrollValue.Set(scrollWheelValue);

                #endregion
            }
            else
            {
                Vector2 moveInput = new Vector2(Joystic.Horizontal, Joystic.Vertical);
                Player.MovementInput.Set(moveInput);

                if (Player.EquippedItem.Get() && (Player.EquippedItem.Get().HasProperty("Allows Building") || Player.EquippedItem.Get().ItemData.IsBuildable))
                {
                    if (Player.EquippedItem.Get().ItemData.IsBuildable)
                    {
                        PlaceButton.SetActive(true);
                        ShotButton.SetActive(false);
                        BuildMenuButton.SetActive(false);
                    }
                    else
                    {
                        PlaceButton.SetActive(true);
                        ShotButton.SetActive(false);
                        BuildMenuButton.SetActive(true);
                    }
                }
                else
                {
                    if (Player.SelectBuildable.Active)
                    {
                        Player.SelectBuildable.TryStop();
                    }

                    PlaceButton.SetActive(false);
                    ShotButton.SetActive(true);
                    BuildMenuButton.SetActive(false);
                }

                Player.CanShowObjectPreview.Set(true);
            }


            if (_attack)
            {
                Player.AttackOnce.Try();
            }

            var ray = _camera.ViewportPointToRay(Vector2.one * 0.5f);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 2f, RayLayer, QueryTriggerInteraction.Ignore))
            {
                var raycastData = new RaycastData(hitInfo);

                if (raycastData.InteractableObject && hitInfo.collider.GetComponent<ItemPickup>())
                {
                    PickupButton.SetActive(true);
                    InteractButton.SetActive(false);
                }
                else if(raycastData.InteractableObject)
                {
                    InteractButton.SetActive(true);
                    PickupButton.SetActive(false);
                }
                else
                {
                    InteractButton.SetActive(false);
                    PickupButton.SetActive(false);
                }

            }
            else
            {
                PickupButton.SetActive(false);
                InteractButton.SetActive(false);
            }

            // Interact continuously.
            //Player.InteractContinuously.Set(m_Input.GetButton("Interact"));

        }

        private void OnSucceded_PlaceObject()
        {
            Player.CanShowObjectPreview.Set(false);
        }

        public void Crouch()
        {
            if (!Player.Crouch.Active)
                Player.Crouch.TryStart();
            else
                Player.Crouch.TryStop();
        }

        public void Interact()
        {
            Player.InteractOnce.Try();
        }

        public void Build()
        {
            Player.PlaceObject.Try();
        }

        public void BuildMenu()
        {
            if (!Player.SelectBuildable.Active)
                Player.SelectBuildable.TryStart();
            else
                Player.SelectBuildable.TryStop();
        }

        // Кнопка прыжка
        public void OnJump()
        {
            Player.Jump.TryStart();
        }

        // Инвентарь
        public void OnInventory()
        {
            if (!InventoryController.Instance.IsClosed)
                InventoryController.Instance.SetState.Try(ET.InventoryState.Closed);
            if (InventoryController.Instance.IsClosed)
                InventoryController.Instance.SetState.Try(ET.InventoryState.Normal);
        }

        // Атака
        public void OnAttackOpen()
        {
            _attack = true;
        }

        public void OnAttackClose()
        {
            _attack = false;
        }

        // Прицел
        public void OnAim()
        {
            // Нажатие на кнопку
            openAim = !openAim;

            if (openAim)
                Player.Aim.TryStart();
            else
                Player.Aim.ForceStop();
        }
    }
}

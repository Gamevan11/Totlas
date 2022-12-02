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

        public FloatingJoystick Joystic;
        public FixedTouchField Touch;

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
            // Inventory.
            if (!InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Close Inventory"))
                InventoryController.Instance.SetState.Try(ET.InventoryState.Closed);
            if (InventoryController.Instance.IsClosed && m_Input.GetButtonDown("Open Inventory"))
                InventoryController.Instance.SetState.Try(ET.InventoryState.Normal);

             var rotateObjectAxis = m_Input.GetAxisRaw("Rotate Object");
             if(Mathf.Abs(rotateObjectAxis) > 0f)
             	Player.RotateObject.Try(rotateObjectAxis);

             if(m_Input.GetButtonDown("Select Buildable"))
             {
             	if(!Player.SelectBuildable.Active)
             		Player.SelectBuildable.TryStart();
             	else
             		Player.SelectBuildable.TryStop();
             }

            // Movement.

            // Look.

            #if UNITY_EDITOR
            isPC = true;
            #endif

            if (!isPC)
            {
                Vector2 moveInput = new Vector2(Joystic.Horizontal, Joystic.Vertical);
                Player.MovementInput.Set(moveInput);

                Player.LookInput.Set(new Vector2(Touch.TouchDist.x, Touch.TouchDist.y));
            }
            else
            {
                Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Player.MovementInput.Set(moveInput);
            }

            // Interact once.
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

            if (m_Input.GetButtonDown("Crouch"))
            {
                if (!Player.Crouch.Active)
                    Player.Crouch.TryStart();
                else
                    Player.Crouch.TryStop();
            }

            // Attack
            if (_attack)
            {
                openAim = false;
                Player.AttackOnce.Try();
            }

            // Aim.
            //if (m_Input.GetButtonDown("Aim"))
            //    Player.Aim.TryStart();
            //else if (m_Input.GetButtonUp("Aim"))
            //    Player.Aim.ForceStop();

            if (m_Input.GetButtonDown("Place Object"))
            {
                if (Player.CanShowObjectPreview.Is(true))
                    Player.PlaceObject.Try();
            }
            else
                Player.CanShowObjectPreview.Set(true);

            var scrollWheelValue = m_Input.GetAxis("Mouse ScrollWheel");
            Player.ScrollValue.Set(scrollWheelValue);

            //if (!SlotTake)
            //    {
            //        Player.Aim.ForceStop();
            //        SlotTake = true;
            //        openAim = false;
            //    }
        }

        private void OnSucceded_PlaceObject()
        {
            Player.CanShowObjectPreview.Set(false);
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
        public void OnAttackOpen() =>
            _attack = true;

        public void OnAttackClose() =>
            _attack = false;

        // Прицел
        public void OnAim()
        {
            // Нажатие на кнопку
            openAim = !openAim;
            print(openAim);

            if (openAim)
                Player.Aim.TryStart();
            else
                Player.Aim.ForceStop();
        }
    }
}

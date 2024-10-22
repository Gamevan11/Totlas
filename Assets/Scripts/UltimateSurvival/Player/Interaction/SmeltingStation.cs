﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	public enum SmeltingStationType { Furnace, Campfire }

	/// <summary>
	/// 
	/// </summary>
	public class SmeltingStation : InteractableObject, IInventoryTrigger
	{
		const float UPDATE_INTERVAL = 0.1f;

		public Value<bool> IsBurning = new Value<bool>(false);

		public Value<float> Progress = new Value<float>(0f);

		public Message BurnedItem = new Message();

		public ItemHolder FuelSlot { get; private set; }
		public ItemHolder InputSlot { get; private set; }
		public List<ItemHolder> LootSlots { get; private set; }

		[SerializeField]
		private SmeltingStationType m_Type;

		[SerializeField]
		[Range(1, 18)]
		private int m_LootContainerSize = 6;

		[Header("Fire")]

		[SerializeField]
		private ParticleSystem m_Fire;

		[SerializeField]
		private AudioSource m_FireSource;

		[SerializeField]
		private Firelight m_FireLight;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FireVolume = 0.6f;

		[Header("Damage (Optional)")]

		[SerializeField]
		private DamageArea m_DamageArea;

		private WaitForSeconds m_UpdateInterval = new WaitForSeconds(UPDATE_INTERVAL);
		private Coroutine m_BurningHandler;
		private ItemProperty.Value m_BurnTimeProperty;
		private ItemProperty.Value m_FuelTimeProperty;
        private ItemProperty.Value p_BurnTimeProperty;
        private string m_ItemResult;

        private SavableItem lastFuelItem = null;
        private SavableItem lastInputItem = null;

        List<string> Names = new List<string>();
        List<bool> CanAdd = new List<bool>();
        bool hasEmptySlots = false;
        bool hasSameItem = false;
		bool forceStop = false;

        public override void OnInteract(PlayerEventHandler player)
		{
			if(m_Type == SmeltingStationType.Campfire)
				InventoryController.Instance.OpenCampfire.Try(this);
			else if(m_Type == SmeltingStationType.Furnace)
				InventoryController.Instance.OpenFurnace.Try(this);
		}

		private void Start()
		{
			FuelSlot = new ItemHolder();
			InputSlot = new ItemHolder();
			FuelSlot.Updated.AddListener(On_HolderUpdated);
			InputSlot.Updated.AddListener(On_HolderUpdated);

			// Initialize the loot container (create the empty slots).
			LootSlots = new List<ItemHolder>();

			for(int i = 0;i < m_LootContainerSize;i ++)
			{
                LootSlots.Add(new ItemHolder());
				LootSlots[LootSlots.Count - 1].Updated.AddListener(On_HolderUpdated);
            }

			IsBurning.AddChangeListener(OnChanged_IsBurning);
			IsBurning.SetAndForceUpdate(false);
		}

		private void On_HolderUpdated(ItemHolder holder)
		{
			bool shouldStopBurning = false;
			hasEmptySlots = false;
			hasSameItem = false;

            Names.Clear();
            CanAdd.Clear();

            #region LootSlots

            for (int i = 0; i < LootSlots.Count; i++)
            {
				for (int y = 0; y < LootSlots.Count; y++)
				{
                    if (LootSlots[y].CurrentItem == null)
					{
						hasEmptySlots = true;
						break;
					}
                }

				if (hasEmptySlots) { break; }

                Names.Add(LootSlots[i].CurrentItem.Name);
                if (LootSlots[i].CurrentItem.CurrentInStack < LootSlots[i].CurrentItem.ItemData.StackSize)
                {
                    CanAdd.Add(true);
                }
                else
                {
                    CanAdd.Add(false);
                }
 
            }

            ItemData data = null;

            if (InputSlot.CurrentItem != null)
            {
                GameController.ItemDatabase.FindItemByName(InputSlot.CurrentItem.GetPropertyValue("Burn Result").String, out data);
            }

            for (int x = 0; x < Names.Count; x++)
            {
                if (!hasEmptySlots && InputSlot.CurrentItem != null && data != null && data.Name == Names[x] && CanAdd[x])
                {
                    hasSameItem = true;
                    break;
                }
            }

            if (!hasEmptySlots && !hasSameItem)
            {
				forceStop = true;
                StopBurning();
                return;
			}

            #endregion

            if (FuelSlot.HasItem && InputSlot.HasItem)
			{
				// The items have the correct properties, and the burn process can begin.
				if(FuelSlot.CurrentItem.HasProperty("Fuel Time") && InputSlot.CurrentItem.HasProperty("Burn Time") && InputSlot.CurrentItem.HasProperty("Burn Result"))
				{
					if(InputSlot.CurrentItem != lastInputItem || FuelSlot.CurrentItem != lastFuelItem || (forceStop && (hasEmptySlots || hasSameItem)))
					{
						forceStop = false;
						StopBurning();

						if (lastInputItem != null)
						{
							p_BurnTimeProperty = lastInputItem.GetPropertyValue("Burn Time");
							var burnTime = p_BurnTimeProperty.Float;
							burnTime.Current = burnTime.Default;
							p_BurnTimeProperty.SetValue(ItemProperty.Type.Float, burnTime);
						}

						lastInputItem = InputSlot.CurrentItem;
                        lastFuelItem = FuelSlot.CurrentItem;

						m_FuelTimeProperty = FuelSlot.CurrentItem.GetPropertyValue("Fuel Time");
						m_BurnTimeProperty = InputSlot.CurrentItem.GetPropertyValue("Burn Time");
                        m_ItemResult = InputSlot.CurrentItem.GetPropertyValue("Burn Result").String;

						IsBurning.Set(true);
						m_BurningHandler = StartCoroutine(C_Burn());

						return;
					}
				}
				else
					shouldStopBurning = true;
			}
			else
			{
                lastInputItem = null;
                lastFuelItem = null;
                shouldStopBurning = true;
            }

			if(IsBurning.Is(true) && shouldStopBurning)
				StopBurning();
		}

		private void OnChanged_IsBurning()
		{
			if(IsBurning.Is(true))
			{
				m_Fire.Play(true);
				GameController.Audio.LerpVolumeOverTime(m_FireSource, m_FireVolume, 3f);
				if(m_DamageArea)
					m_DamageArea.Active = true;
			}
			else
			{
				m_Fire.Stop(true);
				GameController.Audio.LerpVolumeOverTime(m_FireSource, 0f, 3f);
				if(m_DamageArea)
					m_DamageArea.Active = false;

				Progress.Set(0f);
			}

			m_FireLight.Toggle(IsBurning.Get());
		}

		private IEnumerator C_Burn()
		{
			while(true)
			{
				yield return m_UpdateInterval;

				// If the fuel, or the items to burn finished, stop burning.
				if(!FuelSlot.CurrentItem || !InputSlot.CurrentItem)
				{
					StopBurning();
					yield break;
				}

				var burnTime = m_BurnTimeProperty.Float;
				burnTime.Current -= UPDATE_INTERVAL;
				m_BurnTimeProperty.SetValue(ItemProperty.Type.Float, burnTime);

				Progress.Set(1f - burnTime.Ratio);

				if(burnTime.Current <= 0f)
				{
					ItemData resultedItem;
                    if (GameController.ItemDatabase.FindItemByName(m_ItemResult, out resultedItem))
                    {
                        if (!CollectionUtils.AddItem(resultedItem, 1, LootSlots))
                        {
							Debug.Log(resultedItem.Name + " " + LootSlots.Count);
                            StopBurning();
                            yield break;
                        }
                    }
                    else
						Debug.LogWarning("The item has burned but no result was given, make sure the item has the 'Burn Result' property, so we know what to add as a result of burning / smelting.", this);

					if(InputSlot.CurrentItem.CurrentInStack == 1)
					{
						InputSlot.SetItem(null);
						StopBurning();
						yield break;
					}
					else
					{
						burnTime.Current = burnTime.Default;
						m_BurnTimeProperty.SetValue(ItemProperty.Type.Float, burnTime);
						InputSlot.CurrentItem.CurrentInStack --;
					}	
				}
					
				var fuelTime = m_FuelTimeProperty.Float;
				fuelTime.Current -= UPDATE_INTERVAL;
				m_FuelTimeProperty.SetValue(ItemProperty.Type.Float, fuelTime);

				if(fuelTime.Current <= 0f)
				{
					if(FuelSlot.CurrentItem.CurrentInStack == 1)
					{
						FuelSlot.SetItem(null);
						StopBurning();
						yield break;
					}
					else
					{
						fuelTime.Current = fuelTime.Default;
						m_FuelTimeProperty.SetValue(ItemProperty.Type.Float, fuelTime);
						FuelSlot.CurrentItem.CurrentInStack --;
					}	
				}
			}
		}

		private void StopBurning()
		{
			m_FuelTimeProperty = null;
			m_BurnTimeProperty = null;
			m_ItemResult = string.Empty;

			IsBurning.Set(false);
			if(m_BurningHandler != null)
				StopCoroutine(m_BurningHandler);
			m_BurningHandler = null;
		}
	}
}

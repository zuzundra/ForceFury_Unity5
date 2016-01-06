using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitSlotManager : MonoBehaviour
{
    public event EventHandler UnitSlotIsSelected;

    [SerializeField]
    private UIUnitSlot[] _unitSlots = null;
    private ArrayRO<UIUnitSlot> _unitSlotsRO = null;
    public ArrayRO<UIUnitSlot> UnitSlotsRO
    {
        get
        {
            if (_unitSlotsRO == null)
            {
                _unitSlotsRO = new ArrayRO<UIUnitSlot>(_unitSlots);
            }
            return _unitSlotsRO;
        }
    }

    UIUnitSlot SelectedSlot
    {
        get
        {
            for (int i = 0; i < UnitSlotsRO.Length; i++)
            {
                UIUnitSlot slot = UnitSlotsRO[i];
                if (slot != null && slot.IsSelected)
                    return slot;
            }
            return null;
        }
    }

    public int LeaderShipCostSum
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < UnitSlotsRO.Length; i++)
            {
                UIUnitSlot slot = UnitSlotsRO[i];
                if (slot != null && slot.UnitData != null)
                    sum += slot.UnitData.LeadershipCost;
            }
            return sum;
        }
    }

    public void Awake()
    {
        for (int i = 0; i < UnitSlotsRO.Length; i++)
        {
            UIUnitSlot slot = UnitSlotsRO[i];
            if (slot != null)
                slot.SlotIsSelected += new EventHandler(slot_SlotIsSelected);
        }
    }

    public void SetHeroTemplate()
    {
        UIUnitSlot firstUISlot = null;
        BaseHeroData heroData = Global.Instance.Player.Heroes.Current.Data;
        for (int i = 0; i < heroData.SlotTemplate.Length; i++)
        {
            UnitSlot slot = heroData.SlotTemplate[i];
            if (slot != null)
            {
                UIUnitSlot uiSlot = GetUIUnitSlot(slot.Place);
                if (uiSlot != null)
                {
                    BaseSoldierData soldierData = UnitsConfig.Instance.GetSoldierData(slot.Unit);
                    uiSlot.SetUnitData(soldierData);
                    if (firstUISlot == null)
                    {
                        uiSlot.SelectSlot(true);
                        firstUISlot = uiSlot;
                    }                    
                }
            }
        }
    }

    UIUnitSlot GetUIUnitSlot(UnitPlace place)
    {
        for (int i = 0; i < UnitSlotsRO.Length; i++)
        {
            UIUnitSlot slot = UnitSlotsRO[i];
            if (slot != null && slot.Place.Range == place.Range && slot.Place.Position == place.Position)
                return slot;
        }
        return null;
    }

    public void ChangeCurrentUnit()
    {
        UIUnitSlot selectedSlot = SelectedSlot;
        if (selectedSlot != null)
            selectedSlot.ShowUnitSelect();
    }

    public void DeleteCurrentUnit()
    {
        UIUnitSlot selectedSlot = SelectedSlot;
        if (selectedSlot != null)
            selectedSlot.SetUnitData(null);
    }

    void slot_SlotIsSelected(object sender, EventArgs e)
    {
        if (UnitSlotIsSelected != null)
            UnitSlotIsSelected(sender, e);
        for (int i = 0; i < UnitSlotsRO.Length; i++)
            if (UnitSlotsRO[i] != null)
                UnitSlotsRO[i].SelectSlot(false);
    }

    public void SaveHiredSoldiers()
    {
        List<BaseSoldier> soldiers = new List<BaseSoldier>();
        for (int i = 0; i < UnitSlotsRO.Length; i++)
        {
            if (UnitSlotsRO[i] != null)
            {
                BaseSoldierData soldierData = UnitSlotsRO[i].UnitData;
                if (soldierData != null)
                {
                    BaseSoldier soldier = new BaseSoldier(soldierData,
                        Global.Instance.Player.City.GetSoldierUpgradesInfo(soldierData.Key).Level);
                    soldier.TemplatePlace = UnitSlotsRO[i].Place;
                    soldiers.Add(soldier); 
                }
            }
        }
        Global.Instance.CurrentMission.SelectedSoldiers = new ArrayRO<BaseSoldier>(soldiers.ToArray());

        UnitSlot[] slotTemplate = new UnitSlot[soldiers.Count];
        for (int i = 0; i < slotTemplate.Count(); i++)
            slotTemplate[i] = new UnitSlot(soldiers[i].TemplatePlace, soldiers[i].Data.Key);
        Global.Instance.Player.Heroes.Current.Data.SlotTemplate = new ArrayRO<UnitSlot>(slotTemplate);
    }

    public void Clear()
    {
        for (int i = 0; i < UnitSlotsRO.Length; i++)
        {
            if (UnitSlotsRO[i] != null)
                UnitSlotsRO[i].SetUnitData(null);
        }
    }
}
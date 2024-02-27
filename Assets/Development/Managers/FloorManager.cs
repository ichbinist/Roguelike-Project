using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class FloorManager : Singleton<FloorManager>
{
    [FoldoutGroup("Floor Manager")]
    [FoldoutGroup("Floor Manager/Company")]
    public Company Company;
    [FoldoutGroup("Floor Manager/Company/Settings")]
    [Min(1)]
    public int FloorCount;
    [FoldoutGroup("Floor Manager/Company/Settings")]
    [Min(1)]
    public int SectionCount;


    [FoldoutGroup("Floor Manager/Statistics")]
    [ReadOnly]
    public int CurrentFloorIndex;

    public Floor GetCurrentFloor()
    {
        return Company.FloorList[CurrentFloorIndex];
    }

    private void Start()
    {
        CurrentFloorIndex = 0;
    }

    public void InitializeFloors(int sectionCount, int floorCount)
    {
        FloorCount = floorCount;
        SectionCount = sectionCount;

        Company = new Company(SectionCount, FloorCount);
        MapManager.Instance.StartCoroutine(MapManager.Instance.CreateMapCoroutine());
    }

    public void NextFloor()
    {
        CurrentFloorIndex++;
    }

    public void ResetFloors()
    {
        Company = new Company(SectionCount, FloorCount);
        MapManager.Instance.StartCoroutine(MapManager.Instance.CreateMapCoroutine());
    }

    public void ResetFloor()
    {
        CombatGameManager.Instance.PlayerCharacter.CurrentHealth = CombatGameManager.Instance.PlayerCharacter.MaximumHealth;
        CombatGameManager.Instance.PlayerCharacter.CurrentAmmo = ItemManager.Instance.GetInventoryItemAsWeapon(CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterWeaponController>().CurrentWeaponID).AmmoCapacity;
        CombatGameManager.Instance.PlayerCharacter.transform.position = Vector3.zero;
        RoomManager.Instance.DungeonGenerator.ResetMaze();
        CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterPowerController>().LocalCooldown = 0;
        MapManager.Instance.StartCoroutine(MapManager.Instance.CreateMapCoroutine());
        CombatGameManager.Instance.OnGameReset.Invoke();
    }
}
[System.Serializable]
public class Floor
{
    [ReadOnly]
    public int FloorIndex;

    [ReadOnly]
    public string FloorIndexName;
    [ReadOnly]
    public string SectionName;
}

[System.Serializable]
public class Company
{
    [ReadOnly]
    public List<Floor> FloorList = new List<Floor>();
    [ReadOnly]
    public string CompanyName;

    public Company(int SectionCount, int FloorCount)
    {
        CompanyName = CompanyNameGenerator.GenerateCompanyName();

        for (int i = 0; i < SectionCount; i++)
        {
            string sectionName = SectionNameGenerator.GenerateSectionName();
            for (int j = 0; j < FloorCount; j++)
            {
                Floor floor = new Floor();
                FloorList.Add(floor);
                floor.FloorIndex = j + 1;
                floor.SectionName = sectionName;
                floor.FloorIndexName = (i + 1).ToString() + " - " + floor.FloorIndex.ToString();
            }
        }
    }
}
public static class CompanyNameGenerator
{
    private static readonly string[] NamePrefixes = {
        "Cyber", "Neon", "Synth", "Virtua", "Quantum", "TechNoir", "Infra", "NexGen", "Digi", "Byte",
        "MegaByte", "Nanotech", "Omni", "GenSys", "Quantex", "Holo", "Neura", "SkyNet", "Grid", "Bit",
        "Hyper", "Cryo", "Fiber", "Xenon", "Zeta", "NeuraLink", "Spectra", "CyberLink", "Quantica", "Vivo",
        "Neuro", "Xion", "Vortex", "Pulse", "Cipher", "Ether", "Synex", "NexCorp", "Tron", "Neo", "DataLink",
        "Vapor", "Syntex", "Meta", "NeuraByte", "Plasma", "ZeroG", "OmniLink", "QuantumByte", "TechVibe"
    };

    private static readonly string[] Suffixes = { "Corp.", "Co.", "Inc.", "Ltd.", "LLC." };

    public static string GenerateCompanyName()
    {
        System.Random random = new System.Random();

        // Randomly select a prefix and a suffix
        string prefix = NamePrefixes[random.Next(NamePrefixes.Length)];
        string suffix = Suffixes[random.Next(Suffixes.Length)];

        // Combine the prefix and suffix to form the company name
        string companyName = $"{prefix} {suffix}";

        return companyName;
    }
}

public static class SectionNameGenerator
{
    private static readonly string[] SectionPrefixes = { "Main", "Upper", "Lower", "Sky", "Tech", "Executive", "Nano", "Digital", "Virtual", "Future" };
    private static readonly string[] SectionSuffixes = { "Section", "Level", "Suite", "Hub", "Zone", "Area" };

    public static string GenerateSectionName()
    {
        System.Random random = new System.Random();

        // Randomly select a prefix and a suffix
        string prefix = SectionPrefixes[random.Next(SectionPrefixes.Length)];
        string suffix = SectionSuffixes[random.Next(SectionSuffixes.Length)];

        // Combine the prefix and suffix to form the floor name
        string floorName = $"{prefix} {suffix}";

        return floorName;
    }
}

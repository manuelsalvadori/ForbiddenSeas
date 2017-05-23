﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagshipStatus : NetworkBehaviour
{
    public enum ShipClass {pirates, vikings, egyptians, orientals};

    public ShipClass shipClass;
    public static string shipName;
    public static int m_MaxHealth;
	[SyncVar]
    public float m_Maneuvrability;
	[SyncVar]
    public float m_maxSpeed;

    [SyncVar]
    public int m_Health;
    [SyncVar]
    public int m_reputation = 0;
    [SyncVar]
    public float m_yohoho = 0;
    [SyncVar]
    public bool m_isDead = false;
    [SyncVar]
    public int m_DoT = 0;

    [SyncVar]
    public int m_main, m_special;
    [SyncVar]
    public float m_mainCD, m_specialCD;
    //[SyncList]
    //public bool[] status = {false, false, false, false, false, false};

    public Player m_Me;

    void Start()
    {
        m_Me = gameObject.GetComponent<Player>();
        if(isLocalPlayer)
            StartCoroutine(DmgOverTime());
    }

    public void InitializeFlagshipStatus()
    {
        switch ((int)shipClass)
        {
            case 0:
                shipName = Pirates.shipName;
                m_MaxHealth = Pirates.maxHealth;
                m_Maneuvrability = Pirates.maneuverability;
                m_maxSpeed = Pirates.maxSpeed;
                m_main = Pirates.mainAttackDmg;
                m_special = Pirates.specAttackDmg;
                m_mainCD = Pirates.mainAttackCD;
                m_specialCD = Pirates.specAttackCD;
                break;

            case 1:
                shipName = Vikings.shipName;
                m_MaxHealth = Vikings.maxHealth;
                m_Maneuvrability = Vikings.maneuverability;
                m_maxSpeed = Vikings.maxSpeed;
                m_main = Vikings.mainAttackDmg;
                m_special = Vikings.specAttackDmg;
                m_mainCD = Vikings.mainAttackCD;
                m_specialCD = Vikings.specAttackCD;
                break;

            case 2:
                shipName = Egyptians.shipName;
                m_MaxHealth = Egyptians.maxHealth;
                m_Maneuvrability = Egyptians.maneuverability;
                m_maxSpeed = Egyptians.maxSpeed;
                m_main = Egyptians.mainAttackDmg;
                m_special = Egyptians.specAttackDmg;
                m_mainCD = Egyptians.mainAttackCD;
                m_specialCD = Egyptians.specAttackCD;
                break;

            case 3:
                shipName = Orientals.shipName;
                m_MaxHealth = Orientals.maxHealth;
                m_Maneuvrability = Orientals.maneuverability;
                m_maxSpeed = Orientals.maxSpeed;
                m_main = Orientals.mainAttackDmg;
                m_special = Orientals.specAttackDmg;
                m_mainCD = Orientals.mainAttackCD;
                m_specialCD = Orientals.specAttackCD;
                break;

            default:
                return;
        }

        m_Health = m_MaxHealth;
    }

    [Command]
    public void CmdTakeDamage(int dmg, string a_name, string da_name)
    {
        m_Health -= dmg;
        RpcTakenDamage(a_name, da_name);

        if (m_Health <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        
        if (m_Me.m_LocalTreasure && m_Me.m_HasTreasure)
        {
            m_Me.m_HasTreasure = false;
            StartCoroutine(m_Me.LostTheTreasure());
        }
        if (!m_isDead)
        {
            //Set all the penalties here.
            m_yohoho -= 15f;
            if (m_yohoho < 0)
                m_yohoho = 0;
            //Decrease Reputation
            //Increase Death Count
            //Increase Opponent Kill Count
        }
        m_isDead = true;
        m_reputation += ReputationValues.KILLED;
        m_reputation = (m_reputation < 0) ? 0 : m_reputation;
        GetComponent<Player>().TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);
        m_Health = m_MaxHealth;
        TargetRpcRespawn(GetComponent<NetworkIdentity>().connectionToClient);

    }

    [ClientRpc]
    void RpcTakenDamage(string a, string da)
    {
        Debug.Log("io sono: player " + LocalGameManager.Instance.GetPlayerId(gameObject).ToString() + " Colpito " + a + " da " + da);
    }

    //metodo DoT && HoT
    private IEnumerator DmgOverTime()
    {
        Debug.Log("dentro coroutine");
        while (true)
        {
            yield return new WaitForSeconds(3f);
            CmdTakeDamage(m_DoT, "Player " + LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), "DoT");
        }
    }

    [TargetRpc]
    public void TargetRpcRespawn(NetworkConnection u)
    {
        transform.position = GetComponent<Player>().m_SpawnPoint;
    }

    //status alterati - power-ups

    [Command]
    public void CmdMiasma()
    {
        m_DoT += Orientals.specAttackDmg;
        StartCoroutine(resetDoT(Orientals.specAttackDmg, (float)StatusTiming.POISON_DURATION));
    }

    [Command]
    public void CmdBlind(NetworkIdentity u)
    {
        TargetRpcBlind(u.connectionToClient);
    }

    [TargetRpc]
    private void TargetRpcBlind(NetworkConnection nc)
    {
        Blind bl = GameObject.FindWithTag("Blind").GetComponent<Blind>();
        bl.SetBlind(true);
        StartCoroutine(resetBlind(bl, (float)StatusTiming.BLIND_DURATION));
    }

    [Command]
    public void CmdDamageUp()
    {

    }

    [Command]
    public void CmdSpeedUp()
    {

    }

    [Command]
    public void CmdYohoho()
    {

    }

    [Command]
    public void CmdRegen()
    {
        m_DoT += Symbols.REGEN_AMOUNT;
        StartCoroutine(resetDoT(Symbols.REGEN_AMOUNT, (float)StatusTiming.REGEN_DURATION));
    }

    private IEnumerator resetDoT(int dmg, float duration)
    {
        yield return new WaitForSeconds(duration);
        m_DoT -= dmg;
    }

    private IEnumerator resetBlind(Blind bl, float duration)
    {
        yield return new WaitForSeconds(duration);
        bl.SetBlind(false);
    }
}
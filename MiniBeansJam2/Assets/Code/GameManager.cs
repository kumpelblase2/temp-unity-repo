﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject InteractMenu;

    private GameObject selectedZombie;
    private GameObject currentPlayer;

    public int MenuScene;
    
    public const string ZOMBIE_TAG = "Target";
    public const string PLAYER_TAG = "Player";
    public const string GROUND_TAG = "Ground";

    private List<GameObject> enemyList;

    private KeyCode[] playerKeys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5};

    // Use this for initialization
    void Start()
    {
        enemyList = GameObject.FindGameObjectsWithTag(ZOMBIE_TAG).ToList();
        var players = GameObject.FindGameObjectsWithTag(PLAYER_TAG).ToList();
        if (players.Count > 0)
        {
            currentPlayer = players[0];
            currentPlayer.GetComponent<Player>().SetSelected(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray clickRay;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(1))
        {
            ClearSelectedEnemies();
            clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out hit))
            {
                var colliderGameObject = hit.collider.gameObject;
                if (colliderGameObject.CompareTag(ZOMBIE_TAG))
                {
                    colliderGameObject.GetComponent<FieldOfView>().SetSelected(true);
                }
                else if (colliderGameObject.CompareTag(GROUND_TAG))
                {
                    InteractMenu.SetActive(true);
                    InteractMenu.transform.position = hit.point;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ClearSelectedEnemies();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            DoAttack();
        }

        var players = GameObject.FindGameObjectsWithTag(PLAYER_TAG).ToList();
        for (int i = 0; i < Math.Min(players.Count, playerKeys.Length); i++)
        {
            if (Input.GetKeyDown(playerKeys[i]))
            {
                if (currentPlayer != null)
                {
                    currentPlayer.GetComponent<Player>().SetSelected(false);
                }

                currentPlayer = players[i];
                currentPlayer.GetComponent<Player>().SetSelected(true);
            }
        }

        if (!ArePlayersAlive())
        {
            SceneManager.LoadScene(MenuScene);
        }
    }

    private void DoAttack()
    {
        var player = currentPlayer.GetComponent<Player>();
        if (player.Type != PlayerType.TANK)
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(clickRay, out hit))
            {
                var colliderGameObject = hit.collider.gameObject;
                if (colliderGameObject.CompareTag(ZOMBIE_TAG))
                {
                    AttackZombie(colliderGameObject);
                }
            }
        }
        else
        {
            player.PushAway();
        }
    }

    private void AttackZombie(GameObject zombieObject)
    {
        var player = currentPlayer.GetComponent<Player>();
        var zombie = zombieObject.GetComponent<Zombie>();
        switch (player.Type)
        {
            case PlayerType.ARCHER:
                player.TryShootArrowAt(zombie);
                break;
            case PlayerType.MAGE:
                player.TryThrowExplosiveAt(zombie);
                break;
            
        }
    }

    private void ClearSelectedEnemies()
    {
        foreach (var enemy in enemyList)
        {
            if (enemy != null)
            {
                enemy.GetComponent<FieldOfView>().SetSelected(false);
            }
        }
    }

    private bool ArePlayersAlive()
    {
        var players = GameObject.FindGameObjectsWithTag(PLAYER_TAG).ToList();
        foreach (var player in players)
        {
            if (player.GetComponent<Player>().IsAlive())
            {
                return true;
            }
        }

        return false;
    }
}
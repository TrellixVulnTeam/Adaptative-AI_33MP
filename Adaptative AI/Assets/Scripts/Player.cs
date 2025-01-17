﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum Options
    {
        NONE = -1,
        ATTACK,
        DEFENSE,
        HEAL,
        SPECIAL_ATTACK,
        RECOVER_MANA,
        INCREASE_STATS,
        DECREASE_STATS
    }

    [HideInInspector] public bool playerTurn = false;
    [HideInInspector] public bool endTurn = false;
    [HideInInspector] public bool dead = false;
    public string playerName = "";
    public Player enemy = null;
    public int initialLife = 100;
    public int initialSpeed = 10;
    public int initialAttack = 20;
    public int initialDefense = 10;
    public int initialMana = 50;
    public GameManager gameManager = null;
    public Slider lifeSlider = null;
    public Slider manaSlider = null;
    public Text lifeinitialValueText = null;
    public Text manainitialValueText = null;
    public Text maxTimesToRecoverManaText = null;
    public Text timesToRecoverManaText = null;
    public GameObject decreasePrefab = null;
    public GameObject increasePrefab = null;
    public GameObject healPrefab = null;
    public GameObject manaPrefab = null;
    public GameObject defensePrefab = null;
    public GameObject attackPrefab = null;
    public GameObject specialAttackPrefab = null;
    public Vector3 position;
    public AI componentAI = null;
    int life;
    int speed;
    int attack;
    int defense;
    int mana;
    int levelOfChangeStats = 0;
    int timesToRecoverMana = 0;
    bool firstFrame = true;
    float defendingBonus = 1;
    Options optionChoosen;
    public bool training = false;
    protected int finalLife = 0;
    protected int finalEnemyLife = 0;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!playerTurn || endTurn)
            return;

        if (optionChoosen == Options.NONE)
        {
            if (firstFrame && componentAI == null)
            {
                gameManager.Log(playerName + " please choose an option for this turn.");
            }
            else if (firstFrame)
            {
                componentAI.ChooseOption();
            }
            firstFrame = false;
            return;
        }


        ExecuteOption();
        optionChoosen = Options.NONE;
        endTurn = true;
        firstFrame = true;
    }

    public void MatchEnded(bool victory)
    {
        if (componentAI != null && !gameManager.GetEndGame())
        {
            componentAI.MatchEnded(victory);
        }
        finalLife = life;
        finalEnemyLife = enemy.life;
    }

    void ExecuteOption()
    {
        defendingBonus = 1;
        switch (optionChoosen)
        {
            case Options.ATTACK:
                Instantiate(attackPrefab, enemy.position, Quaternion.identity, enemy.gameObject.transform);
                if (!training)
                {
                    gameManager.Log(playerName + " has attacked " + enemy.playerName);
                }
                Attack(gameManager.basicAttackDamage);
                break;
            case Options.DEFENSE:
                Instantiate(defensePrefab, position, Quaternion.identity, gameObject.transform);
                mana -= gameManager.manaSpentWithDefense;
                manaSlider.value = mana;
                defendingBonus = defendingBonus + ((float)gameManager.percentageDefense / 100f);
                if (!training)
                {
                    gameManager.Log(playerName + " is defending");
                }
                break;
            case Options.SPECIAL_ATTACK:
                Instantiate(specialAttackPrefab, position, Quaternion.identity, gameObject.transform);
                if (!training)
                {
                    gameManager.Log(playerName + " has used a special attack against " + enemy.playerName);
                }
                mana -= gameManager.manaSpentWithSpecialAttack;
                manaSlider.value = mana;
                Attack(gameManager.manaAttackDamage);
                break;
            case Options.RECOVER_MANA:
                Instantiate(manaPrefab, position, Quaternion.identity, gameObject.transform);
                timesToRecoverMana--;
                if (timesToRecoverManaText != null)
                {
                    timesToRecoverManaText.text = timesToRecoverMana.ToString();
                }
                RecoverMana();
                break;
            case Options.HEAL:
                Instantiate(healPrefab, position, Quaternion.identity, gameObject.transform);
                mana -= gameManager.manaSpentWithHealing;
                manaSlider.value = mana;
                Heal();
                break;
            case Options.INCREASE_STATS:
                if (!training)
                {
                    gameManager.Log(playerName + " has increased its stats");
                }
                mana -= gameManager.manaSpentWithIncreasingStats;
                manaSlider.value = mana;
                ChangeStats(true, this);
                levelOfChangeStats++;
                Instantiate(increasePrefab, position, Quaternion.identity, gameObject.transform);
                break;
            case Options.DECREASE_STATS:
                if (!training)
                {
                    gameManager.Log(playerName + " has decreased " + enemy.playerName + " stats");
                }
                mana -= gameManager.manaSpentWithDecreasingStats;
                manaSlider.value = mana;
                ChangeStats(false, enemy);
                enemy.levelOfChangeStats--;
                Instantiate(decreasePrefab, enemy.position,Quaternion.identity, enemy.gameObject.transform);
                break;
        }
    }


    public void DecideOption(Options option)
    {
        optionChoosen = option;
        endTurn = true;
    }

    void Heal()
    {
        float percentageInDecimals = (float)gameManager.percentageHealing / 100f;
        float lifeToRecover = initialLife * percentageInDecimals;
        life += (int)lifeToRecover;
        if (life > initialLife)
        {
            life = initialLife;
        }
        lifeSlider.value = life;

        if (!training)
        {
            gameManager.Log(playerName + " has recovered life");
        }
    }

    void ChangeStats(bool positiveChange, Player playerToAffect)
    {
        int multiplierOfLevel = positiveChange ? 1 : -1;
        float percentageInDecimals = (float)gameManager.percentageChangingStats / 100f;
        float attackChanged= playerToAffect.initialAttack * percentageInDecimals * multiplierOfLevel;
        playerToAffect.attack += (int)attackChanged;
        float defenseChanged = playerToAffect.initialDefense * percentageInDecimals * multiplierOfLevel;
        playerToAffect.defense += (int)defenseChanged;
        float speedChanged = playerToAffect.initialSpeed * percentageInDecimals * multiplierOfLevel;
        playerToAffect.speed += (int)speedChanged;
    }

    void RecoverMana()
    {
        float percentageInDecimals = (float)gameManager.percentageRecoveryMana / 100f;
        float manaToRecover = initialMana * percentageInDecimals;
        mana += (int)manaToRecover;
        if (mana > initialMana)
        {
            mana = initialMana;
        }
        manaSlider.value = mana;
        if (!training)
        {
            gameManager.Log(playerName + " has recovered mana");
        }
    }
    void Attack(int attackDamage)
    {
        int damage = (attack - (int)(enemy.defense * enemy.defendingBonus)) * attackDamage;
        enemy.TakeDamage(damage < 0 ? 0 : damage);
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        life = life < 0 ? 0 : life;
        if (life <= 0)
        {
            dead = true;
        }
        lifeSlider.value = life;
    }

    public int getSpeed() { return speed; }

    public int getLife() { return life; }

    public int getMana() { return mana; }

    public int getLevelOfChangeStats() { return levelOfChangeStats; }

    public int getTimesToRecoverMana() { return timesToRecoverMana; }

    public float getDefendingBonus() { return defendingBonus; }

    public virtual void Reset()
    {
        firstFrame = true;
        life = initialLife;
        lifeSlider.value = life;
        attack = initialAttack;
        defense = initialDefense;
        speed = initialSpeed;
        mana = initialMana;
        manaSlider.value = mana;
        endTurn = true;
        playerTurn = false;
        dead = false;
        optionChoosen = Options.NONE;
        defendingBonus = 1;
        levelOfChangeStats = 0;
        timesToRecoverMana = gameManager.maxOfTimesToRecoverMana;
        lifeinitialValueText.text = initialLife.ToString();
        manainitialValueText.text = initialMana.ToString();
        if (maxTimesToRecoverManaText != null)
        {
            maxTimesToRecoverManaText.text = gameManager.maxOfTimesToRecoverMana.ToString();
        }
        if (timesToRecoverManaText != null)
        {
            timesToRecoverManaText.text = timesToRecoverMana.ToString();
        }
        if (componentAI != null)
        {
            finalEnemyLife = finalLife = 0;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : AI
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ChooseOption()
    {
        bool optionValid;
        Player.Options optionChosen;
        do
        {
            optionChosen = (Player.Options)Random.Range(0, 7);
            optionValid = CheckIfOptionIsValid(optionChosen);
        } while (!optionValid);
        player.DecideOption(optionChosen);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Models;


namespace Heisenslaught.DataTransfer
{
    public class CreateDraftDTO
    {
        public int firstPick;
        public int pickTime;
        public int bonusTime;
        public bool bankTime;
        public string team1Name;
        public string team2Name;
        public string map;
        public List<string> disabledHeroes;

        public CreateDraftDTO(DraftModel model = null)
        {
            if(model != null)
            {
                firstPick = model.config.firstPick;
                pickTime = model.config.pickTime;
                bonusTime = model.config.bonusTime;
                bankTime = model.config.bankTime;
                team1Name = model.config.team1Name;
                team2Name = model.config.team2Name;
                map = model.config.map;
                disabledHeroes = model.config.disabledHeroes;
            }
        }

        public DraftConfigModel ToModel()
        {
            var model = new DraftConfigModel();
            model.bankTime = bankTime;
            model.bonusTime = bonusTime;
            model.disabledHeroes = disabledHeroes;
            model.firstPick = firstPick;
            model.map = map;
            model.pickTime = pickTime;
            model.team1Name = team1Name;
            model.team2Name = team2Name;
            return model;
        }
    }
}

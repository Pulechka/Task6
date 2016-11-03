﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.Entities;

namespace UsersKeeper.BllContracts
{
    public interface IAwardLogic
    {
        bool Add(string title);
        IEnumerable<Award> GetAll();
    }
}

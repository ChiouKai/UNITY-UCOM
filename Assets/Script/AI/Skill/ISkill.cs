using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    AI ai { get; }
    string Name { get; }
    int AP { get; }
    int CD { get; }
    int CDCount { get; }
    int Point { get; }
    string Func();
    
}

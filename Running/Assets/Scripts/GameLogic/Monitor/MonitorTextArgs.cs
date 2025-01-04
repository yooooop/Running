using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorTextArgs : EventArgs
{
    public int OpponentFirstNum { get; }
    public int OpponentSecondNum { get; }
    public int PlayerFirstNum { get; }
    public int PlayerSecondNum { get; }
    public string PlayerOperation { get; }
    public string OpponentOperation { get; }
    public int PlayerLife { get; }
    public int OpponentLife { get; }
    public int PlayerResult { get; }
    public int OpponentResult { get; }
    public int Round { get; }

    public MonitorTextArgs(int ofn, int osn, int pfn, int psn, string po, string oo, int pl, int ol, int pr, int or, int round)
    {
        OpponentFirstNum = ofn;
        OpponentSecondNum = osn;
        PlayerFirstNum = pfn;
        PlayerSecondNum = psn;
        PlayerOperation = po;
        OpponentOperation = oo;
        PlayerLife = pl;
        OpponentLife = ol;
        PlayerResult = pr;
        OpponentResult = or;
        Round = round;
    }

}

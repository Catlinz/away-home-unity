using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComputerSystem {
    public ModifiableFloat cpu;
    public int cpuUsed;
    public float health;

    public float AvailableCpu {
        get { return cpu - (float)cpuUsed; }
    }

    public ComputerSystem() {
        cpu = 0;
        cpuUsed = 0;
        health = 100;
    }

    public ComputerSystem(ComputerSystem src) {
        cpu = src.cpu;
        cpuUsed = src.cpuUsed;
        health = src.health;
    }

    public void FreeCpu(int cpu) {
        cpuUsed -= cpu;
        if (cpuUsed < 0) { cpuUsed = 0; }
    }

    public void UseCpu(int cpu) {
        cpuUsed += cpu;
    }



}

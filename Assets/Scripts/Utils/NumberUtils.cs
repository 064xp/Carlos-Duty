using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberUtils 
{
    public static double Map(double x, double in_min, double in_max, double out_min, double out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}

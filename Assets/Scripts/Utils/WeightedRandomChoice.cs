using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandomChoice 
{
    // https://softwareengineering.stackexchange.com/a/150642
    public static T RandomChoice<T>(WeightedValue<T>[] valueList) {
        float totalWeight = 0; // this stores sum of weights of all elements before current
        T selected = default(T); // currently selected element

        foreach (var data in valueList) {
            float r = Random.Range(0, totalWeight + data.weight); // random value
            if (r >= totalWeight) // probability of this is weight/(totalWeight+weight)
                selected = data.value; // it is the probability of discarding last selected element and selecting current one instead
            totalWeight += data.weight; // increase weight sum
        }

        return selected; // when iterations end, selected is some element of sequence. 
    }
}

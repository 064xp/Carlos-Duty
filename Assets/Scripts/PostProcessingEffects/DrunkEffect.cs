using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class DrunkEffect : MonoBehaviour
{
    private struct QueuedEffect {
        public QueuedEffect(float weight, float speed) {
            this.weight = weight;
            this.speed = speed;
        }
        public float weight;
        public float speed;
    };

    public float incrementSpeed = 0.3f;
    public float startDecayDelay = 1.5f;
    public float decaySpeed = 0.05f;
    [SerializeField]
    private float decayAfter = 0f;
    private Queue<QueuedEffect> queue;
    private Volume volume;

    private void Start() {
        volume = GetComponent<Volume>();
        queue = new Queue<QueuedEffect>();
    }

    private void Update() {
        //print(Time.time);
        EffectDecay();
    }

    private void EffectDecay() {
        if(Time.time >= decayAfter) {
            volume.weight = Mathf.Clamp01(volume.weight - decaySpeed);
        }
    }

    public void ApplyEffect(float weight) {
        StartCoroutine(EnqueueEffect(weight));
    }

    IEnumerator EnqueueEffect(float weight) {
        QueuedEffect queuedEffect = new QueuedEffect(weight, incrementSpeed);

        queue.Enqueue(queuedEffect);
        if (queue.Count > 1) {
            yield break;
        }

        while(queue.Count > 0) {
            QueuedEffect currentEffect = queue.Peek();
            float targetWeight = Mathf.Clamp01(volume.weight + currentEffect.weight);
            while(volume.weight < targetWeight) {
                volume.weight += currentEffect.speed;
                yield return null;
            }
            queue.Dequeue();
            decayAfter = Time.time + startDecayDelay;
        }
    }

    public void SetEffectWeight(float weight) {
        volume.weight = Mathf.Clamp(weight, 0f, 1f);
        if (weight == 0f) volume.enabled = false;
        else if (weight > 0f && !volume.enabled) volume.enabled = true;
    }
}

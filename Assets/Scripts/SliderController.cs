using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    [Range(0,1)]public float decreaseSpeed = 1f;
    [Range(0, 5)] public float refillSpeed = 1f;

    private Coroutine currentRoutine;

    private void Start()
    {
        slider.value = slider.maxValue;
        //currentRoutine = StartCoroutine(RefillSlider());
    }
    public void DecreaseSliderOnTap()
    {
        if (slider.value > 0)
        {
            slider.value -= decreaseSpeed;
           // currentRoutine = StartCoroutine(RefillSlider());
        }
    }
    private void Update()
    {
        if (slider.value < slider.maxValue)
        {
            if (slider.value < slider.maxValue / 20)
            {
                slider.value += (refillSpeed / 2) * Time.deltaTime;
            }
            else
            {
                slider.value += refillSpeed * Time.deltaTime;
            }
            // slider.value += refillSpeed * Time.deltaTime;
        }
    }
   
    private IEnumerator RefillSlider()
    {
        //while (slider.value < slider.maxValue)
        //{
        //    if (slider.value < slider.maxValue / 10)
        //    {
        //        slider.value += refillSpeed * Time.deltaTime;
        //    }
        //    else if (slider.value < slider.maxValue / 7)
        //    {
        //        slider.value += refillSpeed * 4 * Time.deltaTime;
        //    }
        //    else if(slider.value < slider.maxValue / 4)
        //    {
        //        slider.value += refillSpeed * 8 * Time.deltaTime;
        //    }
        //    else
        //    {
        //        slider.value += refillSpeed * 10 * Time.deltaTime;
        //    }

        //    yield return null;
        //}
        Debug.Log("here11");
        while (slider.value < slider.maxValue)
        {
            Debug.Log("here");
            //float progress = slider.value / slider.maxValue;

            //// Gradually increase speed based on progress
            //float dynamicSpeed = Mathf.Lerp(refillSpeed, refillSpeed * 10f, progress);

            slider.value += refillSpeed * Time.deltaTime;
            yield return null;
        }

        slider.value = slider.maxValue; // Snap to max to avoid overshooting
    }
}

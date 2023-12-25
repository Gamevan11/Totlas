using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicators : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image foodBar;
    [SerializeField] private Image waterBar;

    [SerializeField] private Text healthText;
    [SerializeField] private Text foodText;
    [SerializeField] private Text waterText;

    [SerializeField] private float maxFoodAmount = 500f; // Максимальное значение еды
    [SerializeField] private float maxWaterAmount = 500f; // Максимальное значение воды
    [SerializeField] private float maxHealthAmount = 100f; // Максимальное значение здоровья

    private float foodAmount; // Текущее значение еды
    private float waterAmount; // Текущее значение воды
    private float healthAmount; // Текущее значение здоровья

    [SerializeField] private float foodDepletionRate = 500f / 60f; // Скорость убывания еды
    [SerializeField] private float waterDepletionRate = 500f / 30f; // Скорость убывания воды
    [SerializeField] private float healthDepletionRate = 100f / 60f; // Скорость убывания здоровья

    private void Start()
    {
        // Начальная установка полос и текстовых значений
        foodAmount = maxFoodAmount;
        waterAmount = maxWaterAmount;
        healthAmount = maxHealthAmount;
        UpdateUI();
    }

    private void Update()
    {
        UpdateFood();
        UpdateWater();
        UpdateHealth();
    }

    private void UpdateUI()
    {
        // Обновление всех полос и текстовых значений
        UpdateBarAndText(healthBar, healthText, healthAmount);
        UpdateBarAndText(foodBar, foodText, foodAmount);
        UpdateBarAndText(waterBar, waterText, waterAmount);
    }

    private void UpdateFood()
    {
        if (foodAmount > 0)
        {
            foodAmount -= foodDepletionRate * Time.deltaTime;
            if (foodAmount < 0)
            {
                foodAmount = 0;
            }
        }
        else
        {
            healthAmount -= (healthDepletionRate * Time.deltaTime) * (maxFoodAmount / maxHealthAmount);
        }

        // Обновляем полосу и текст для еды после каждого обновления
        UpdateBarAndText(foodBar, foodText, foodAmount);
    }

    private void UpdateWater()
    {
        if (waterAmount > 0)
        {
            waterAmount -= waterDepletionRate * Time.deltaTime;
            if (waterAmount < 0)
            {
                waterAmount = 0;
            }
        }
        else
        {
            healthAmount -= (healthDepletionRate * Time.deltaTime) * (maxWaterAmount / maxHealthAmount);
        }

        // Обновляем полосу и текст для воды после каждого обновления
        UpdateBarAndText(waterBar, waterText, waterAmount);
    }

    private void UpdateHealth()
    {
        if (foodAmount > 0 && waterAmount > 0)
        {
            // Если и еда, и вода есть, здоровье не уменьшается
        }
        else
        {
            healthAmount -= healthDepletionRate * Time.deltaTime;
        }

        // Обновляем полосу и текст для здоровья после каждого обновления
        UpdateBarAndText(healthBar, healthText, healthAmount);
    }

    private void UpdateBarAndText(Image bar, Text text, float value)
    {
        // Округляем значение до целых
        float roundedValue = Mathf.Round(value);

        // Обновляем текстовое значение
        text.text = roundedValue.ToString();

        // Обновляем полосу
        bar.fillAmount = roundedValue / maxHealthAmount;
    }
}
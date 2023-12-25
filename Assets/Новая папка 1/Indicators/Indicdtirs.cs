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

    [SerializeField] private float maxFoodAmount = 500f; // ������������ �������� ���
    [SerializeField] private float maxWaterAmount = 500f; // ������������ �������� ����
    [SerializeField] private float maxHealthAmount = 100f; // ������������ �������� ��������

    private float foodAmount; // ������� �������� ���
    private float waterAmount; // ������� �������� ����
    private float healthAmount; // ������� �������� ��������

    [SerializeField] private float foodDepletionRate = 500f / 60f; // �������� �������� ���
    [SerializeField] private float waterDepletionRate = 500f / 30f; // �������� �������� ����
    [SerializeField] private float healthDepletionRate = 100f / 60f; // �������� �������� ��������

    private void Start()
    {
        // ��������� ��������� ����� � ��������� ��������
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
        // ���������� ���� ����� � ��������� ��������
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

        // ��������� ������ � ����� ��� ��� ����� ������� ����������
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

        // ��������� ������ � ����� ��� ���� ����� ������� ����������
        UpdateBarAndText(waterBar, waterText, waterAmount);
    }

    private void UpdateHealth()
    {
        if (foodAmount > 0 && waterAmount > 0)
        {
            // ���� � ���, � ���� ����, �������� �� �����������
        }
        else
        {
            healthAmount -= healthDepletionRate * Time.deltaTime;
        }

        // ��������� ������ � ����� ��� �������� ����� ������� ����������
        UpdateBarAndText(healthBar, healthText, healthAmount);
    }

    private void UpdateBarAndText(Image bar, Text text, float value)
    {
        // ��������� �������� �� �����
        float roundedValue = Mathf.Round(value);

        // ��������� ��������� ��������
        text.text = roundedValue.ToString();

        // ��������� ������
        bar.fillAmount = roundedValue / maxHealthAmount;
    }
}